using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Dreambuild.IO
{
    public static class Json
    {
        public static object Decode(string value)
        {
            return JsonConvert.DeserializeObject(value);
        }

        public static object Decode(string value, Type targetType)
        {
            return JsonConvert.DeserializeObject(value, targetType);
        }

        public static T Decode<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static string Encode(Object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static object Read(TextReader reader, Type targetType)
        {
            return JsonSerializer.Create().Deserialize(reader, targetType);
        }

        public static T Read<T>(TextReader reader)
        {
            return (T)JsonSerializer.Create().Deserialize(reader, typeof(T));
        }

        public static void Write(Object value, TextWriter writer)
        {
            JsonSerializer.Create().Serialize(writer, value);
        }
    }
}
