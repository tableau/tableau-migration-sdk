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

namespace Tableau.Migration.Api.Models
{
    /// <inheritdoc/>
    public class UpdateConnectionOptions : IUpdateConnectionOptions
    {
        /// <summary>
        /// Creates a new <see cref="UpdateConnectionOptions"/> object.
        /// </summary>
        /// <param name="serverAddress">The server address on the connection.</param>
        /// <param name="serverPort">The server port on the connection.</param>
        /// <param name="connectionUsername">The user name on the connection.</param>
        /// <param name="password">The password on the connection.</param>
        /// <param name="embedPassword">The embed password flag on the connection.</param>
        /// <param name="queryTaggingEnabled">The query tagging enabled flag on the connection.</param>
        public UpdateConnectionOptions(
            string? serverAddress = null,
            string? serverPort = null,
            string? connectionUsername = null,
            string? password = null,
            bool? embedPassword = null,
            bool? queryTaggingEnabled = null)
        {
            ServerAddress = serverAddress;
            ServerPort = serverPort;
            ConnectionUsername = connectionUsername;
            Password = password;
            EmbedPassword = embedPassword;
            QueryTaggingEnabled = queryTaggingEnabled;
        }

        /// <inheritdoc/>
        public string? ServerAddress { get; }

        /// <inheritdoc/>
        public string? ServerPort { get; }

        /// <inheritdoc/>
        public string? ConnectionUsername { get; }

        /// <inheritdoc/>
        public string? Password { get; }

        /// <inheritdoc/>
        public bool? EmbedPassword { get; }

        /// <inheritdoc/>
        public bool? QueryTaggingEnabled { get; }
    }
}
