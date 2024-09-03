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

namespace Tableau.Migration.Config
{
    /// <summary>
    /// Network options related to Tableau connections.
    /// </summary>
    public class NetworkOptions
    {
        /// <summary>
        /// Defaults for network settings.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// The default file chunk size in Kilobytes.
            /// </summary>
            public const int FILE_CHUNK_SIZE_KB = 65536;

            /// <summary>
            /// The default Network Headers Logging Flag - Disabled as Default.
            /// </summary>
            public const bool LOG_HEADERS_ENABLED = false;

            /// <summary>
            /// The default Network Content Logging Flag - Disabled as Default.
            /// </summary>
            public const bool LOG_CONTENT_ENABLED = false;

            /// <summary>
            /// The default Network Binary Content Logging Flag - Disabled as Default.
            /// </summary>
            public const bool LOG_BINARY_CONTENT_ENABLED = false;

            /// <summary>
            /// The default Network Exceptions Logging Flag - Disabled as Default.
            /// </summary>
            public const bool LOG_EXCEPTIONS_ENABLED = false;

            /// <summary>
            /// The default omitted user agent comment.
            /// </summary>
            public const string? USER_AGENT_COMMENT = null;
        }

        /// <summary>
        /// Maximum file chunk size in Kilobytes.
        /// </summary>
        public int FileChunkSizeKB
        {
            get => _fileChunkSizeKB ?? Defaults.FILE_CHUNK_SIZE_KB;
            set => _fileChunkSizeKB = value;
        }
        private int? _fileChunkSizeKB;

        /// <summary>
        /// Indicates whether the SDK logs request/response headers. The default value is disabled.
        /// </summary>
        public bool HeadersLoggingEnabled
        {
            get => _headersLoggingEnabled ?? Defaults.LOG_HEADERS_ENABLED;
            set => _headersLoggingEnabled = value;
        }
        private bool? _headersLoggingEnabled;

        /// <summary>
        /// Indicates whether the SDK logs request/response content. The default value is disabled.
        /// </summary>
        public bool ContentLoggingEnabled
        {
            get => _contentLoggingEnabled ?? Defaults.LOG_CONTENT_ENABLED;
            set => _contentLoggingEnabled = value;
        }
        private bool? _contentLoggingEnabled;

        /// <summary>
        /// Indicates whether the SDK logs request/response binary (not textual) content. The default value is disabled.
        /// </summary>
        public bool BinaryContentLoggingEnabled
        {
            get => _binaryContentLoggingEnabled ?? Defaults.LOG_BINARY_CONTENT_ENABLED;
            set => _binaryContentLoggingEnabled = value;
        }
        private bool? _binaryContentLoggingEnabled;

        /// <summary>
        /// Indicates whether the SDK logs network exceptions. The default value is disabled.
        /// </summary>
        public bool ExceptionsLoggingEnabled
        {
            get => _exceptionsLoggingEnabled ?? Defaults.LOG_EXCEPTIONS_ENABLED;
            set => _exceptionsLoggingEnabled = value;
        }
        private bool? _exceptionsLoggingEnabled;

        /// <summary>
        /// Gets or sets the comment to include in the HTTP user agent header, or null to omit the user agent comment.
        /// </summary>
        public string? UserAgentComment
        {
            get => _userAgentComment ?? Defaults.USER_AGENT_COMMENT;
            set => _userAgentComment = value;
        }
        private string? _userAgentComment;

        /// <summary>
        /// Resilience options related to Tableau connections. 
        /// This configuration adds a transient-fault-handling layer for all communication to Tableau.
        /// </summary>
        public ResilienceOptions Resilience { get; set; } = new();
    }
}