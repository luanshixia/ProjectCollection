using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BinaryFormatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;
using XmlSerializer = System.Xml.Serialization.XmlSerializer;

namespace Dreambuild.IO
{
    public static class Serialization
    {
        public static void XmlSave<T>(T obj, string fileName, params Type[] extraTypes)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), extraTypes);
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                serializer.Serialize(fs, obj);
            }
        }

        public static T XmlLoad<T>(string fileName, params Type[] extraTypes)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), extraTypes);
            using (System.IO.Stream fs = System.IO.File.OpenRead(fileName))
            {
                T result = (T)serializer.Deserialize(fs);
                return result;
            }
        }

        public static void BinarySave<T>(T obj, string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                bf.Serialize(fs, obj);
            }
        }

        public static T BinaryLoad<T>(string fileName, System.Runtime.Serialization.SerializationBinder binder)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Binder = binder;
            using (System.IO.Stream fs = System.IO.File.OpenRead(fileName))
            {
                T result = (T)bf.Deserialize(fs);
                return result;
            }
        }
    }

    public static class Xml
    {
        public static T XmlDecode<T>(this string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (System.IO.StringReader sr = new System.IO.StringReader(xml))
            {
                T result = (T)serializer.Deserialize(sr);
                return result;
            }
        }

        public static string XmlEncode<T>(this T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                serializer.Serialize(sw, obj);
                return sw.ToString();
            }
        }
    }
}
