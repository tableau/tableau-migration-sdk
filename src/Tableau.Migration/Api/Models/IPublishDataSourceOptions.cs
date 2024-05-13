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

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for API client data source publish options. 
    /// </summary>
    public interface IPublishDataSourceOptions : IPublishFileOptions
    {
        /// <summary>
        /// Gets the name of the data source.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the data source.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets whether or not the data source uses Tableau Bridge.
        /// </summary>
        bool UseRemoteQueryAgent { get; }

        ///<inheritdoc/>
        bool EncryptExtracts { get; }

        /// <summary>
        /// Gets whether or not to overwrite any existing data source.
        /// </summary>
        bool Overwrite { get; }

        /// <summary>
        /// Gets the ID of the project to publish to.
        /// </summary>
        Guid ProjectId { get; }
    }
}
