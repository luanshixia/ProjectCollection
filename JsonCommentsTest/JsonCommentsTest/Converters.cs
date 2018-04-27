using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace JsonCommentsTest
{
    /// <summary>
    /// Overrides the default CamelCase resolver to respect property name set in the <c>JsonPropertyAttribute</c>.
    /// </summary>
    internal class CamelCasePropertyNamesWithOverridesContractResolver : CamelCasePropertyNamesContractResolver
    {
        /// <summary>
        /// Creates <c>JSON</c> property for the class member.
        /// </summary>
        /// <param name="member">The member to serialize.</param>
        /// <param name="memberSerialization">The type of member serialization.</param>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var result = base.CreateProperty(member, memberSerialization);

            var attributes = member.GetCustomAttributes(attributeType: typeof(JsonPropertyAttribute), inherit: true);
            if (attributes.Any())
            {
                var propertyName = attributes.Cast<JsonPropertyAttribute>().Single().PropertyName;
                if (!string.IsNullOrEmpty(propertyName))
                {
                    result.PropertyName = propertyName;
                }
            }

            return result;
        }

        /// <summary>
        /// Creates dictionary contract
        /// </summary>
        /// <param name="objectType">The object type.</param>
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            var contract = base.CreateDictionaryContract(objectType);

            var attributes = objectType.GetCustomAttributes(attributeType: typeof(JsonPreserveCaseDictionaryAttribute), inherit: true);
            if (attributes.Any())
            {
                contract.DictionaryKeyResolver = propertyName => propertyName;
            }

            return contract;
        }
    }

    /// <summary>
    /// The attribute to preserve the letter case for dictionary keys.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class JsonPreserveCaseDictionaryAttribute : Attribute
    {
    }

    /// <summary>
    /// The line info converter.
    /// </summary>
    internal class LineInfoConverter : JsonConverter
    {
        /// <summary>
        /// Gets a value indicating whether this converter can write JSON.
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Converter is not writable. Method should not be invoked");
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        public override bool CanConvert(Type objectType)
        {
            return typeof(JsonLineInfo).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Reads the <c>JSON</c>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <param name="serializer">The serializer.</param>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            int lineNumber = 0;
            int linePosition = 0;
            var jsonLineInfo = reader as IJsonLineInfo;
            if (jsonLineInfo != null && jsonLineInfo.HasLineInfo())
            {
                lineNumber = jsonLineInfo.LineNumber;
                linePosition = jsonLineInfo.LinePosition;
            }

            var lineInfoObject = FastActivator.CreateInstance(objectType) as JsonLineInfo;
            serializer.Populate(reader, lineInfoObject);

            lineInfoObject.LineNumber = lineNumber;
            lineInfoObject.LinePosition = linePosition;

            return lineInfoObject;
        }
    }

    /// <summary>
    /// The line info class.
    /// </summary>
    public class JsonLineInfo
    {
        /// <summary>
        /// Determines whether object has line information.
        /// </summary>
        public bool HasLineInfo()
        {
            return this.LineNumber.HasValue || this.LinePosition.HasValue;
        }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        [JsonIgnore]
        public int? LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the line position.
        /// </summary>
        [JsonIgnore]
        public int? LinePosition { get; set; }
    }

    /// <summary>
    /// The fast instance of a type activator.
    /// </summary>
    /// <remarks>
    /// Activator.CreateInstance is using reflection to create instances which can substantially affect performance for frequently instantiated types.
    /// FastActivator.CreateInstance is using lightweight code generation expression trees and then compile it to a delegate to avoid performance penalty.
    /// </remarks>
    public static class FastActivator
    {
        /// <summary>
        /// The instance of a type factories.
        /// </summary>
        private static ConcurrentDictionary<Type, Func<object>> factories = new ConcurrentDictionary<Type, Func<object>>();

        /// <summary>
        /// Creates the instance of the type.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        public static T CreateInstance<T>()
            where T : class, new()
        {
            return FastActivator.Factory<T>.Instance();
        }

        /// <summary>
        /// Creates the instance of the type.
        /// </summary>
        /// <param name="type">The type of instance to create.</param>
        public static object CreateInstance(Type type)
        {
            return FastActivator.GetFactory(type)();
        }

        /// <summary>
        /// Gets the instance of a type factory.
        /// </summary>
        /// <param name="type">The type of instance to create.</param>
        public static Func<object> GetFactory(Type type)
        {
            Func<object> factory;
            return FastActivator.factories.TryGetValue(type, out factory)
                ? factory
                : FastActivator.CreateFactory(type);
        }

        /// <summary>
        /// Creates the instance of a type factory.
        /// </summary>
        /// <param name="type">The type of instance to create.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Func<object> CreateFactory(Type type)
        {
            return FastActivator.factories[type] = FastActivator.CompileFactory<object>(type);
        }

        /// <summary>
        /// Compiles the instance of a type factory.
        /// </summary>
        /// <typeparam name="T">The type of factory return value.</typeparam>
        /// <param name="type">The type of instance to create.</param>
        private static Func<T> CompileFactory<T>(Type type)
        {
            var dynamicMethod = new DynamicMethod(
                name: "FastActivator.Factory." + typeof(T).Name + "." + type.Name,
                returnType: type,
                parameterTypes: null,
                owner: type,
                skipVisibility: true);

            var generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Ret);

            return (Func<T>)dynamicMethod.CreateDelegate(typeof(Func<T>));
        }

        /// <summary>
        /// The instance of a type factory cache.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        public static class Factory<T>
            where T : class, new()
        {
            /// <summary>
            /// The cached instance of a type factory.
            /// </summary>
            public static readonly Func<T> Instance = FastActivator.CompileFactory<T>(typeof(T));
        }
    }

    /// <summary>
    /// The TimeSpan converter based on ISO 8601 format.
    /// </summary>
    internal class TimeSpanConverter : JsonConverter
    {
        /// <summary>
        /// Writes the <c>JSON</c>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, XmlConvert.ToString((TimeSpan)value));
        }

        /// <summary>
        /// Reads the <c>JSON</c>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <param name="serializer">The serializer.</param>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.TokenType != JsonToken.Null ? (object)XmlConvert.ToTimeSpan(serializer.Deserialize<string>(reader)) : null;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan) || objectType == typeof(TimeSpan?);
        }
    }

    /// <summary>
    /// The 'AdjustToUniversal' ISO 8601 date time converter.
    /// </summary>
    public class AdjustToUniversalIsoDateTimeConverter : IsoDateTimeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdjustToUniversalIsoDateTimeConverter"/> class.
        /// </summary>
        public AdjustToUniversalIsoDateTimeConverter()
        {
            this.DateTimeStyles = DateTimeStyles.AdjustToUniversal;
            this.Culture = CultureInfo.InvariantCulture;
        }
    }
}
