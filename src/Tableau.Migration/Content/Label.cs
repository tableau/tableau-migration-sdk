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

namespace Tableau.Migration.Content
{
    ///<inheritdoc/>
    internal class Label : ILabel
    {
        public Label(LabelsResponse.LabelType response)
        {
            Guard.AgainstNull(response, () => nameof(response));
            Id = Guard.AgainstDefaultValue(response.Id, () => nameof(response.Id));

            var site = Guard.AgainstNull(response.Site, () => nameof(response.Site));
            SiteId = Guard.AgainstDefaultValue(site.Id, () => nameof(response.Site.Id));

            var owner = Guard.AgainstNull(response.Owner, () => nameof(response.Owner));
            SiteId = Guard.AgainstDefaultValue(owner.Id, () => nameof(response.Owner.Id));

            UserDisplayName = Guard.AgainstNull(response.UserDisplayName, () => nameof(response.UserDisplayName));

            ContentId = Guard.AgainstDefaultValue(response.ContentId, () => nameof(response.ContentId));

            ContentType = Guard.AgainstNull(response.ContentType, () => nameof(response.ContentType));
            Message = response.Message;
            Value = Guard.AgainstNull(response.Value, () => nameof(response.Value));
            Category = Guard.AgainstNull(response.Category, () => nameof(response.Category));
            Active = response.Active;
            Elevated = response.Elevated;
            CreatedAt = response.CreatedAt;
            UpdatedAt = response.UpdatedAt;
        }

        ///<inheritdoc/>
        public Guid Id { get; set; }

        ///<inheritdoc/>
        public Guid SiteId { get; set; }

        ///<inheritdoc/>
        public Guid OwnerId { get; set; }

        ///<inheritdoc/>
        public string UserDisplayName { get; set; }

        ///<inheritdoc/>
        public Guid ContentId { get; set; }

        ///<inheritdoc/>
        public string ContentType { get; set; }

        ///<inheritdoc/>
        public string? Message { get; set; }

        ///<inheritdoc/>
        public string Value { get; set; }

        ///<inheritdoc/>
        public string Category { get; set; }

        ///<inheritdoc/>
        public bool Active { get; set; }

        ///<inheritdoc/>
        public bool Elevated { get; set; }

        ///<inheritdoc/>
        public string? CreatedAt { get; set; }

        ///<inheritdoc/>
        public string? UpdatedAt { get; set; }
    }
}
