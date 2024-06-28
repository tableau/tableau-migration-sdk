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

using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Api
{
    internal static class IContentFileStoreExtensions
    {
        /// <summary>
        /// Creates a file managed by the file store from a download stream.
        /// </summary>
        /// <param name="store">The file store to save to.</param>
        /// <param name="contentItem">The content item to resolve a relative file store path from.</param>
        /// <param name="download">The downloaded file.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A handle to the newly created file.</returns>
        public static async Task<IContentFileHandle> CreateAsync<T>(this IContentFileStore store, T contentItem, FileDownload download, CancellationToken cancel)
            => await store.CreateAsync(contentItem, download.Filename ?? string.Empty, download.Content, cancel).ConfigureAwait(false);
    }
}
