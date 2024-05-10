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
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IO;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Default <see cref="ITableauFileEditor"/> implementation.
    /// </summary>
    public class TableauFileEditor : ITableauFileEditor
    {
        private readonly IContentFileHandle _fileStoreFile;
        private readonly CancellationToken _disposalCancel;

        private ITableauFileXmlStream? _xmlStream;
        private bool _disposed = false;

        /// <inheritdoc />
        public RecyclableMemoryStream Content { get; }

        /// <inheritdoc />
        public ZipArchive? Archive { get; }

        private bool XmlStreamOwnsContent => Archive is not null;

        private bool CleanupContent => !XmlStreamOwnsContent || _xmlStream is null;

        /// <summary>
        /// Creates a new <see cref="TableauFileEditor"/> object.
        /// </summary>
        /// <param name="fileStoreFile">The file store file to edit.</param>
        /// <param name="content">
        /// The memory backed stream
        /// with unencrypted tableau file data 
        /// to write back to the file store upon disposal.
        /// </param>
        /// <param name="archive">The zip archive to use to manipulate the file, or null to consider the file as a single XML file.</param>
        /// <param name="disposalCancel">A cancellation token to obey, and to use when the editor is disposed.</param>
        public TableauFileEditor(
            IContentFileHandle fileStoreFile, 
            RecyclableMemoryStream content,
            ZipArchive? archive,
            CancellationToken disposalCancel)
        {
            _fileStoreFile = fileStoreFile;
            Content = content;
            Archive = archive;
            _disposalCancel = disposalCancel;

            Content.Seek(0, SeekOrigin.Begin);
        }

        internal static bool IsXmlFile(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();
            return extension switch
            {
                ".twb" or ".tds" => true,
                _ => false
            };
        }

        /// <inheritdoc />
        public ITableauFileXmlStream GetXmlStream()
        {
            if (_xmlStream is not null)
            {
                return _xmlStream;
            }

            var leaveOpen = !XmlStreamOwnsContent;
            if (Archive is null)
            {
                _xmlStream = new TableauFileXmlStream(Content, _disposalCancel, leaveOpen: leaveOpen);
            }
            else
            {
                var xmlEntry = Archive.Entries.Single(e => IsXmlFile(e.Name));
                _xmlStream = new TableauFileXmlStream(xmlEntry.Open(), _disposalCancel, leaveOpen: leaveOpen);
            }

            return _xmlStream;
        }

        /// <summary>
        /// Opens a new Tableau file editor.
        /// </summary>
        /// <param name="handle">The file store file to edit.</param>
        /// <param name="memoryStreamManager">The memory stream manager.</param>
        /// <param name="cancel">A cancellation token to obey, and to use when the editor is disposed.</param>
        /// <param name="zipFormatOverride">
        /// True to consider the file a zip archive, 
        /// false to consider the file an XML file, 
        /// or null to detect whether the file is a zip archive.
        /// </param>
        /// <returns>The newly created file editor.</returns>
        public static async Task<TableauFileEditor> OpenAsync(
            IContentFileHandle handle,
            IMemoryStreamManager memoryStreamManager,
            CancellationToken cancel,
            bool? zipFormatOverride = null)
        {
            var fileStream = await handle.OpenReadAsync(cancel).ConfigureAwait(false);

            var outputStream = memoryStreamManager.GetStream(handle.OriginalFileName);

            await using (fileStream)
            {
                //Read the file into a seekable memory stream
                //that the ZipArchive requires for update mode.
                await fileStream.Content.CopyToAsync(outputStream, cancel).ConfigureAwait(false);
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            var isZip = zipFormatOverride == true || await handle.IsZipAsync(cancel).ConfigureAwait(false);

            var archive = isZip ? new ZipArchive(outputStream, ZipArchiveMode.Update, leaveOpen: true) : null;

            outputStream.Seek(0, SeekOrigin.Begin);

            return new(handle, outputStream, archive, cancel);
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
            if (_xmlStream is not null)
            {
                await _xmlStream.DisposeAsync().ConfigureAwait(false);
            }

            if (Archive is not null)
            {
                await Task.Run(() => Archive.Dispose()).ConfigureAwait(false);
            }

            var fileStoreStream = await _fileStoreFile.OpenWriteAsync(_disposalCancel).ConfigureAwait(false);
            await using (fileStoreStream)
            {
                Content.Seek(0, SeekOrigin.Begin);
                await Content.CopyToAsync(fileStoreStream.Content, _disposalCancel)
                    .ConfigureAwait(false);
            }

            if (CleanupContent)
            {
                await Content.DisposeAsync().ConfigureAwait(false);
            }

            _disposed = true;
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
