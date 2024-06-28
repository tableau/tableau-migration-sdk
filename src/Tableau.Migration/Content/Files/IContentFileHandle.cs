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
    public interface IContentFileHandle : IAsyncDisposable
    {
        /// <summary>
        /// Gets the original filename of the file, used for the upload filename when publishing the content item.
        /// </summary>
        string OriginalFileName { get; }

        /// <summary>
        /// Gets the path to the file.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the file store the handle is for.
        /// </summary>
        IContentFileStore Store { get; }

        /// <summary>
        /// Opens a stream to read from a file.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The stream to read from.</returns>
        Task<IContentFileStream> OpenReadAsync(CancellationToken cancel);

        /// <summary>
        /// Opens a stream to write to a file.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The stream to write to.</returns>
        Task<IContentFileStream> OpenWriteAsync(CancellationToken cancel);

        /// <summary>
        /// Gets the current read/write stream to the XML content of the Tableau file, 
        /// opening a new stream if necessary.
        /// Changes to the stream will be automatically saved before publishing.
        /// </summary>
        /// <returns>The XML stream to edit.</returns>
        Task<ITableauFileXmlStream> GetXmlStreamAsync(CancellationToken cancel);
    }
}