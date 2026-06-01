//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Default <see cref="ITableauFileJsonStream"/> implementation.
    /// </summary>
    public class TableauFileJsonStream : ITableauFileJsonStream
    {
        private readonly CancellationToken _disposalCancel;
        private readonly bool _leaveOpen;

        private JsonNode? _json;

        /// <inheritdoc />
        public Stream JsonContent { get; }

        /// <summary>
        /// Creates a new <see cref="TableauFileJsonStream"/> object.
        /// </summary>
        /// <param name="jsonContent">The JSON stream.</param>
        /// <param name="disposalCancel">A cancellation token to obey, and to use when the editor is disposed.</param>
        /// <param name="leaveOpen">Whether or not to close the stream on disposal.</param>
        public TableauFileJsonStream(Stream jsonContent, CancellationToken disposalCancel, bool leaveOpen = false)
        {
            if (!jsonContent.CanSeek || !jsonContent.CanRead || !jsonContent.CanWrite)
            {
                throw new ArgumentException("JSON stream requires a seekable read/write stream.", nameof(jsonContent));
            }

            JsonContent = jsonContent;
            _disposalCancel = disposalCancel;
            _leaveOpen = leaveOpen;
        }

        /// <inheritdoc />
        public async Task<JsonNode> GetJsonAsync(CancellationToken cancel)
        {
            if (_json is not null)
            {
                return _json;
            }

            JsonContent.Seek(0, SeekOrigin.Begin);
            using (var doc = await JsonDocument.ParseAsync(JsonContent, cancellationToken: cancel).ConfigureAwait(false))
            {
                _json = JsonNode.Parse(doc.RootElement.GetRawText());
            }

            JsonContent.Seek(0, SeekOrigin.Begin);
            return _json!;
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

            if (_json is not null)
            {
                JsonContent.SetLength(0);
                JsonContent.Seek(0, SeekOrigin.Begin);

                using var writer = new Utf8JsonWriter(JsonContent, new JsonWriterOptions { Indented = true });
                _json.WriteTo(writer);
                await writer.FlushAsync(_disposalCancel).ConfigureAwait(false);
            }

            if (!_leaveOpen)
            {
                await JsonContent.DisposeAsync().ConfigureAwait(false);
            }

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

