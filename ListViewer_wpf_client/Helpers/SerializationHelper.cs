using System;
using System.IO;
using System.Xml.Serialization;

namespace ListViewer_wpf_client.Helpers
{
    public static class SerializationHelper
    {
        public static T Deserialize<T>(String input)
            where T : class
        {
            var reader = new StringReader(input);
            var serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(reader) as T;
        }

        public static String Serialize<T>(T input)
        {
            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof (T));
            serializer.Serialize(stream, input);
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}