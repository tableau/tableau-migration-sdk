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
    /// Class for API client project creation options. 
    /// </summary>
    public class CreateProjectOptions : ICreateProjectOptions
    {
        /// <inheritdoc/>
        public IContentReference? ParentProject { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string? Description { get; }

        /// <inheritdoc/>
        public string? ContentPermissions { get; }

        /// <inheritdoc/>
        public bool PublishSamples { get; }

        /// <summary>
        /// Creates a new <see cref="CreateProjectOptions"/> instance.
        /// </summary>
        /// <param name="parentProject">The parent project if applicable.</param>
        /// <param name="name">The name of the project.</param>
        /// <param name="description">The description of the project.</param>
        /// <param name="contentPermissions">The content permissions for the project.</param>
        /// <param name="publishSamples">True to publish sample content, false otherwise.</param>
        public CreateProjectOptions(
            IContentReference? parentProject,
            string name,
            string? description,
            string? contentPermissions,
            bool publishSamples)
        {
            if (parentProject is not null)
            {
                Guard.AgainstDefaultValue(parentProject.Id, () => parentProject.Id);
                Guard.AgainstDefaultValue(parentProject.Location, () => parentProject.Location);
                ParentProject = parentProject;
            }

            Name = Guard.AgainstNullEmptyOrWhiteSpace(name, nameof(name));
            Description = description;
            ContentPermissions = contentPermissions;
            PublishSamples = publishSamples;
        }
    }
}
