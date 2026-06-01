//
//  Copyright (c) 2025, Salesforce, Inc.
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
    /// Interface to represent the response returned by the Update method in <see cref="IFlowsApiClient"/>.
    /// </summary>
    public interface IUpdateFlowResult
    {
        /// <summary>
        /// Gets the unique identifier of the flow.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the name of the flow.
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Gets the description of the flow.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets the webpage URL of the flow.
        /// </summary>
        string? WebpageUrl { get; }

        /// <summary>
        /// Gets the file type of the flow.
        /// </summary>
        string? FileType { get; }

        /// <summary>
        /// Gets the creation date/time of the flow.
        /// </summary>
        string? CreatedAt { get; }

        /// <summary>
        /// Gets the update date/time of the flow.
        /// </summary>
        string? UpdatedAt { get; }

        /// <summary>
        /// Gets the project ID of the flow.
        /// </summary>
        Guid? ProjectId { get; }

        /// <summary>
        /// Gets the project name of the flow.
        /// </summary>
        string? ProjectName { get; }

        /// <summary>
        /// Gets the owner ID of the flow.
        /// </summary>
        Guid? OwnerId { get; }
    }
}

