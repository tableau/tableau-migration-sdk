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
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    internal sealed class UpdateDataSourceResult : IUpdateDataSourceResult
    {
        /// <inheritdoc/>
        public Guid Id { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string? ContentUrl { get; }

        /// <inheritdoc/>
        public string? Type { get; }

        /// <inheritdoc/>
        public DateTime CreatedAtUtc { get; }

        /// <inheritdoc/>
        public DateTime UpdatedAtUtc { get; }

        /// <inheritdoc/>
        public bool IsCertified { get; }

        /// <inheritdoc/>
        public string? CertificationNote { get; }

        /// <inheritdoc/>
        public bool EncryptExtracts { get; }

        /// <inheritdoc/>
        public Guid ProjectId { get; }

        /// <inheritdoc/>
        public Guid OwnerId { get; }

        /// <inheritdoc/>
        public Guid? JobId { get; }

        /// <summary>
        /// Creates a new <see cref="UpdateDataSourceResult"/> instance.
        /// </summary>
        /// <param name="response">The REST API response.</param>
        public UpdateDataSourceResult(UpdateDataSourceResponse response)
        {
            Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(response.Item.Id, () => response.Item.Id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.Name, () => response.Item.Name);
            ContentUrl = response.Item.ContentUrl;
            Type = response.Item.Type;
            CreatedAtUtc = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.CreatedAt, () => response.Item.CreatedAt).ParseFromIso8601();
            UpdatedAtUtc = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.UpdatedAt, () => response.Item.UpdatedAt).ParseFromIso8601();
            IsCertified = response.Item.IsCertified;
            CertificationNote = response.Item.CertificationNote;
            EncryptExtracts = response.Item.EncryptExtracts;

            Guard.AgainstNull(response.Item.Project, () => response.Item.Project);
            ProjectId = Guard.AgainstDefaultValue(response.Item.Project.Id, () => response.Item.Project.Id);

            Guard.AgainstNull(response.Item.Owner, () => response.Item.Owner);
            OwnerId = Guard.AgainstDefaultValue(response.Item.Owner.Id, () => response.Item.Owner.Id);

            if (response.Item.Job is not null)
            {
                JobId = Guard.AgainstDefaultValue(response.Item.Job.Id, () => response.Item.Job.Id);
            }
        }
    }
}
