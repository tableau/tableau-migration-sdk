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

using System.IO.Abstractions;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Files;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Represents a content file store that stores files in a per-migration sub directory.
    /// </summary>
    public class MigrationDirectoryContentFileStore : DirectoryContentFileStore
    {
        /// <summary>
        /// Creates a new <see cref="MigrationDirectoryContentFileStore"/> object.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="pathResolver">The path resolver.</param>
        /// <param name="configReader">The configuration reader.</param>
        /// <param name="memoryStreamManager">The memory stream manager to user.</param>
        /// <param name="migrationInput">The migration input to get the migration ID from.</param>
        public MigrationDirectoryContentFileStore(
            IFileSystem fileSystem,
            IContentFilePathResolver pathResolver,
            IConfigReader configReader,
            IMemoryStreamManager memoryStreamManager,
            IMigrationInput migrationInput)
            : base(fileSystem, pathResolver, configReader, memoryStreamManager, $"migration-{migrationInput.MigrationId:N}")
        { }
    }
}
