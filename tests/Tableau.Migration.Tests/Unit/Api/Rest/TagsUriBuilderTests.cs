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
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest
{
    public class TagsUriBuilderTests
    {
        public abstract class TagsUriBuilderTest : AutoFixtureTestBase
        { }

        public class Prefix : TagsUriBuilderTest
        {
            [Fact]
            public void Initializes()
            {
                var prefix = Create<string>();

                var builder = new TagsUriBuilder(prefix);

                Assert.Equal(prefix, builder.Prefix);
            }
        }

        public class Suffix : TagsUriBuilderTest
        {
            [Fact]
            public void Initializes()
            {
                var builder = new TagsUriBuilder(Create<string>());

                Assert.Equal("tags", builder.Suffix);
            }
        }

        public class BuildUri : TagsUriBuilderTest
        {
            [Fact]
            public void Builds()
            {
                var prefix = Create<string>();
                var id = Create<Guid>();

                var builder = new TagsUriBuilder(prefix);

                var uri = builder.BuildUri(id);

                Assert.Equal(uri, $"{prefix}/{id.ToUrlSegment()}/tags");
            }
        }
    }
}
