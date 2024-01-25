using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.ClientXml.Models
{
    /// <summary>
    /// Quasi union type of <see cref="SuccessfulLogin"/> and <see cref="FailedLogin"/>.
    /// </summary>
    internal class SuccessfulOrFailedLogin : IXmlSerializable
    {
        internal const string SUCCESSFUL_LOGIN_LOCAL_NAME = "successful_login";
        internal const string FAILED_LOGIN_LOCAL_NAME = "error";

        public SuccessfulLogin? SuccessfulLogin { get; set; }
        public FailedLogin? FailedLogin { get; set; }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.LocalName == SUCCESSFUL_LOGIN_LOCAL_NAME)
            {
                SuccessfulLogin = (SuccessfulLogin?)new XmlSerializer(typeof(SuccessfulLogin)).Deserialize(reader);
                FailedLogin = null;
            }
            else if (reader.LocalName == FAILED_LOGIN_LOCAL_NAME)
            {
                SuccessfulLogin = null;
                FailedLogin = (FailedLogin?)new XmlSerializer(typeof(FailedLogin)).Deserialize(reader);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(SuccessfulOrFailedLogin)} cannot deserialize root element named \"{reader.LocalName}\"");
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException($"{nameof(SuccessfulOrFailedLogin)} only supports serialization, not deserialization");
        }
    }
}
