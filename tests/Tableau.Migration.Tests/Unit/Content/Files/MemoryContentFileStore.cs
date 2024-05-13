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
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class MemoryContentFileStore : IContentFileStore
    {
        private readonly IMemoryStreamManager _memoryStreamManager;

        private readonly ConcurrentDictionary<string, byte[]> _fileData = new();
        private readonly ConcurrentDictionary<string, ITableauFileEditor> _editors = new();

        public bool HasOpenTableauFileEditor => _editors.Any();

        public MemoryContentFileStore(IMemoryStreamManager memoryStreamManager)
        {
            _memoryStreamManager = memoryStreamManager;
        }

        internal MemoryContentFileStore()
            : this(MemoryStreamManager.Instance)
        { }

        public byte[] GetFileData(string path) => _fileData[path];

        public virtual IContentFileHandle Create(string relativePath, string originalFileName)
        {
            _fileData.GetOrAdd(relativePath, Array.Empty<byte>());
            return new ContentFileHandle(this, relativePath, originalFileName);
        }

        public virtual IContentFileHandle Create<TContent>(TContent contentItem, string originalFileName)
            => Create(Guid.NewGuid().ToString(), originalFileName);

        public virtual Task DeleteAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            _fileData.TryRemove(handle.Path, out var _);
            return Task.CompletedTask;
        }

        public virtual ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }

        public virtual Task<IContentFileStream> OpenReadAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            IContentFileStream stream = new ContentFileStream(new MemoryStream(_fileData[handle.Path]));
            return Task.FromResult(stream);
        }

        public virtual Task<IContentFileStream> OpenWriteAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            IContentFileStream writableStream = new WritableMemoryContentFileStream(data =>
            {
                _fileData.AddOrUpdate(handle.Path, data, (k, v) => data);
            });
            return Task.FromResult(writableStream);
        }

        public virtual async Task<ITableauFileEditor> GetTableauFileEditorAsync(IContentFileHandle handle,
            CancellationToken cancel, bool? zipFormatOverride = null)
            => await _editors.GetOrAddAsync<string, ITableauFileEditor>(handle.Path, async (path) => 
                await TableauFileEditor.OpenAsync(handle, _memoryStreamManager, cancel, zipFormatOverride).ConfigureAwait(false)).ConfigureAwait(false);

        public virtual async Task CloseTableauFileEditorAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            if (_editors.TryRemove(handle.Path, out var editor))
            {
                await editor.DisposeAsync();
            }
        }
    }
}
