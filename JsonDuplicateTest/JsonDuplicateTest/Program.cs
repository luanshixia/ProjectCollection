using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace JsonDuplicateTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var model0 = new Model { A = "9", B = "9" };
            var json0 = JsonConvert.SerializeObject(model0, new DuplicateKeyConverter());
            Console.WriteLine(json0);

            var json = @"{ ""a"": ""1"", ""b"": ""2"", ""a"": ""3"" }";
            var model = JsonConvert.DeserializeObject<Model>(json, new DuplicateKeyConverter());
            Console.WriteLine(model);
            Console.ReadLine();
        }
    }

    public class Model
    {
        public string A { get; set; }
        public string B { get; set; }

        public override string ToString()
        {
            return $"A: {A}; B: {B}";
        }
    }

    public class DuplicateKeyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsClass;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var paths = new HashSet<string>();
            existingValue = existingValue ?? Activator.CreateInstance(objectType, true);

            var backup = new StringWriter();
            using (var writer = new JsonTextWriter(backup))
            {
                do
                {
                    writer.WriteToken(reader.TokenType, reader.Value);

                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (paths.Contains(reader.Path))
                        {
                            throw new ArgumentException();
                        }

                        paths.Add(reader.Path);
                    }
                }
                while (reader.Read());
            }

            JsonConvert.PopulateObject(backup.ToString(), existingValue);
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }
}
