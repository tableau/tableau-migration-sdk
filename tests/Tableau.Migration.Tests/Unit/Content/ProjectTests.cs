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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class ProjectTests
    {
        public abstract class ProjectTest : AutoFixtureTestBase
        {
            protected (ProjectsResponse.ProjectType, IContentReference?) CreateTestListResponse()
            {
                var response = new ProjectsResponse.ProjectType
                {
                    Id = Create<Guid>(),
                    Name = Create<string>(),
                    Description = Create<string>(),
                    ContentPermissions = Create<string>(),
                    ParentProjectId = Create<Guid?>()?.ToString()
                };

                IContentReference? parent = null;
                var parentId = response.GetParentProjectId();
                if (parentId is not null)
                {
                    parent = new ContentReferenceStub(parentId.Value, string.Empty, Create<ContentLocation>());
                }

                return (response, parent);
            }

            protected (CreateProjectResponse, IContentReference?) CreateTestCreateResponse()
            {
                var response = new CreateProjectResponse
                {
                    Item = new CreateProjectResponse.ProjectType
                    {
                        Id = Create<Guid>(),
                        Name = Create<string>(),
                        Description = Create<string>(),
                        ContentPermissions = Create<string>(),
                        ParentProjectId = Create<Guid?>()?.ToString()
                    }
                };

                IContentReference? parent = null;
                var parentId = response.Item.GetParentProjectId();
                if (parentId is not null)
                {
                    parent = new ContentReferenceStub(parentId.Value, string.Empty, Create<ContentLocation>());
                }

                return (response, parent);
            }
        }

        public class Ctor
        {
            public class FromProjectsResponse : ProjectTest
            {
                [Fact]
                public void Initializes()
                {
                    (var response, var parent) = CreateTestListResponse();
                    var owner = Create<IContentReference>();
                    var project = new Project(response, parent, owner);

                    Assert.Equal(response.Id, project.Id);
                    Assert.Equal(response.Name, project.Name);
                    Assert.Equal(response.Description, project.Description);
                    Assert.Equal(response.ContentPermissions, project.ContentPermissions);

                    Assert.Same(parent, project.ParentProject);
                    Assert.Equal(response.GetParentProjectId(), project.ParentProject!.Id);

                    Assert.Same(owner, project.Owner);
                }

                [Fact]
                public void TopLevelProject()
                {
                    (var response, var _) = CreateTestListResponse();
                    response.ParentProjectId = null;

                    var project = new Project(response, null, Create<IContentReference>());

                    Assert.Equal(response.Id, project.Id);
                    Assert.Equal(response.Name, project.Name);
                    Assert.Equal(response.Description, project.Description);
                    Assert.Equal(response.ContentPermissions, project.ContentPermissions);

                    Assert.Null(project.ParentProject);

                    Assert.Equal(new ContentLocation(project.Name), project.Location);
                }

                [Fact]
                public void HasParentProject()
                {
                    (var response, var _) = CreateTestListResponse();

                    var parentId = Create<Guid>();
                    response.ParentProjectId = parentId.ToString();
                    var parent = new ContentReferenceStub(parentId, string.Empty, Create<ContentLocation>());

                    var project = new Project(response, parent, Create<IContentReference>());

                    Assert.Equal(response.Id, project.Id);
                    Assert.Equal(response.Name, project.Name);
                    Assert.Equal(response.Description, project.Description);
                    Assert.Equal(response.ContentPermissions, project.ContentPermissions);

                    Assert.Equal(new ContentLocation(parent.Location, project.Name), project.Location);
                }

                [Fact]
                public void ParentProjectIdMismatch()
                {
                    (var response, var _) = CreateTestListResponse();

                    var parentId = Create<Guid>();
                    var parent = new ContentReferenceStub(parentId, string.Empty, Create<ContentLocation>());

                    response.ParentProjectId = Create<Guid>().ToString();

                    Assert.Throws<ArgumentException>(() => new Project(response, parent, Create<IContentReference>()));
                }

                [Fact]
                public void EmptyId()
                {
                    (var response, var parent) = CreateTestListResponse();
                    response.Id = Guid.Empty;

                    var loc = new ContentLocation(response.Name ?? string.Empty);

                    Assert.Throws<ArgumentException>(() => new Project(response, parent, Create<IContentReference>()));
                }

                [Theory]
                [NullEmptyWhiteSpaceData]
                public void NameRequired(string? name)
                {
                    (var response, var parent) = CreateTestListResponse();
                    response.Name = name;

                    Assert.Throws<ArgumentException>(() => new Project(response, parent, Create<IContentReference>()));
                }

                [Theory]
                [NullEmptyWhiteSpaceData]
                public void EmptyDescriptionSupported(string? description)
                {
                    (var response, var parent) = CreateTestListResponse();
                    response.Description = description;

                    var proj = new Project(response, parent, Create<IContentReference>());
                    Assert.Equal(description, response.Description);
                }

                [Theory]
                [NullEmptyWhiteSpaceData]
                public void ContentPermissionsRequired(string? permissions)
                {
                    (var response, var parent) = CreateTestListResponse();
                    response.ContentPermissions = permissions;

                    Assert.Throws<ArgumentException>(() => new Project(response, parent, Create<IContentReference>()));
                }
            }

            public class FromCreateProjectResponse : ProjectTest
            {
                [Fact]
                public void Initializes()
                {
                    (var response, var parent) = CreateTestCreateResponse();
                    Assert.NotNull(response.Item);

                    var loc = new ContentLocation(response.Item.Name ?? string.Empty);
                    var owner = Create<IContentReference>();
                    var project = new Project(response, parent, owner);

                    Assert.Equal(response.Item.Id, project.Id);
                    Assert.Equal(response.Item.Name, project.Name);
                    Assert.Equal(response.Item.Description, project.Description);
                    Assert.Equal(response.Item.ContentPermissions, project.ContentPermissions);

                    Assert.Same(parent, project.ParentProject);
                    Assert.Equal(response.Item.GetParentProjectId(), project.ParentProject!.Id);

                    Assert.Same(owner, project.Owner);
                }

                [Fact]
                public void EmptyId()
                {
                    (var response, var parent) = CreateTestCreateResponse();
                    Assert.NotNull(response.Item);
                    response.Item.Id = Guid.Empty;

                    Assert.Throws<ArgumentException>(() => new Project(response, parent, Create<IContentReference>()));
                }

                [Theory]
                [NullEmptyWhiteSpaceData]
                public void NameRequired(string? name)
                {
                    (var response, var parent) = CreateTestCreateResponse();
                    Assert.NotNull(response.Item);
                    response.Item.Name = name;

                    Assert.Throws<ArgumentException>(() => new Project(response, parent, Create<IContentReference>()));
                }

                [Theory]
                [NullEmptyWhiteSpaceData]
                public void EmptyDescriptionSupported(string? description)
                {
                    (var response, var parent) = CreateTestCreateResponse();
                    Assert.NotNull(response.Item);
                    response.Item.Description = description;

                    var proj = new Project(response, parent, Create<IContentReference>());
                    Assert.Equal(description, response.Item.Description);
                }

                [Theory]
                [NullEmptyWhiteSpaceData]
                public void ContentPermissionsRequired(string? permissions)
                {
                    (var response, var parent) = CreateTestCreateResponse();
                    Assert.NotNull(response.Item);
                    response.Item.ContentPermissions = permissions;

                    Assert.Throws<ArgumentException>(() => new Project(response, parent, Create<IContentReference>()));
                }
            }
        }
    }
}

