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
    /// Interface for a project REST response.
    /// </summary>
    public interface IProjectType : IRestIdentifiable, INamedContent, IWithOwnerType
    {
        /// <summary>
        /// Gets the content permissions for the project.
        /// </summary>
        string? ContentPermissions { get; }

        /// <summary>
        /// Gets the description for the project.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets the parent project ID for the project.
        /// </summary>
        string? ParentProjectId { get; }

        /// <summary>
        /// Gets the controlling permissions project ID for the project.
        /// </summary>
        string? ControllingPermissionsProjectId { get; }
    }
}