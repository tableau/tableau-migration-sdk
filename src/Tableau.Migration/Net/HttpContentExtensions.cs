using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net
{
    internal static class HttpContentExtensions
    {
        internal static bool IsUtf8Content(this HttpContent content)
            => content.Headers.ContentType?.IsUtf8() is true;

        internal static bool IsXmlContent(this HttpContent content)
            => content.Headers.ContentType?.IsXml() is true;

        internal static bool IsJsonContent(this HttpContent content)
            => content.Headers.ContentType?.IsJson() is true;

        internal static bool IsTextContent(this HttpContent content)
            => content.Headers.ContentType?.IsText() is true;

        internal static bool LogsAsTextContent(this HttpContent content)
            => content.Headers.ContentType?.LogsAsText() is true;

        internal static async Task<string> ReadAsEncodedStringAsync(this HttpContent content, CancellationToken cancel)
        {
            if (content.IsUtf8Content())
            {
                //Handle character sets that aren't supported by .NET standard
                //but that we know about.
                var decoded = await content.ReadAsByteArrayAsync(cancel).ConfigureAwait(false);

                return Encoding.UTF8.GetString(decoded);
            }

            //fall back to the standard way of reading strings.
            var s = await content.ReadAsStringAsync(cancel).ConfigureAwait(false);

            return s;
        }

        internal static TContent AddContent<TContent>(this TContent content, string key, object value, bool? predicate = null)
            where TContent : MultipartContent
        {
            var add = predicate.GetValueOrDefault(true);

            if (!add)
                return content;

            string? formatted;

            if (value is string stringValue)
                formatted = stringValue;
            else if (value is bool boolValue)
                formatted = boolValue.ToString().ToLower();
            else if (value is IFormattable formattableValue)
                formatted = formattableValue?.ToString(null, CultureInfo.InvariantCulture);
            else
                formatted = value?.ToString();

            var stringContent = new StringContent(formatted ?? string.Empty);

            if (content is MultipartFormDataContent formDataContent)
            {
                formDataContent.Add(stringContent, key);
            }
            else
            {
                stringContent.Headers.TryAddWithoutValidation("Content-Disposition", $"name={key}");
                stringContent.Headers.ContentType = null;
                content.Add(stringContent);
            }

            return content;
        }
    }
}
