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
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Abstract <see cref="IContentFileStore"/> implementation that stores
    /// files in a file system directory.
    /// </summary>
    public class DirectoryContentFileStore : IContentFileStore
    {
        private readonly ConcurrentDictionary<string, ITableauFileEditor> _openTableauFileEditors = new();
        private bool _disposed = false;

        /// <inheritdoc />
        public bool HasOpenTableauFileEditor => _openTableauFileEditors.Any();

        /// <summary>
        /// Gets the file system to use.
        /// </summary>
        protected IFileSystem FileSystem { get; }

        /// <summary>
        /// Gets the content file path resolver.
        /// </summary>
        protected IContentFilePathResolver PathResolver { get; }

        /// <summary>
        /// Gets the memory stream manager.
        /// </summary>
        protected IMemoryStreamManager MemoryStreamManager { get; }

        /// <summary>
        /// Gets the content files being tracked by the file store.
        /// </summary>
        protected ConcurrentSet<string> TrackedFilePaths { get; }

        /// <summary>
        /// Gets the base path of the file store.
        /// </summary>
        protected string BaseStorePath { get; }

        /// <summary>
        /// Creates a new <see cref="DirectoryContentFileStore"/> object.
        /// </summary>
        /// <param name="fileSystem">The file system to use.</param>
        /// <param name="pathResolver">The path resolver to use.</param>
        /// <param name="configReader">A config reader to get the root path and other options from.</param>
        /// <param name="memoryStreamManager">The memory stream manager to user.</param>
        /// <param name="storeDirectoryName">The relative directory name to use for this file store.</param>
        public DirectoryContentFileStore(
            IFileSystem fileSystem,
            IContentFilePathResolver pathResolver,
            IConfigReader configReader,
            IMemoryStreamManager memoryStreamManager,
            string storeDirectoryName)
        {
            FileSystem = fileSystem;
            PathResolver = pathResolver;
            MemoryStreamManager = memoryStreamManager;
            TrackedFilePaths = new();

            var config = configReader.Get();

            var rootPath = config.Files.RootPath;
            BaseStorePath = Path.Combine(rootPath, storeDirectoryName);
        }

        /// <inheritdoc />
        private Task DeleteAsync(string path, CancellationToken cancel)
        {
            return Task.Run(() =>
            {
                if (FileSystem.File.Exists(path))
                {
                    FileSystem.File.Delete(path);
                }

                TrackedFilePaths.Remove(path);
            }, cancel);
        }

        #region - IContentFileStore Implementation -

        /// <inheritdoc />
        public IContentFileHandle Create(string relativeStorePath, string originalFileName)
        {
            Guard.AgainstNullOrWhiteSpace(relativeStorePath, nameof(relativeStorePath));

            string path = Path.Combine(BaseStorePath, relativeStorePath);

            TrackedFilePaths.Add(path);

            return new ContentFileHandle(this, path, originalFileName);
        }

        /// <inheritdoc />
        public IContentFileHandle Create<TContent>(TContent contentItem, string originalFileName)
            => Create(PathResolver.ResolveRelativePath(contentItem, originalFileName), originalFileName);

        /// <inheritdoc />
        public Task<IContentFileStream> OpenReadAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            var fs = FileSystem.File.OpenRead(handle.Path);
            return Task.FromResult<IContentFileStream>(new ContentFileStream(fs));
        }

        /// <inheritdoc />
        public Task<IContentFileStream> OpenWriteAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            var dir = Path.GetDirectoryName(handle.Path);
            if (!string.IsNullOrEmpty(dir))
            {
                FileSystem.Directory.CreateDirectory(dir);
            }

            var fs = FileSystem.File.Open(handle.Path, FileMode.Create);

            return Task.FromResult<IContentFileStream>(new ContentFileStream(fs));
        }

        /// <inheritdoc />
        public async Task<ITableauFileEditor> GetTableauFileEditorAsync(IContentFileHandle handle, CancellationToken cancel, bool? zipFormatOverride = null)
            => await _openTableauFileEditors.GetOrAddAsync(
                handle.Path,
                async path => (ITableauFileEditor)await TableauFileEditor.OpenAsync(handle, MemoryStreamManager, cancel, zipFormatOverride).ConfigureAwait(false))
                .ConfigureAwait(false);

        /// <inheritdoc />
        public async Task CloseTableauFileEditorAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            if (_openTableauFileEditors.TryRemove(handle.Path, out var editor))
            {
                await editor.DisposeAsync().ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public async Task DeleteAsync(IContentFileHandle handle, CancellationToken cancel)
        {
            await CloseTableauFileEditorAsync(handle, cancel).ConfigureAwait(false);
            await DeleteAsync(handle.Path, cancel).ConfigureAwait(false);
        } 

        #endregion

        #region - IAsyncDisposable Implementation -

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public virtual async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            // Perform async cleanup.
            foreach (var path in TrackedFilePaths.ToImmutableArray())
            {
                await DeleteAsync(path, default).ConfigureAwait(false);
            }

            if (HasOpenTableauFileEditor)
            {
                // Remove open editors
                foreach (var editor in _openTableauFileEditors.Values)
                {
                    await editor.DisposeAsync().ConfigureAwait(false);
                }
                _openTableauFileEditors.Clear();
            }

            if (FileSystem.Directory.Exists(BaseStorePath))
            {
                FileSystem.Directory.Delete(BaseStorePath, true);
            }

            _disposed = true;

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
