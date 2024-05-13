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
    /// <summary>
    /// Interface describing a logical file that is part of a content item.
    /// </summary>
    /// <param name="Store">The file store the handle is for.</param>
    /// <param name="Path">The path to the file.</param>
    /// <param name="OriginalFileName">The original filename of the file, used for the upload filename when publishing the content item.</param>
    public record ContentFileHandle(IContentFileStore Store, string Path, string OriginalFileName)
        : IContentFileHandle
    {
        private bool _disposed = false;

        /// <inheritdoc />
        public virtual async Task<IContentFileStream> OpenReadAsync(CancellationToken cancel)
            => await Store.OpenReadAsync(this, cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public virtual async Task<IContentFileStream> OpenWriteAsync(CancellationToken cancel)
            => await Store.OpenWriteAsync(this, cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public virtual async Task<ITableauFileXmlStream> GetXmlStreamAsync(CancellationToken cancel)
        {
            //We expect the editor/stream will be disposed automatically by the preparer before publish. 
            var editor = await Store.GetTableauFileEditorAsync(this, cancel).ConfigureAwait(false);
            return editor.GetXmlStream();
        }

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
            await Store.DeleteAsync(this, default).ConfigureAwait(false);

            _disposed = true;

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
