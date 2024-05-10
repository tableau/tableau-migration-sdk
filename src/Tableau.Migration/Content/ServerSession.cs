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

using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Content
{
    internal sealed class ServerSession : IServerSession
    {
        /// <inheritdoc />
        public IContentReference Site { get; }

        /// <inheritdoc />
        public ISiteSettings? Settings { get; }

        /// <inheritdoc />
        public bool IsAdministrator { get; }

        public ServerSession(ServerSessionResponse response)
        {
            var session = Guard.AgainstNull(response.Item, () => response.Item);

            var site = Guard.AgainstNull(response.Item.Site, () => response.Item.Site);

            Site = new ContentReferenceStub(Guard.AgainstDefaultValue(site.Id, () => response.Item.Site.Id), 
                Guard.AgainstNull(site.ContentUrl, () => response.Item.Site.ContentUrl),
                new(Guard.AgainstNullEmptyOrWhiteSpace(site.Name, () => response.Item.Site.Name)));

            var user = Guard.AgainstNull(response.Item.User, () => response.Item.User);
            var siteRole = Guard.AgainstNullEmptyOrWhiteSpace(response.Item.User.SiteRole, () => response.Item.User.SiteRole);
            var adminLevel = SiteRoleMapping.GetAdministratorLevel(siteRole);
            IsAdministrator = !AdministratorLevels.IsAMatch(adminLevel, AdministratorLevels.None);

            if(IsAdministrator)
            {
                Settings = new ServerSessionSettings(response.Item.Site);
            }
        }
    }
}
