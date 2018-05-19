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

        public static void Write(Object value, TextWriter writer)
        {
            JsonSerializer.Create().Serialize(writer, value);
        }
    }
}
