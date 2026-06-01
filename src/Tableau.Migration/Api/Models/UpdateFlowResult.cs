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
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Models
{
    internal sealed class UpdateFlowResult : IUpdateFlowResult
    {
        /// <inheritdoc/>
        public Guid Id { get; }

        /// <inheritdoc/>
        public string? Name { get; }

        /// <inheritdoc/>
        public string? Description { get; }

        /// <inheritdoc/>
        public string? WebpageUrl { get; }

        /// <inheritdoc/>
        public string? FileType { get; }

        /// <inheritdoc/>
        public string? CreatedAt { get; }

        /// <inheritdoc/>
        public string? UpdatedAt { get; }

        /// <inheritdoc/>
        public Guid? ProjectId { get; }

        /// <inheritdoc/>
        public string? ProjectName { get; }

        /// <inheritdoc/>
        public Guid? OwnerId { get; }

        /// <summary>
        /// Creates a new <see cref="UpdateFlowResult"/> instance.
        /// </summary>
        /// <param name="response">The REST API response.</param>
        public UpdateFlowResult(UpdateFlowResponse response)
        {
            Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(response.Item.Id, () => response.Item.Id);
            Name = response.Item.Name;
            Description = response.Item.Description;
            WebpageUrl = response.Item.WebpageUrl;
            FileType = response.Item.FileType;
            CreatedAt = response.Item.CreatedAt;
            UpdatedAt = response.Item.UpdatedAt;
            ProjectId = response.Item.Project?.Id;
            ProjectName = response.Item.Project?.Name;
            OwnerId = response.Item.Owner?.Id;
        }
    }
}

