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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Simulation.Rest.Net.Responses
{
    public class RestPermissionsCreateResponseBuilderTests
    {
        public abstract class RestPermissionsCreateResponseBuilderTest : ResponseBuilderTestBase
        { }

        public class RespondAsync : RestPermissionsCreateResponseBuilderTest
        {
            [Fact]
            public async Task DenyProjectLeaderErrorAsync()
            {
                var data = new TableauData(Create<UsersResponse.UserType>());

                var siteId = data.SignIn!.Site!.Id;
                var proj = data.AddProject(new()
                {
                    Id = Guid.NewGuid()
                });

                var builder = new RestPermissionsCreateResponseBuilder<ProjectsResponse.ProjectType>(
                    data,
                    Serializer,
                    "projects",
                    d => d.Projects);

                var capability = new Capability(new CapabilityType { Name = PermissionsCapabilityNames.ProjectLeader, Mode = PermissionsCapabilityModes.Deny });
                var grantee = new GranteeCapability(GranteeType.User, data.SignIn.User!.Id, new[] { capability });
                var permissions = new Migration.Content.Permissions.Permissions(proj.Id, new[] { grantee });
                var requestContent = new PermissionsAddRequest(permissions);

                var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost/api/1.0/sites/{siteId.ToUrlSegment()}/projects/{proj.Id.ToUrlSegment()}/permissions");
                request.Content = Serializer.Serialize(requestContent, MediaTypes.Xml);
                request.Headers.TryAddWithoutValidation(RestHeaders.AuthenticationToken, data.SignIn.Token);

                var response = await builder.RespondAsync(request, Cancel);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.NotNull(response.Content);

                var responseContent = await Serializer.DeserializeAsync<PermissionsResponse>(response.Content, Cancel);
                Assert.NotNull(responseContent);
                Assert.NotNull(responseContent.Error);
                Assert.Equal("400009", responseContent.Error.Code);
            }
        }
    }
}
