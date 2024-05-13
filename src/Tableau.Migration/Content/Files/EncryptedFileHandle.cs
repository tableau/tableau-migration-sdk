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
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// <see cref="ContentFileHandle"/> that is potentially stored in encrypted form.
    /// </summary>
    /// <param name="Store"><inheritdoc /></param>
    /// <param name="Path"><inheritdoc /></param>
    /// <param name="OriginalFileName"><inheritdoc /></param>
    /// <param name="Inner">The file handle to the inner file store.</param>
    public record EncryptedFileHandle(IContentFileStore Store, string Path, string OriginalFileName, IContentFileHandle Inner)
        : ContentFileHandle(Store, Path, OriginalFileName)
    {
        /// <summary>
        /// Creates a new <see cref="EncryptedFileHandle"/> object.
        /// </summary>
        /// <param name="store">The file store the handle is for.</param>
        /// <param name="inner">The file handle to the inner file store.</param>
        public EncryptedFileHandle(IContentFileStore store, IContentFileHandle inner)
            : this(store, inner.Path, inner.OriginalFileName, inner)
        { }

        /// <inheritdoc />
        public override async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await Inner.DisposeAsync().ConfigureAwait(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}
