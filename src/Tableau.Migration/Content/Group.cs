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

using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Content
{
    internal class Group : UsernameContentBase, IGroup
    {
        public string? GrantLicenseMode { get; set; }

        public string? SiteRole { get; set; }

        public Group(GroupsResponse.GroupType response)
        {
            var domain = Guard.AgainstNull(response.Domain, () => response.Domain);

            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);

            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Name, () => response.Name);
            Domain = Guard.AgainstNullEmptyOrWhiteSpace(domain.Name, () => response.Domain.Name);

            GrantLicenseMode = response?.Import?.GrantLicenseMode;
            SiteRole = response?.Import?.SiteRole;
        }

        public Group(CreateGroupResponse response)
        {
            var group = Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(group.Id, () => response.Item.Id);

            Name = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.Name, () => response.Item.Name);

            if (response.Item.Import is null)
            {
                Domain = Constants.LocalDomain;
            }
            else
            {
                Domain = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.Import.DomainName, () => response.Item.Import.DomainName);
            }
        }

        protected Group(IGroup group)
        {
            Id = group.Id;
            Name = group.Name;
            Domain = group.Domain;
            GrantLicenseMode = group.GrantLicenseMode;
            SiteRole = group.SiteRole;
        }
    }
}
