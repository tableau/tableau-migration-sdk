//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Text;
using System.Threading.Tasks;

namespace Tableau.Migration.Net.Rest
{
    internal static class HttpRequestMessageExtensions
    {
        public static void SetRestAuthenticationToken(this HttpRequestMessage request, string? token)
        {
            request.Headers.Remove(RestHeaders.AuthenticationToken);

            if (token is not null)
            {
                request.Headers.TryAddWithoutValidation(RestHeaders.AuthenticationToken, token);
            }
        }

        /// <summary>
        /// The default ToString() method for HttpRequestMessage includes the X-Tableau-Auth header, which is a security risk.
        /// </summary>
        public static string ToSanitizedString(this HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var sb = new StringBuilder();

            sb.AppendLine($"Method: {request.Method}");
            sb.AppendLine($"RequestUri: {request.RequestUri}");
            sb.AppendLine($"Version: {request.Version}");
            sb.AppendLine($"Content: {request.Content?.ReadAsStringAsync().GetAwaiter().GetResult() ?? "null"}");

            return sb.ToString();
        }
    }
}
