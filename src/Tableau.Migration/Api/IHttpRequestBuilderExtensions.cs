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

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Static class containing extension methods for <see cref="IHttpRequestBuilder"/> objects.
    /// </summary>
    internal static class IHttpRequestBuilderExtensions
    {
        /// <summary>
        /// Sends the request and treats the response as a file download.
        /// </summary>
        public static async Task<IAsyncDisposableResult<FileDownload>> DownloadAsync(this IHttpRequestBuilder requestBuilder, CancellationToken cancel)
        {
            //Send with ResponseHeadersRead completion option
            //so we do not internally buffer the file stream.
            var result = await requestBuilder
                .SendAsync(HttpCompletionOption.ResponseHeadersRead, cancel)
                .DownloadResultAsync(cancel)
                .ConfigureAwait(false);

            return result;
        }
    }
}
