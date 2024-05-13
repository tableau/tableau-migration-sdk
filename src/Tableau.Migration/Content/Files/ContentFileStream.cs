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
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Default <see cref="IContentFileStream"/> implementation
    /// that owns and disposes of a content file stream.
    /// </summary>
    public class ContentFileStream : IContentFileStream
    {
        /// <inheritdoc />
        public Stream Content { get; }

        /// <summary>
        /// Creates a new <see cref="ContentFileStream"/> object.
        /// </summary>
        /// <param name="content">The content stream to take ownership of.</param>
        public ContentFileStream(Stream content)
        {
            Content = content;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public virtual async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await Content.DisposeAsync().ConfigureAwait(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}
