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

namespace Tableau.Migration.Config
{
    /// <summary>
    /// Options related to file storage.
    /// </summary>
    public class FileOptions
    {
        /// <summary>
        /// Defaults for file options.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// The default disable file encryption flag.
            /// </summary>
            public const bool DISABLE_FILE_ENCRYPTION = false;

            /// <summary>
            /// The default root path of the file store.
            /// </summary>
            public readonly static string ROOT_PATH = Path.GetTempPath();
        }

        /// <summary>
        /// Gets or sets whether or not to disable file encryption.
        /// Do not disable file encryption when migrating production content.
        /// </summary>
        public bool DisableFileEncryption
        {
            get => _disableFileEncryption ?? Defaults.DISABLE_FILE_ENCRYPTION;
            set => _disableFileEncryption = value;
        }
        private bool? _disableFileEncryption;

        /// <summary>
        /// Gets or sets the file store root path.
        /// </summary>
        public string RootPath
        {
            get => _rootPath ?? Defaults.ROOT_PATH;
            set => _rootPath = value;
        }
        private string? _rootPath;
    }
}
