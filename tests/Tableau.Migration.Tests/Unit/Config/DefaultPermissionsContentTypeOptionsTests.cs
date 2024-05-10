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
using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Config
{
    public class DefaultPermissionsContentTypeOptionsTests
    {
        public abstract class DefaultPermissionsContentTypeOptionsTest : AutoFixtureTestBase
        {
            protected static readonly IImmutableList<string> DefaultUrlSegments = DefaultPermissionsContentTypeUrlSegments.GetAll();
        }

        public class Ctor : DefaultPermissionsContentTypeOptionsTest
        {
            [Fact]
            public void Initializes_default()
            {
                var options = new DefaultPermissionsContentTypeOptions();

                Assert.True(options.UrlSegments.SequenceEqual(DefaultUrlSegments));
            }

            [Fact]
            public void Initializes_with_custom_segments()
            {
                var customUrlSegments = CreateMany<string>(10);

                var options = new DefaultPermissionsContentTypeOptions(customUrlSegments);

                Assert.True(options.UrlSegments.SequenceEqual(DefaultUrlSegments.Concat(customUrlSegments)));
            }

            [Fact]
            public void Deduplicates()
            {
                var options = new DefaultPermissionsContentTypeOptions(DefaultUrlSegments);

                Assert.True(options.UrlSegments.SequenceEqual(DefaultUrlSegments));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void Ignores_null_empty_or_whitespace_segments(string? customUrlSegment)
            {
                var options = new DefaultPermissionsContentTypeOptions(new[] { customUrlSegment! });

                Assert.True(options.UrlSegments.SequenceEqual(DefaultUrlSegments));
            }
        }
    }
}
