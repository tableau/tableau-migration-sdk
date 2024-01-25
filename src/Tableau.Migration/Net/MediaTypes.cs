using System.Net.Http.Headers;
using System.Net.Mime;

namespace Tableau.Migration.Net
{
    internal class MediaTypes
    {
        public static readonly MediaTypeWithQualityHeaderValue Json = new(MediaTypeNames.Application.Json);
        public static readonly MediaTypeWithQualityHeaderValue Xml = new(MediaTypeNames.Application.Xml);
        public static readonly MediaTypeWithQualityHeaderValue OctetStream = new(MediaTypeNames.Application.Octet);
        public static readonly MediaTypeWithQualityHeaderValue TextXml = new(MediaTypeNames.Text.Xml);
    }
}
