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
using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class DataSourceTests
    {
        public class Ctor
        {
            public class FromDataSourcesResponse : AutoFixtureTestBase
            {
                protected DataSourcesResponse.DataSourceType CreateTestResponse()
                {
                    return Create<DataSourcesResponse.DataSourceType>();
                }

                private static void AssertSuccess(DataSourcesResponse.DataSourceType response, DataSource result
                    , IContentReference projectRef, IContentReference ownerRef)
                {
                    Assert.NotNull(result);
                    Assert.Equal(response.Id, result.Id);
                    Assert.Equal(response.Name, result.Name);
                    Assert.Equal(response.ContentUrl, result.ContentUrl);

                    Assert.Equal(response.Description, result.Description);
                    Assert.Equal(response.CreatedAt, result.CreatedAt);
                    Assert.Equal(response.UpdatedAt, result.UpdatedAt);

                    Assert.Equal(response.EncryptExtracts, result.EncryptExtracts);
                    Assert.Equal(response.HasExtracts, result.HasExtracts);
                    Assert.Equal(response.IsCertified, result.IsCertified);
                    Assert.Equal(response.UseRemoteQueryAgent, result.UseRemoteQueryAgent);

                    Assert.Equal(response.WebpageUrl, result.WebpageUrl);

                    var project = response.Project;
                    Assert.NotNull(project);

                    Assert.Same(projectRef, result.Container);
                    Assert.Equal(projectRef.Location.Append(result.Name), result.Location);

                    Assert.Same(ownerRef, result.Owner);

                    AssertTags(response.Tags, result.Tags);

                    static void AssertTags(DataSourcesResponse.DataSourceType.TagType[]? response, IList<ITag> result)
                    {
                        if (response is null)
                        {
                            Assert.Empty(result);
                            return;
                        }

                        Assert.Equal(response.Length, result.Count);

                        foreach (var responseTag in response)
                        {
                            Assert.NotNull(result.FirstOrDefault(t => t.Label == responseTag.Label));
                        }
                    }
                }

                [Fact]
                public void Success()
                {
                    var response = CreateTestResponse();
                    var project = Create<IContentReference>();
                    var owner = Create<IContentReference>();

                    var result = new DataSource(response, project, owner);
                    FromDataSourcesResponse.AssertSuccess(response, result, project, owner);
                }

                [Fact]
                public void EmptyId()
                {
                    var response = CreateTestResponse();
                    response.Id = Guid.Empty;

                    Assert.Throws<ArgumentException>(() => new DataSource(response, Create<IContentReference>(), Create<IContentReference>()));
                }

                [Theory]
                [InlineData(null)]
                [InlineData("")]
                [InlineData(" ")]
                public void InvalidName(string? testName)
                {
                    var response = CreateTestResponse();
                    response.Name = testName;

                    Assert.Throws<ArgumentException>(() => new DataSource(response, Create<IContentReference>(), Create<IContentReference>()));
                }

                [Theory]
                [InlineData(null)]
                [InlineData("")]
                [InlineData(" ")]
                public void InvalidContentUrl(string? testSize)
                {
                    var response = CreateTestResponse();
                    response.ContentUrl = testSize;

                    Assert.Throws<ArgumentException>(() => new DataSource(response, Create<IContentReference>(), Create<IContentReference>()));
                }

                [Fact]
                public void Handles_Empty_Tags()
                {
                    var response = CreateTestResponse();
                    response.Tags = [];

                    var project = Create<IContentReference>();
                    var owner = Create<IContentReference>();

                    var result = new DataSource(response, project, owner);

                    FromDataSourcesResponse.AssertSuccess(response, result, project, owner);
                }

                [Fact]
                public void MapsTags()
                {
                    var response = CreateTestResponse();
                    var project = Create<IContentReference>();
                    var owner = Create<IContentReference>();

                    var result = new DataSource(response, project, owner);

                    Assert.NotNull(response?.Tags);
                    FromDataSourcesResponse.AssertSuccess(response, result, project, owner);
                }
            }
        }
    }
}