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

using System.IO;
using System.IO.Abstractions;
using Tableau.Migration.Config;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// <see cref="IContentFileStore"/> implementation that stores
    /// files in a random temporary directory.
    /// </summary>
    public class TemporaryDirectoryContentFileStore
        : DirectoryContentFileStore
    {
        /// <summary>
        /// Creates a new <see cref="TemporaryDirectoryContentFileStore"/> object.
        /// </summary>
        /// <param name="fileSystem">The file system to use.</param>
        /// <param name="pathResolver">The path resolver to use.</param>
        /// <param name="configReader">A config reader to get the root path and other options from.</param>
        /// <param name="memoryStreamManager">The memory stream manager to user.</param>
        public TemporaryDirectoryContentFileStore(
            IFileSystem fileSystem,
            IContentFilePathResolver pathResolver,
            IConfigReader configReader,
            IMemoryStreamManager memoryStreamManager)
            : base(fileSystem, pathResolver, configReader, memoryStreamManager, Path.GetRandomFileName())
        { }
    }
}
