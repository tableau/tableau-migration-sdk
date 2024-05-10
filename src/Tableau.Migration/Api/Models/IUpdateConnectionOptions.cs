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
    /// <summary>
    /// Interface for update connection options. 
    /// Any combination if these properties are valid. 
    /// If none of the options are provided, no update is made.
    /// </summary>
    public interface IUpdateConnectionOptions
    {
        /// <summary>
        /// The ServerAddress.
        /// </summary>
        public string? ServerAddress { get; }

        /// <summary>
        /// The server port on the connection.
        /// </summary>
        string? ServerPort { get; }

        /// <summary>
        /// The user name on the connection.
        /// </summary>
        public string? ConnectionUsername { get; }

        /// <summary>
        /// The connection password.
        /// </summary>
        string? Password { get; }

        /// <summary>
        /// The embed password flag on the connection.
        /// </summary>
        public bool? EmbedPassword { get; }

        /// <summary>
        /// The query tagging enabled flag on the connection.
        /// </summary>
        public bool? QueryTaggingEnabled { get; }
    }
}
