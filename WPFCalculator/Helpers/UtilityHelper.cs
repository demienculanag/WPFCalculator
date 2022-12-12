using System;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace WPFCalculator.Helpers
{
    public static class UtilityHelper
    {
        public static void CopyStringValue(string value)
        {
            Clipboard.SetText(value);
        }
        public static string GetClipboardValue()
        {
            return Clipboard.GetText();
        }

        public static string SerializeObjectToXML(object obj)
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var xmlSerializer = new XmlSerializer(obj.GetType());
                var emptyXmlSerializerNamespace = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                xmlSerializer.Serialize(stringwriter, obj, emptyXmlSerializerNamespace);
                return stringwriter.ToString();
            }
        }

        public static void DownloadXMLFile(object obj, string fileName)
        {
            var toDownloadXmlString = SerializeObjectToXML(obj);
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter writer = new StreamWriter($"{path}/{fileName}.xml"))
            {
                writer.Write(toDownloadXmlString);
                writer.Flush();
            }
            MessageBox.Show($"File Successfully Exported to {path}");
            return;

        }
       
        public static T DeserializeXmlString<T>(string xml)
          where T : new()
        {
            if (string.IsNullOrEmpty(xml))
            {
                return new T();
            }

            using (var stringReader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }

        }

    }
}
