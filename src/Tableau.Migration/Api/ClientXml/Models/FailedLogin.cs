using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.ClientXml.Models
{
    [XmlType("error")]
    internal class FailedLogin
    {
        [XmlArray("sites")]
        [XmlArrayItem("site")]
        public SiteType[] Sites { get; set; } = Array.Empty<SiteType>();

        public class SiteType
        {
            [XmlAttribute("id")]
            public string? UrlNamespace { get; set; }

            [XmlText]
            public string? DisplayName { get; set; }
        }
    }
}
