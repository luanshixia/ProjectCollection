using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Simplest
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = System.IO.File.ReadAllText("simple.json");

            var simple = JsonConvert.DeserializeObject<Simple>(json);
            Console.WriteLine(simple);

            Console.ReadLine();
        }
    }

    public class Simple
    {
        [JsonProperty(Required = Required.Always)]
        public SimpleObject[] Array { get; set; }
    }

    [JsonConverter(typeof(LineInfoConverter))]
    public class SimpleObject : JsonLineInfo
    {
        public string Value { get; set; }
    }

    public class JsonLineInfo
    {
        [JsonIgnore]
        public int? LineNumber { get; set; }

        [JsonIgnore]
        public int? LinePosition { get; set; }
    }

    public class LineInfoConverter : JsonConverter
    {
        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Converter is not writable. Method should not be invoked");
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(JsonLineInfo).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var lineInfoObject = Activator.CreateInstance(objectType) as JsonLineInfo;
            serializer.Populate(reader, lineInfoObject);

            var jsonLineInfo = reader as IJsonLineInfo;
            if (jsonLineInfo != null && jsonLineInfo.HasLineInfo())
            {
                lineInfoObject.LineNumber = jsonLineInfo.LineNumber;
                lineInfoObject.LinePosition = jsonLineInfo.LinePosition;
            }

            return lineInfoObject;
        }
    }
}
