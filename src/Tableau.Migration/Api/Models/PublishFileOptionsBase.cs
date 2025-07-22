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

using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Abstract base class for <see cref="IPublishFileOptions"/> implementations.
    /// </summary>
    public abstract class PublishFileOptionsBase : IPublishFileOptions
    {
        /// <summary>
        /// Creates a new <see cref="PublishFileOptionsBase"/> object.
        /// </summary>
        /// <param name="file">The file content to publish.</param>
        /// <param name="fileType">The type of the file to publish.</param>
        public PublishFileOptionsBase(IContentFileHandle file, string fileType)
            : this(file, file.OriginalFileName, fileType)
        { }

        /// <summary>
        /// Creates a new <see cref="PublishFileOptionsBase"/> object.
        /// </summary>
        /// <param name="file">The file content to publish.</param>
        /// <param name="fileName">The name of the file to publish.</param>
        /// <param name="fileType">The type of the file to publish.</param>
        public PublishFileOptionsBase(IContentFileHandle file, string fileName, string fileType)
        {
            File = file;
            FileName = fileName;
            FileType = fileType;
        }

        /// <inheritdoc/>
        public IContentFileHandle File { get; set; }

        /// <inheritdoc/>
        public string FileName { get; set; }

        /// <inheritdoc/>
        public string FileType { get; set; }
    }
}