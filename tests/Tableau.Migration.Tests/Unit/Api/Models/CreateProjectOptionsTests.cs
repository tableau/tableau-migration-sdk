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
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class CreateProjectOptionsTests
    {
        public class CreateProjectOptionsTest : AutoFixtureTestBase
        { }

        public class Ctor : CreateProjectOptionsTest
        {
            [Fact]
            public void Sets_values()
            {
                var parentProject = Create<IContentReference?>();
                var name = Create<string>();
                var description = Create<string?>();
                var contentPermissions = Create<string?>();
                var publishSamples = Create<bool>();

                var options = new CreateProjectOptions(parentProject, name, description, contentPermissions, publishSamples);

                Assert.Same(parentProject, options.ParentProject);
                Assert.Equal(name, options.Name);
                Assert.Equal(description, options.Description);
                Assert.Equal(contentPermissions, options.ContentPermissions);
                Assert.Equal(publishSamples, options.PublishSamples);
            }

            [Fact]
            public void Handles_null_parent_project_ID()
            {
                var options = new CreateProjectOptions(null, Create<string>(), Create<string?>(), Create<string?>(), Create<bool>());

                Assert.Null(options.ParentProject);
            }

            [Fact]
            public void Throws_when_parent_project_ID_default_value()
            {
                var parentProject = new ContentReferenceStub(Guid.Empty, string.Empty, Create<ContentLocation>());

                var exception = Assert.Throws<ArgumentException>(() => new CreateProjectOptions(parentProject, Create<string>(), Create<string?>(), Create<string?>(), Create<bool>()));

                Assert.Equal("parentProject.Id", exception.ParamName);
            }

            [Fact]
            public void Throws_when_parent_project_location_default_value()
            {
                var parentProject = new ContentReferenceStub(Guid.NewGuid(), string.Empty, new ContentLocation());

                var exception = Assert.Throws<ArgumentException>(() => new CreateProjectOptions(parentProject, Create<string>(), Create<string?>(), Create<string?>(), Create<bool>()));

                Assert.Equal("parentProject.Location", exception.ParamName);
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void Throws_when_name_is_null_empty_or_whitespace(string? name)
            {
                var exception = Assert.Throws<ArgumentException>(() => new CreateProjectOptions(null, name!, Create<string?>(), Create<string?>(), Create<bool>()));

                Assert.Equal("name", exception.ParamName);
            }
        }
    }
}
