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
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest
{
    public class PermissionsUriBuilderTests
    {
        public abstract class PermissionsUriBuilderTest : AutoFixtureTestBase
        { }

        public class Prefix : PermissionsUriBuilderTest
        {
            [Fact]
            public void Initializes()
            {
                var prefix = Create<string>();

                var builder = new PermissionsUriBuilder(prefix, Create<string>());

                Assert.Equal(prefix, builder.Prefix);
            }
        }

        public class Suffix : PermissionsUriBuilderTest
        {
            [Fact]
            public void Initializes()
            {
                var suffix = Create<string>();

                var builder = new PermissionsUriBuilder(Create<string>(), suffix);

                Assert.Equal(suffix, builder.Suffix);
            }
        }

        public class BuildUri : PermissionsUriBuilderTest
        {
            [Fact]
            public void Builds()
            {
                var prefix = Create<string>();
                var suffix = Create<string>();
                var id = Create<Guid>();

                var builder = new PermissionsUriBuilder(prefix, suffix);

                var uri = builder.BuildUri(id);

                Assert.Equal(uri, $"{prefix}/{id.ToUrlSegment()}/{suffix}");
            }
        }

        public class BuildDeleteUri : PermissionsUriBuilderTest
        {
            [Fact]
            public void Builds()
            {
                var prefix = Create<string>();
                var suffix = Create<string>();
                var id = Create<Guid>();
                var capability = Create<ICapability>();
                var granteeType = Create<GranteeType>();
                var granteeId = Create<Guid>();

                var builder = new PermissionsUriBuilder(prefix, suffix);

                var uri = builder.BuildDeleteUri(id, capability, granteeType, granteeId);

                Assert.Equal(uri, $"{prefix}/{id.ToUrlSegment()}/{suffix}/{granteeType.ToUrlSegment()}/{granteeId.ToUrlSegment()}/{capability.Name}/{capability.Mode}");
            }
        }
    }
}
