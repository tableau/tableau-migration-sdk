//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net
{
    internal static class HttpContentExtensions
    {
        internal static bool IsUtf8Content(this HttpContent content)
            => content.Headers.ContentType?.IsUtf8() is true;

        internal static bool IsHtmlContent(this HttpContent content)
            => content.Headers.ContentType?.IsHtml() is true;

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

                return Constants.DefaultEncoding.GetString(decoded);
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
