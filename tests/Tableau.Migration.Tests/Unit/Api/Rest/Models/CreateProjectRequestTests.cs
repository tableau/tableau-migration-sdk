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
using AutoFixture;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class CreateProjectRequestTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Serializes()
            {
                var request = AutoFixture.Create<CreateProjectRequest>();

                Assert.NotNull(request.Project);

                request.Project.ParentProjectId = Guid.NewGuid().ToString();

                var serialized = Serializer.SerializeToXml(request);

                Assert.NotNull(serialized);

                var expected = $@"
<tsRequest>
    <project 
        parentProjectId=""{request.Project.ParentProjectId}"" 
        name=""{request.Project.Name}"" 
        description=""{request.Project.Description}"" 
        contentPermissions=""{request.Project.ContentPermissions}"" />
</tsRequest>";

                AssertXmlEqual(expected, serialized);
            }
        }

        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes_with_values()
            {
                var parentProjectId = Create<Guid>();
                var name = Create<string>();
                var description = Create<string>();
                var contentPermissions = Create<string>();

                var request = new CreateProjectRequest(parentProjectId, name, description, contentPermissions);

                Assert.NotNull(request.Project);

                Assert.Equal(parentProjectId, Guid.Parse(request.Project.ParentProjectId!));
                Assert.Equal(name, request.Project.Name);
                Assert.Equal(description, request.Project.Description);
                Assert.Equal(contentPermissions, request.Project.ContentPermissions);
            }

            [Fact]
            public void Handles_null_optional_values()
            {
                var name = Create<string>();

                var request = new CreateProjectRequest(null, name, null, null);

                Assert.NotNull(request.Project);

                Assert.Null(request.Project.ParentProjectId);
                Assert.Equal(name, request.Project.Name);
                Assert.Null(request.Project.Description);
                Assert.Null(request.Project.ContentPermissions);
            }

            [Fact]
            public void Initializes_with_options()
            {
                var options = Create<ICreateProjectOptions>();

                var request = new CreateProjectRequest(options);

                Assert.NotNull(request.Project);

                Assert.Equal(options.ParentProject?.Id, Guid.Parse(request.Project.ParentProjectId!));
                Assert.Equal(options.Name, request.Project.Name);
                Assert.Equal(options.Description, request.Project.Description);
                Assert.Equal(options.ContentPermissions, request.Project.ContentPermissions);
            }
        }
    }
}