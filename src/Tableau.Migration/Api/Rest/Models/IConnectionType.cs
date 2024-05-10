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

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for a connection REST response.
    /// </summary>
    public interface IConnectionType : IRestIdentifiable
    {
        /// <summary>
        /// The connection type for the response.
        /// </summary>
        public string? Type { get; }

        /// <summary>
        /// The server address for the response.
        /// </summary>
        public string? ServerAddress { get; }

        /// <summary>
        /// The server port for the response.
        /// </summary>
        public string? ServerPort { get; }

        /// <summary>
        /// The connection username for the response.
        /// </summary>
        public string? ConnectionUsername { get; }

        /// <summary>
        /// The query tagging enabled flag for the response. 
        /// This is returned only for administrator users.
        /// </summary>
        public string? QueryTaggingEnabled { get; }
    }
}
