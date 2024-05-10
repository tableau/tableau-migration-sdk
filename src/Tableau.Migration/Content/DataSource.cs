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
    internal class DataSource : ContainerContentBase, IDataSource
    {
        public DataSource(IDataSourceType response, IContentReference project, IContentReference owner)
            : this(
                  response.Id,
                  response.Name,
                  response.ContentUrl,
                  response.Description,
                  response.CreatedAt,
                  response.UpdatedAt,
                  response.EncryptExtracts,
                  response.HasExtracts,
                  response.IsCertified,
                  response.UseRemoteQueryAgent,
                  response.WebpageUrl,
                  response.Tags.ToTagList(t => new Tag(t)),
                  project,
                  owner)
        { }

        public DataSource(IDataSource dataSource)
            : this(
                  dataSource.Id,
                  dataSource.Name,
                  dataSource.ContentUrl,
                  dataSource.Description,
                  dataSource.CreatedAt,
                  dataSource.UpdatedAt,
                  dataSource.EncryptExtracts,
                  dataSource.HasExtracts,
                  dataSource.IsCertified,
                  dataSource.UseRemoteQueryAgent,
                  dataSource.WebpageUrl,
                  dataSource.Tags,
                  ((IContainerContent)dataSource).Container,
                  dataSource.Owner)
        { }

        private DataSource(
            Guid id,
            string? name,
            string? contentUrl,
            string? description,
            string? createdAt,
            string? updatedAt,
            bool encryptExtracts,
            bool hasExtracts,
            bool isCertified,
            bool useRemoteQueryAgent,
            string? webpageUrl,
            IList<ITag> tags,
            IContentReference project,
            IContentReference owner)
            : base(project)
        {
            Id = Guard.AgainstDefaultValue(id, () => id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(name, () => name);
            ContentUrl = Guard.AgainstNullEmptyOrWhiteSpace(contentUrl, () => contentUrl);

            Description = description ?? string.Empty;
            CreatedAt = createdAt ?? string.Empty;
            UpdatedAt = updatedAt ?? string.Empty;

            EncryptExtracts = encryptExtracts;
            HasExtracts = hasExtracts;
            IsCertified = isCertified;
            UseRemoteQueryAgent = useRemoteQueryAgent;

            WebpageUrl = webpageUrl ?? string.Empty;

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
        public bool EncryptExtracts { get; set; }

        /// <inheritdoc/>
        public bool HasExtracts { get; }

        /// <inheritdoc/>
        public bool IsCertified { get; }

        /// <inheritdoc/>
        public bool UseRemoteQueryAgent { get; set; }

        /// <inheritdoc/>
        public string? WebpageUrl { get; }

        // <inheritdoc/>
        public IContentReference Owner { get; set; }

        /// <inheritdoc/>
        public IList<ITag> Tags { get; set; }
    }
}
