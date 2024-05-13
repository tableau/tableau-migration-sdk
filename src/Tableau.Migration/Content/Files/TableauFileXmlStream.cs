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
using System.Xml.Linq;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Default <see cref="ITableauFileXmlStream"/> implementation.
    /// </summary>
    public class TableauFileXmlStream : ITableauFileXmlStream
    {
        private readonly CancellationToken _disposalCancel;
        private readonly bool _leaveOpen;

        private XDocument? _xml;

        /// <inheritdoc />
        public Stream XmlContent { get; }

        /// <summary>
        /// Creates a new <see cref="TableauFileXmlStream"/> object.
        /// </summary>
        /// <param name="xmlContent">The XML stream.</param>
        /// <param name="disposalCancel">A cancellation tokey to obey, and to use when the editor is disposed.</param>
        /// <param name="leaveOpen">Whether or not to close the stream on disposal.</param>
        public TableauFileXmlStream(Stream xmlContent, CancellationToken disposalCancel, bool leaveOpen = false)
        {
            if (!xmlContent.CanSeek || !xmlContent.CanRead || !xmlContent.CanWrite)
            {
                throw new ArgumentException("XML stream requires a seekable read/write stream.", nameof(xmlContent));
            }

            XmlContent = xmlContent;
            _disposalCancel = disposalCancel;
            _leaveOpen = leaveOpen;
        }

        /// <inheritdoc />
        public async Task<XDocument> GetXmlAsync(CancellationToken cancel)
        {
            if (_xml is not null)
            {
                return _xml;
            }

            return _xml = await XDocument.LoadAsync(XmlContent, LoadOptions.PreserveWhitespace, cancel)
                        .ConfigureAwait(false);
        }

        #region - IAsyncDisposable Implementation -

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public virtual async ValueTask DisposeAsync()
        {
            // Perform async cleanup.

            if (_xml is not null)
            {
                XmlContent.SetLength(0);
                XmlContent.Seek(0, SeekOrigin.Begin);
                await _xml.SaveAsync(XmlContent, SaveOptions.None, _disposalCancel)
                    .ConfigureAwait(false);
            }

            if (!_leaveOpen)
            {
                await XmlContent.DisposeAsync().ConfigureAwait(false);
            }

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
