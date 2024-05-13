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
    /// Interface for a data source REST response.
    /// </summary>
    public interface IDataSourceType : IRestIdentifiable, INamedContent, IWithProjectType, IWithOwnerType, IWithTagTypes
    {
        /// <summary>
        /// Gets the description for the response.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets the content URL for the response.
        /// </summary>
        string? ContentUrl { get; }

        /// <summary>
        /// Gets the created timestamp for the response.
        /// </summary>
        string? CreatedAt { get; }

        /// <summary>
        /// Gets the updated timestamp for the response.
        /// </summary>
        string? UpdatedAt { get; }

        /// <summary>
        /// Gets the encrypted extracts flag for the response.
        /// </summary>
        bool EncryptExtracts { get; }

        /// <summary>
        /// Gets whether or not the data source has extracts for the response.
        /// </summary>
        bool HasExtracts { get; }

        /// <summary>
        /// Gets whether or not the data source is certified for the response.
        /// </summary>
        bool IsCertified { get; }

        /// <summary>
        /// Gets whether or not the data source uses a remote query agent for the response.
        /// </summary>
        bool UseRemoteQueryAgent { get; }

        /// <summary>
        /// Gets the data source webpage URL for the response.
        /// </summary>
        string? WebpageUrl { get; }
    }
}
