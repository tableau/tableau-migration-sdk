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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Interface for an object that can store and manage files for content items.
    /// </summary>
    public interface IContentFileStore : IAsyncDisposable
    {
        /// <summary>
        /// Indicates if the store has any Tableau File Editor Open.
        /// </summary>
        bool HasOpenTableauFileEditor { get; }

        /// <summary>
        /// Creates a file managed by the file store.
        /// </summary>
        /// <param name="relativeStorePath">The relative path and file name to create a file for within the file store.</param>
        /// <param name="originalFileName">The original file name external to the file store to preserve when publishing content items.</param>
        /// <returns>A handle to the newly created file.</returns>
        IContentFileHandle Create(string relativeStorePath, string originalFileName);

        /// <summary>
        /// Creates a file managed by the file store.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="contentItem">The content item to resolve a relative file store path from.</param>
        /// <param name="originalFileName">The original file name external to the file store to preserve when publishing content items.</param>
        /// <returns>A handle to the newly created file.</returns>
        IContentFileHandle Create<TContent>(TContent contentItem, string originalFileName);

        /// <summary>
        /// Creates a file managed by the file store.
        /// </summary>
        /// <param name="relativeStorePath">The relative path and file name to create a file for within the file store.</param>
        /// <param name="originalFileName">The original file name external to the file store to preserve when publishing content items.</param>
        /// <param name="initialContent">The initial content to save the file with.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A handle to the newly created file.</returns>
        public async Task<IContentFileHandle> CreateAsync(string relativeStorePath, string originalFileName,
            Stream initialContent, CancellationToken cancel)
        {
            var handle = Create(relativeStorePath, originalFileName);

            var writeStream = await OpenWriteAsync(handle, cancel).ConfigureAwait(false);
            await using (writeStream)
            {
                await initialContent.CopyToAsync(writeStream.Content, cancel).ConfigureAwait(false);
            }

            return handle;
        }

        /// <summary>
        /// Creates a file managed by the file store.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="contentItem">The content item to resolve a relative file store path from.</param>
        /// <param name="originalFileName">The original file name external to the file store to preserve when publishing content items.</param>
        /// <param name="initialContent">The initial content to save the file with.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A handle to the newly created file.</returns>
        public async Task<IContentFileHandle> CreateAsync<TContent>(TContent contentItem, string originalFileName,
            Stream initialContent, CancellationToken cancel)
        {
            var handle = Create(contentItem, originalFileName);

            var writeStream = await OpenWriteAsync(handle, cancel).ConfigureAwait(false);
            await using (writeStream)
            {
                await initialContent.CopyToAsync(writeStream.Content, cancel).ConfigureAwait(false);
            }

            return handle;
        }

        /// <summary>
        /// Opens a stream to read from a file.
        /// </summary>
        /// <param name="handle">The handle to the file to read from.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The stream to read from.</returns>
        Task<IContentFileStream> OpenReadAsync(IContentFileHandle handle, CancellationToken cancel);

        /// <summary>
        /// Opens a stream to write to a file.
        /// </summary>
        /// <param name="handle">The handle to the file to write to.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The stream to write to.</returns>
        Task<IContentFileStream> OpenWriteAsync(IContentFileHandle handle, CancellationToken cancel);

        /// <summary>
        /// Gets the current Tableau file format editor for the content file, 
        /// opening a new editor if necessary.
        /// </summary>
        /// <param name="handle">The handle to the file to get the editor for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <param name="zipFormatOverride">
        /// True to consider the file a zip archive, 
        /// false to consider the file an XML file, 
        /// or null to detect whether the file is a zip archive.
        /// </param>
        /// <returns>
        /// The editor to use.
        /// Changes made will be flushed automatically before the content item is published.
        /// </returns>
        Task<ITableauFileEditor> GetTableauFileEditorAsync(IContentFileHandle handle, CancellationToken cancel, bool? zipFormatOverride = null);

        /// <summary>
        /// Closes the current Tableau file format editor for the content file, 
        /// if one is open.
        /// Changes will be flushed before closing.
        /// </summary>
        /// <param name="handle">The handle to the file to close the editor for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A task to await.</returns>
        Task CloseTableauFileEditorAsync(IContentFileHandle handle, CancellationToken cancel);

        /// <summary>
        /// Deletes a file managed by the file store.
        /// </summary>
        /// <param name="handle">The handle to the file to delete.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A task to await.</returns>
        Task DeleteAsync(IContentFileHandle handle, CancellationToken cancel);
    }
}
