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
    public class SiteTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            protected SiteResponse CreateTestResponse()
            {
                return new SiteResponse
                {
                    Item = new()
                    {
                        Id = Create<Guid>(),
                        Name = Create<string>(),
                        ContentUrl = Create<string>(),
                        ExtractEncryptionMode = ExtractEncryptionModes.Disabled
                    }
                };
            }

            [Fact]
            public void Initializes()
            {
                var response = CreateTestResponse();
                var site = new Site(response);

                Assert.Equal(response.Item!.Id, site.Id);
                Assert.Equal(response.Item.Name, site.Name);
                Assert.Equal(response.Item.ContentUrl, site.ContentUrl);
            }

            [Fact]
            public void ItemRequired()
            {
                var response = CreateTestResponse();
                response.Item = null;

                Assert.Throws<ArgumentNullException>(() => new Site(response));
            }

            [Fact]
            public void EmptyId()
            {
                var response = CreateTestResponse();
                response.Item!.Id = Guid.Empty;

                Assert.Throws<ArgumentException>(() => new Site(response));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void SiteNameRequired(string? name)
            {
                var response = CreateTestResponse();
                response.Item!.Name = name;

                Assert.Throws<ArgumentException>(() => new Site(response));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void ContentUrlShouldNotBeNull(string? contentUrl)
            {
                var response = CreateTestResponse();
                response.Item!.ContentUrl = contentUrl;

                if (contentUrl is null)
                {
                    Assert.Throws<ArgumentNullException>(() => new Site(response));
                }
                else
                {
                    var site = new Site(response);
                }
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void ExtractEncryptionModeRequired(string? s)
            {
                var response = CreateTestResponse();
                response.Item!.ExtractEncryptionMode = s;

                Assert.Throws<ArgumentException>(() => new Site(response));
            }
        }
    }
}
