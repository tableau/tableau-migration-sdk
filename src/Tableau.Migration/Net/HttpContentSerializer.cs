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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Net
{
    internal class HttpContentSerializer : IHttpContentSerializer
    {
        public static readonly HttpContentSerializer Instance = new(TableauSerializer.Instance);

        private readonly ITableauSerializer _serializer;

        public HttpContentSerializer(ITableauSerializer serializer)
        {
            _serializer = serializer;
        }

        public virtual async Task<T?> DeserializeAsync<T>(HttpContent content, CancellationToken cancel)
        {
            var data = default(T);

            /* Tableau APIs sometimes return HTML error pages.
             * We consider those deserialization failures,
             * but want to include the HTML content in the exception
             * so users can check it for debugging purposes.
            */
            if(content.IsHtmlContent())
            {
                var htmlContent = await content.ReadAsEncodedStringAsync(cancel).ConfigureAwait(false);
                throw new FormatException("Server responded with HTML error page: " + htmlContent);
            }

            if (content.IsXmlContent())
            {
                var stringContent = await content.ReadAsEncodedStringAsync(cancel).ConfigureAwait(false);

                if (stringContent is null || stringContent.Length == 0)
                    return data;

                data = _serializer.DeserializeFromXml<T>(stringContent);
            }
            else if (content.IsJsonContent())
            {
                //UF8 deserialization is much faster and common enough to special-case.
                if (content.IsUtf8Content())
                {
                    var utf8Data = await content.ReadAsByteArrayAsync(cancel).ConfigureAwait(false);
                    return _serializer.DeserializeFromJson<T>(utf8Data);
                }
                else
                {
                    var stringContent = await content.ReadAsEncodedStringAsync(cancel).ConfigureAwait(false);

                    if (stringContent is null || stringContent.Length == 0)
                        return data;

                    return _serializer.DeserializeFromJson<T>(stringContent);
                }
            }
            else
            {
                throw new NotSupportedException($"Content Type {content.Headers.ContentType?.MediaType ?? "<null>"} not supported.");
            }

            return data;
        }

        public virtual StringContent? Serialize<TContent>(TContent content, MediaTypeWithQualityHeaderValue contentType)
            where TContent : class
        {
            string? stringContent = null;

            if (contentType.IsXml())
            {
                stringContent = _serializer.SerializeToXml(content);
            }
            else if (contentType.IsJson())
            {
                stringContent = _serializer.SerializeToJson(content);
            }

            if (stringContent is null)
                return null;

#if NET7_0_OR_GREATER
            return new StringContent(stringContent, Constants.DefaultEncoding, contentType);
#else
            return new StringContent(stringContent, Constants.DefaultEncoding, contentType.MediaType);
#endif
        }

        public virtual async Task<Error?> TryDeserializeErrorAsync(HttpContent content, CancellationToken cancel)
        {
            if (content is null || (content.Headers?.ContentLength ?? 0) == 0)
                return null;

            EmptyTableauServerResponse? tsResponse = null;

            try
            {
                tsResponse = await DeserializeAsync<EmptyTableauServerResponse>(content, cancel).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // No-op, deserialization failures are fine here since it might not be a TableauServerResponse at all.
            }

            return tsResponse?.Error;
        }
    }
}
