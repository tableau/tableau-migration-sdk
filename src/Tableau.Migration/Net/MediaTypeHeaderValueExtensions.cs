using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace Tableau.Migration.Net
{
    internal static class MediaTypeHeaderValueExtensions
    {
        /// <summary>
        /// List of MIME types outside of text/* that should be treated as text content for logging purposes.
        /// </summary>
        private static readonly HashSet<string> _loggingOtherTextMediaTypes = new(
            new[]
            {
                MediaTypeNames.Application.Json,
                "application/javascript",
                "application/ecmascript",
                MediaTypeNames.Application.Rtf,
                "application/typescript",
                "application/xhtml+xml",
                MediaTypeNames.Application.Xml,
                "application/sql",
                "application/x-www-form-urlencoded",
                "application/graphql"
            },
            StringComparer.OrdinalIgnoreCase);

        internal static bool IsUtf8(this MediaTypeHeaderValue header)
        {
            if (header.CharSet is not null)
            {
                try
                {
                    var encoding = Encoding.GetEncoding(header.CharSet);

                    if (encoding == Encoding.UTF8)
                        return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        internal static bool IsJson(this MediaTypeHeaderValue header) => IsMediaType(header, MediaTypeNames.Application.Json);

        internal static bool IsXml(this MediaTypeHeaderValue header) => IsMediaType(header, MediaTypeNames.Application.Xml);

        internal static bool IsText(this MediaTypeHeaderValue header)
        {
            // Media type cannot be null
            if (header.MediaType!.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        internal static bool LogsAsText(this MediaTypeHeaderValue header)
        {
            if (header.IsText())
            {
                return true;
            }

            // Media type cannot be null
            if (_loggingOtherTextMediaTypes.Contains(header.MediaType!))
            {
                return true;
            }

            return false;
        }

        private static bool IsMediaType(MediaTypeHeaderValue header, string mediaType)
        {
            return string.Equals(header.MediaType, mediaType, StringComparison.OrdinalIgnoreCase);
        }
    }
}
