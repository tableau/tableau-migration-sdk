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
using System.Collections.Generic;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    internal class Flow : ContainerContentBase, IFlow
    {
        public Flow(IFlowType response, IContentReference project, IContentReference owner)
            : this(
                  response.Id,
                  response.Name,
                  response.Description,
                  response.CreatedAt,
                  response.UpdatedAt,
                  response.WebpageUrl,
                  response.FileType,
                  response.Tags.ToTagList(t => new Tag(t)),
                  project,
                  owner)
        { }

        public Flow(IFlow flow)
            : this(
                  flow.Id,
                  flow.Name,
                  flow.Description,
                  flow.CreatedAt,
                  flow.UpdatedAt,
                  flow.WebpageUrl,
                  flow.FileType,
                  flow.Tags,
                  ((IContainerContent)flow).Container,
                  flow.Owner)
        { }

        private Flow(
            Guid id,
            string? name,
            string? description,
            string? createdAt,
            string? updatedAt,
            string? webpageUrl,
            string? fileType,
            IList<ITag> tags,
            IContentReference project,
            IContentReference owner)
            : base(project)
        {
            Id = Guard.AgainstDefaultValue(id, () => id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(name, () => name);

            Description = description ?? string.Empty;
            CreatedAt = createdAt ?? string.Empty;
            UpdatedAt = updatedAt ?? string.Empty;

            WebpageUrl = webpageUrl ?? string.Empty;
            FileType = Guard.AgainstNullEmptyOrWhiteSpace(fileType, () => fileType);

            Owner = owner;
            Tags = tags;

            Location = project.Location.Append(Name);
        }

        /// <inheritdoc/>
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string CreatedAt { get; }

        /// <inheritdoc/>
        public string? UpdatedAt { get; }

        /// <inheritdoc/>
        public string? WebpageUrl { get; }

        /// <inheritdoc/>
        public string FileType { get; set; }

        // <inheritdoc/>
        public IContentReference Owner { get; set; }

        /// <inheritdoc/>
        public IList<ITag> Tags { get; set; }
    }
}
