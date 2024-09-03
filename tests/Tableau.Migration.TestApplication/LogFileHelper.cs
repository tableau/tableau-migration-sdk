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
using Tableau.Migration.TestApplication.Config;

namespace Tableau.Migration.TestApplication
{
    internal static class LogFileHelper
    {
        private const string FILENAME_TIMESTAMP_FORMAT = "yyyy-MM-dd-HH-mm-ss";

        private const string MANIFEST_FILE_PREFIX = "Manifest";

        private const string LOG_FILE_PREFIX = "Tableau.Migration.TestApplication";

        internal const string LOG_LINE_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff}|{Level}|{ThreadId}|{SourceContext} -\t{Message:lj}{NewLine}{Exception}";

        public static string GetFileNameTimeStamp()
            => Program.StartTime.ToString(FILENAME_TIMESTAMP_FORMAT);

        public static string GetManifestFilePath(LogOptions logOptions)
            => Path.Join(
                string.IsNullOrEmpty(logOptions.ManifestFolderPath) ? logOptions.FolderPath : logOptions.ManifestFolderPath,
                $@"{MANIFEST_FILE_PREFIX}-{GetFileNameTimeStamp()}.json");

        public static string GetLogFilePath(string? logFolderPath)
            => string.IsNullOrEmpty(logFolderPath)
                ? throw new Exception($"The config value for {nameof(LogOptions)}.{nameof(LogOptions.FolderPath)} is null or empty.")
                : Path.Join(logFolderPath, @$"{LOG_FILE_PREFIX}-{GetFileNameTimeStamp()}.log");

    }
}
