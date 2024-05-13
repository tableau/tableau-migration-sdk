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

using System.Collections.Immutable;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class UsernameContentBaseTests
    {
        public class TestUsernameContent : UsernameContentBase
        {
            new public string Name
            {
                get => base.Name;
                set => base.Name = value;
            }

            new public string Domain
            {
                get => base.Domain;
                set => base.Domain = value;
            }
        }

        public class Name : AutoFixtureTestBase
        {
            [Fact]
            public void UpdatesLocation()
            {
                var c = new TestUsernameContent();

                c.Name = Create<string>();

                Assert.Equal(ContentLocation.ForUsername(c.Domain, c.Name), c.Location);
            }
        }

        public class Domain : AutoFixtureTestBase
        {
            [Fact]
            public void UpdatesLocation()
            {
                var c = new TestUsernameContent();

                c.Domain = Create<string>();

                Assert.Equal(ContentLocation.ForUsername(c.Domain, c.Name), c.Location);
            }
        }

        public class SetLocation
        {
            [Theory]
            [InlineData(new string[0], "", "")]
            [InlineData(new[] { "username" }, "", "username")]
            [InlineData(new[] { "domain", "username" }, "domain", "username")]
            [InlineData(new[] { "domain", "username", "extra" }, "domain", "username")]
            public void SetsDomainAndUsername(string[] segments, string expectedDomain, string expectedUsername)
            {
                var c = new TestUsernameContent();

                var mappedLoc = new ContentLocation(segments.ToImmutableArray(), Constants.DomainNameSeparator);
                ((IMappableContent)c).SetLocation(mappedLoc);

                Assert.Equal(expectedDomain, c.Domain);
                Assert.Equal(expectedUsername, c.Name);
            }
        }
    }
}
