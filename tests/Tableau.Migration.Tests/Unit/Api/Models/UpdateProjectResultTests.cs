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
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class UpdateProjectResultTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var response = new UpdateProjectResponse
                {
                    Item = Create<UpdateProjectResponse.ProjectType>()
                };

                response.Item.ParentProjectId = Guid.NewGuid().ToString();
                response.Item.ControllingPermissionsProjectId = Guid.NewGuid().ToString();
                response.Item.Owner = new() { Id = Guid.NewGuid() };

                var result = new UpdateProjectResult(response);

                Assert.Equal(response.Item.Id, result.Id);
                Assert.Equal(response.Item.GetParentProjectId(), result.ParentProjectId);
                Assert.Equal(response.Item.Name, result.Name);
                Assert.Equal(response.Item.Description, result.Description);
                Assert.Equal(response.Item.ContentPermissions, result.ContentPermissions);
                Assert.Equal(response.Item.GetControllingPermissionsProjectId(), result.ControllingPermissionsProjectId);
                Assert.Equal(response.Item.Owner.Id, result.OwnerId);
            }
        }
    }
}
