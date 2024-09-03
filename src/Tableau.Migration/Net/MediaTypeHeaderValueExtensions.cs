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

        internal static bool IsHtml(this MediaTypeHeaderValue header) => IsMediaType(header, MediaTypeNames.Text.Html);

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
