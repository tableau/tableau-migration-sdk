using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Tableau.Migration.Net
{
    internal static class ObjectExtensions
    {
        public static string ToXml<T>(this T obj)
        {
            var ns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            var settings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8
            };

            var serializer = new XmlSerializer(obj?.GetType() ?? typeof(T));

            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            serializer.Serialize(xmlWriter, obj, ns);

            return stringWriter.ToString();
        }

        public static string ToJson<T>(
            this T obj)
            => JsonSerializer.Serialize(
                obj,
                JsonOptions.Default);
    }
}
