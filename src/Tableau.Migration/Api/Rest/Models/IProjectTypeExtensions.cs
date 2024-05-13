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

namespace Tableau.Migration.Api.Rest.Models
{
    internal static class IProjectTypeExtensions
    {
        /// <summary>
        /// Gets the parsed parent project ID.
        /// </summary>
        /// <returns>
        /// The parent project ID, 
        /// or null if <see cref="IProjectType.ParentProjectId"/> is null, empty, or fails to parse.
        /// </returns>
        public static Guid? GetParentProjectId(this IProjectType project)
            => ParseProjectId(project.ParentProjectId);

        /// <summary>
        /// Gets the parsed parent project ID.
        /// </summary>
        /// <returns>
        /// The parent project ID, 
        /// or null if <paramref name="projectId"/> is null, empty, or fails to parse.
        /// </returns>
        public static Guid? ParseProjectId(string? projectId)
            => Guid.TryParse(projectId, out var parsedId) ? parsedId : null;

        /// <summary>
        /// Gets the parsed parent project ID.
        /// </summary>
        /// <returns>
        /// The parent project ID, 
        /// or null if <see cref="IProjectType.ControllingPermissionsProjectId"/> is null, empty, or fails to parse.
        /// </returns>
        public static Guid? GetControllingPermissionsProjectId(this IProjectType project)
            => ParseProjectId(project.ControllingPermissionsProjectId);
    }
}
