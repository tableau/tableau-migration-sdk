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
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class GroupTests
    {
        public class Ctor
        {
            public class From_GroupsResponse : AutoFixtureTestBase
            {
                protected GroupsResponse.GroupType CreateTestResponse()
                {
                    return new GroupsResponse.GroupType
                    {
                        Domain = new()
                        {
                            Name = Create<string>()
                        },
                        Id = Create<Guid>(),
                        Name = Create<string>()
                    };
                }

                [Fact]
                public void DomainObjectRequired()
                {
                    var response = CreateTestResponse();
                    response.Domain = null;

                    Assert.Throws<ArgumentNullException>(() => new Group(response));
                }

                [Fact]
                public void EmptyId()
                {
                    var response = CreateTestResponse();
                    response.Id = Guid.Empty;

                    Assert.Throws<ArgumentException>(() => new Group(response));
                }

                [Theory]
                [NullEmptyWhiteSpaceData]
                public void GroupNameRequired(string? name)
                {
                    var response = CreateTestResponse();
                    response.Name = name;

                    Assert.Throws<ArgumentException>(() => new Group(response));
                }

                [Theory]
                [NullEmptyWhiteSpaceData]
                public void DomainNameRequired(string? name)
                {
                    var response = CreateTestResponse();
                    response.Domain!.Name = name;

                    Assert.Throws<ArgumentException>(() => new Group(response));
                }

                [Fact]
                public void BuildsLocation()
                {
                    var response = CreateTestResponse();

                    var group = new Group(response);

                    Assert.Equal(ContentLocation.ForUsername(response.Domain?.Name!, response.Name!), group.Location);
                }
            }

            public class From_CreateLocalGroupResponse : AutoFixtureTestBase
            {
                protected CreateGroupResponse CreateTestResponse()
                {
                    return new CreateGroupResponse
                    {
                        Item = new()
                        {
                            Import = new()
                            {
                                DomainName = Create<string>()
                            },
                            Id = Create<Guid>(),
                            Name = Create<string>()
                        }
                    };
                }

                [Fact]
                public void ItemObjectRequired()
                {
                    var response = CreateTestResponse();
                    Assert.NotNull(response?.Item?.Import);

                    response.Item = null;

                    Assert.Throws<ArgumentNullException>(() => new Group(response));
                }

                [Fact]
                public void ImportObjectMissingIsLocalGroup()
                {
                    var response = CreateTestResponse();
                    Assert.NotNull(response?.Item?.Import);

                    response.Item.Import = null;

                    var group = new Group(response);

                    Assert.Equal(Constants.LocalDomain, group.Domain);
                }

                [Fact]
                public void EmptyId()
                {
                    var response = CreateTestResponse();
                    Assert.NotNull(response?.Item?.Import);

                    response.Item.Id = Guid.Empty;

                    Assert.Throws<ArgumentException>(() => new Group(response));
                }

                [Theory]
                [NullEmptyWhiteSpaceData]
                public void GroupNameRequired(string? name)
                {
                    var response = CreateTestResponse();
                    Assert.NotNull(response?.Item?.Import);

                    response.Item.Name = name;

                    Assert.Throws<ArgumentException>(() => new Group(response));
                }

                [Theory]
                [NullEmptyWhiteSpaceData]
                public void DomainNameRequired(string? name)
                {
                    var response = CreateTestResponse();
                    Assert.NotNull(response?.Item?.Import);

                    response.Item.Import.DomainName = name;

                    Assert.Throws<ArgumentException>(() => new Group(response));
                }

                [Fact]
                public void BuildsLocation()
                {
                    var response = CreateTestResponse();
                    Assert.NotNull(response?.Item?.Import);

                    var group = new Group(response);

                    Assert.Equal(ContentLocation.ForUsername(response.Item.Import.DomainName!, response.Item.Name!), group.Location);
                }
            }
        }
    }
}
