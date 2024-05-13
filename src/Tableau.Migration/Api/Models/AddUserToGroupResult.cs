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
    internal class AddUserToGroupResult : IAddUserToGroupResult
    {
        /// <inheritdoc/>
        public Guid Id { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string SiteRole { get; }

        public AddUserToGroupResult(AddUserResponse response)
        {
            var user = Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(user.Id, () => response.Item.Id);

            Name = Guard.AgainstNullEmptyOrWhiteSpace(user.Name, () => response.Item.Name);

            SiteRole = Guard.AgainstNullEmptyOrWhiteSpace(user.SiteRole, () => response.Item.SiteRole);
        }
    }
}
