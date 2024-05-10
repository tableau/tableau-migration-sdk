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
using System.IO.Compression;
using Microsoft.IO;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Interface for an object that can edit content in Tableau file formats 
    /// including TDS, TDSX, TWB, and TWBX.
    /// All changes made to the content is persisted upon disposal
    /// </summary>
    public interface ITableauFileEditor : IAsyncDisposable
    {
        /// <summary>
        /// Gets the memory backed stream
        /// with unencrypted tableau file data 
        /// to write back to the file store upon disposal.
        /// </summary>
        RecyclableMemoryStream Content { get; }

        /// <summary>
        /// Gets the zip archive for the file,
        /// or null if the file is an unzipped XML file (TDS or TWB).
        /// The zip archive is backed by the <see cref="Content"/> stream.
        /// </summary>
        ZipArchive? Archive { get; }

        /// <summary>
        /// Gets the current read/write stream to the XML content of the Tableau file,
        /// opening a new stream if necessary.
        /// Changes to the stream will be automatically saved before publishing.
        /// </summary>
        /// <returns>The XML stream to edit.</returns>
        ITableauFileXmlStream GetXmlStream();
    }
}
