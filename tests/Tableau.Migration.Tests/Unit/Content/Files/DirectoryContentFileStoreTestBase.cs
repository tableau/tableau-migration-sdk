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

using System.IO;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public abstract class DirectoryContentFileStoreTestBase<TFileStore> : ContentFileStoreTestBase<TFileStore>
        where TFileStore : DirectoryContentFileStore
    {
        protected string BaseStorePath => GetBaseStorePath(FileStore);

        protected ConcurrentSet<string> TrackedFilePaths => GetTrackedFilePaths(FileStore);

        protected string ExpectedBasePath => Path.Combine(RootPath, BaseRelativePath);

        protected string GetBaseStorePath(TFileStore fileStore) => (fileStore.GetPropertyValue(typeof(DirectoryContentFileStore), "BaseStorePath") as string)!;

        protected ConcurrentSet<string> GetTrackedFilePaths(TFileStore fileStore) => (fileStore.GetPropertyValue(typeof(DirectoryContentFileStore), "TrackedFilePaths") as ConcurrentSet<string>)!;

        protected async Task<IContentFileHandle> CreateTestFileAsync(string relativePath, string originalFileName, string? content = null)
        {
            await using var fileData = new MemoryStream(Constants.DefaultEncoding.GetBytes(content ?? Create<string>()));

            return await ((IContentFileStore)FileStore).CreateAsync(relativePath, originalFileName, fileData, Cancel);
        }
    }
}
