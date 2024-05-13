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
    /// Interface to represent the response returned by the Update method in <see cref="IDataSourcesApiClient"/>.
    /// </summary>
    public interface IUpdateDataSourceResult
    {
        /// <summary>
        /// Gets the unique identifier of the data source.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the name of the data source.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the content URL of the data source.
        /// </summary>
        string? ContentUrl { get; }

        /// <summary>
        /// Gets the type of the data source.
        /// </summary>
        string? Type { get; }

        /// <summary>
        /// Gets the creation date/time of the data source.
        /// </summary>
        DateTime CreatedAtUtc { get; }

        /// <summary>
        /// Gets the update date/time of the data source.
        /// </summary>
        DateTime UpdatedAtUtc { get; }

        /// <summary>
        /// Gets the certification status for the data source.
        /// </summary>
        bool IsCertified { get; }

        /// <summary>
        /// Gets the certification note for the data source.
        /// </summary>
        string? CertificationNote { get; }

        /// <summary>
        /// Gets the encrypt extracts flag for the data source.
        /// </summary>
        bool EncryptExtracts { get; }

        /// <summary>
        /// Gets the project id for the data source.
        /// </summary>
        Guid ProjectId { get; }

        /// <summary>
        /// Gets the owner id of the data source.
        /// </summary>
        Guid OwnerId { get; }

        /// <summary>
        /// Gets the job id for the data source.
        /// </summary>
        Guid? JobId { get; }
    }
}