using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.ClientXml.Models
{
    [XmlType("successful_login")]
    internal class SuccessfulLogin
    {
        [XmlElement("authenticity_token")]
        public string? AuthenticityToken { get; set; }

        [XmlElement("workgroup_session_id")]
        public string? WorkgroupSessionId { get; set; }

        [XmlElement("user")]
        public UserType? User { get; set; }

        [XmlElement("settings")]
        public SettingsType? Settings { get; set; }

        [XmlElement("error")]
        public ErrorType? Error { get; set; }

        public class ErrorType
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

        public class UserType
        {
            [XmlElement("id")]
            public int Id { get; set; }

            [XmlElement("name")]
            public string? Name { get; set; }

            [XmlElement("friendly_name")]
            public string? FriendlyName { get; set; }

            [XmlElement("administrator")]
            public bool IsAdministrator { get; set; }

            [XmlElement("site_namespace")]
            public string? SiteUrlNamespace { get; set; }

            [XmlElement("site_prefix")]
            public string? SitePrefix { get; set; }

            [XmlElement("site_displayname")]
            public string? SiteDisplayName { get; set; }

            [XmlElement("admin_type")]
            public string? AdminTypeValue { get; set; }
        }

        public class SettingsType
        {
            [XmlElement("saas_enabled")]
            public bool SaasEnabled { get; set; }
        }
    }
}
