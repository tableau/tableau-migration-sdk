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
    /// Interface to represent the response returned by the Update method in <see cref="IProjectsApiClient"/>.
    /// </summary>
    public interface IUpdateProjectResult
    {
        /// <summary>
        /// Gets the unique identifier of the project.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the unique identifier of the parent of the project, 
        /// or null if the project is a top-level project.
        /// </summary>
        Guid? ParentProjectId { get; }

        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the project.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets the content permissions mode for the project.
        /// </summary>
        string? ContentPermissions { get; }

        /// <summary>
        /// Gets the unique identifier of the controlling permissions project.
        /// </summary>
        Guid? ControllingPermissionsProjectId { get; }

        /// <summary>
        /// Gets the owner id of the project.
        /// </summary>
        Guid OwnerId { get; }
    }
}
