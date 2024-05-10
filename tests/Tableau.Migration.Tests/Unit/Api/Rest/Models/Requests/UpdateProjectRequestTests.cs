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
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    public class UpdateProjectRequestTests
    {
        public class Ctor
        {
            [Fact]
            public void Initializes()
            {
                var parentId = Guid.NewGuid();
                var controllingPermissionsId = Guid.NewGuid();
                var ownerId = Guid.NewGuid();
                var request = new UpdateProjectRequest("name", "desc", parentId, "contentPerms", controllingPermissionsId, ownerId);

                Assert.NotNull(request.Project);
                Assert.Equal("name", request.Project.Name);
                Assert.Equal("desc", request.Project.Description);
                Assert.Equal(parentId.ToString(), request.Project.ParentProjectId);
                Assert.Equal("contentPerms", request.Project.ContentPermissions);
                Assert.Equal(controllingPermissionsId.ToString(), request.Project.ControllingPermissionsProjectId);
                Assert.Equal(ownerId, request.Project.Owner!.Id);
            }

            [Fact]
            public void ClearParentProject()
            {
                var request = new UpdateProjectRequest(newParentProjectId: Guid.Empty);

                Assert.NotNull(request.Project);
                Assert.Equal(string.Empty, request.Project.ParentProjectId);
            }
        }
    }
}
