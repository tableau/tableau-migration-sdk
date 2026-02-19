//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Tests.Content.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public abstract class PermissionsContentApiClientTestBase<TApiClient, TContent> : ApiClientTestBase<TApiClient, TContent>
        where TApiClient : IContentApiClient, IPermissionsContentApiClient
        where TContent : IRestIdentifiable, INamedContent, new()
    {
        protected abstract ICollection<TContent> GetContentData();

        private void AddGranteesToTestData(IPermissions permissions)
        {
            foreach(var capability in permissions.GranteeCapabilities)
            {
                switch(capability.GranteeType)
                {
                    case GranteeType.User:
                        var user = Create<UsersResponse.UserType>();
                        user.Id = capability.Grantee.Id;
                        Api.Data.AddUser(user);
                        break;
                    case GranteeType.Group:
                        var group = Create<GroupsResponse.GroupType>();
                        group.Id = capability.Grantee.Id;
                        Api.Data.AddGroup(group);
                        break;
                    case GranteeType.GroupSet:
                        var groupSet = Create<GroupSetsResponse.GroupSetType>();
                        groupSet.Id = capability.Grantee.Id;
                        Api.Data.AddGroupSet(groupSet);
                        break;
                    default:
                        throw new NotSupportedException($"Grantee type {capability.GranteeType} is not supported.");
                }
            }
        }

        [Fact]
        public async Task GetPermissionsAsync()
        {
            // Arrange 
            await using var sitesClient = await GetSitesClientAsync(Cancel);

            var permissionsClient = GetApiClient();

            var permissions = Create<PermissionsType>();
            var contentItem = Api.Data.AddContentTypePermissions(UrlPrefix, GetContentData, CreateContentItem, permissions);

            var testCapabilities = permissions.GranteeCapabilities!
               .Select(g => new GranteeCapability(new ContentReferenceStub(g.GranteeId, Create<string>(), Create<ContentLocation>()), g))
               .Cast<IGranteeCapability>()
               .ToList();
            var testPermissions = new Permissions(contentItem.Id, testCapabilities);

            AddGranteesToTestData(testPermissions);

            // Act
            var result = await permissionsClient.Permissions.GetPermissionsAsync(contentItem.Id, Cancel);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.True(IPermissionsComparer.Instance.Equals(testPermissions, result.Value));
        }

        [Fact]
        public async Task CreatePermissionsAsync()
        {
            // Arrange 
            await using var sitesClient = await GetSitesClientAsync(Cancel);

            var permissionsClient = GetApiClient();

            var content = CreateContentItem();

            GetContentData().Add(content);

            var permissions = Create<Permissions>();
            permissions.ParentId = content.Id;

            AddGranteesToTestData(permissions);

            // Act
            var result = await permissionsClient.Permissions.CreatePermissionsAsync(content.Id, permissions, Cancel);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.True(IPermissionsComparer.Instance.Equals(permissions, result.Value));
        }
    }
}
