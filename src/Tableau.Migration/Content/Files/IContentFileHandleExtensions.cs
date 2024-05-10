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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Files
{
    internal static class IContentFileHandleExtensions
    {
        internal static async Task CloseTableauFileEditorAsync(this IContentFileHandle contentFileHandle, CancellationToken cancel)
            => await contentFileHandle.Store.CloseTableauFileEditorAsync(contentFileHandle, cancel).ConfigureAwait(false);

        internal static async Task<bool> IsZipAsync(this IContentFileHandle handle, CancellationToken cancel)
        {
            var isZipFile = IsZipFile(h => h.GetOriginalFilePath()) ?? IsZipFile(h => h.GetStoreFilePath());

            if (isZipFile is not null)
                return isZipFile.Value;

            var fileStream = await handle.OpenReadAsync(cancel).ConfigureAwait(false);

            await using (fileStream)
            {
                return fileStream.Content.IsZip();
            }

            bool? IsZipFile(Func<IContentFileHandle, FilePath> getFilePath)
                => getFilePath(handle).IsZipFile;
        }

        internal static FilePath GetOriginalFilePath(this IContentFileHandle handle)
            => new(handle.OriginalFileName);

        internal static FilePath GetStoreFilePath(this IContentFileHandle handle)
            => new(handle.Path);
    }
}