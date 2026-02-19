//
//  Copyright (c) 2026, Salesforce, Inc.
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

using Moq;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Types
{
    public sealed class GranteeCapabilityTypeTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Theory]
            [EnumData<GranteeType>]
            public void SetsCorrectGranteeType(GranteeType type)
            {
                var mockCapability = Create<Mock<IGranteeCapability>>();
                mockCapability.SetupGet(x => x.GranteeType).Returns(type);

                var c = new GranteeCapabilityType(mockCapability.Object);

                Assert.Equal(type, c.GranteeType);
                Assert.Equal(mockCapability.Object.Grantee.Id, c.GranteeId);
            }
        }

        public sealed class GranteeId : AutoFixtureTestBase
        {
            private IGranteeCapability CreateTestCapability(GranteeType type)
            {
                var mockCapability = Create<Mock<IGranteeCapability>>();
                mockCapability.SetupGet(x => x.GranteeType).Returns(type);
                return mockCapability.Object;
            }

            [Fact]
            public void UserId()
            {
                var c = CreateTestCapability(GranteeType.User);
                var r = new GranteeCapabilityType(c);

                Assert.NotNull(r.User);
                Assert.Equal(c.Grantee.Id, r.User.Id);
                Assert.Equal(c.Grantee.Id, r.GranteeId);
            }

            [Fact]
            public void GroupId()
            {
                var c = CreateTestCapability(GranteeType.Group);
                var r = new GranteeCapabilityType(c);

                Assert.NotNull(r.Group);
                Assert.Equal(c.Grantee.Id, r.Group.Id);
                Assert.Equal(c.Grantee.Id, r.GranteeId);
            }

            [Fact]
            public void GroupSetId()
            {
                var c = CreateTestCapability(GranteeType.GroupSet);
                var r = new GranteeCapabilityType(c);

                Assert.NotNull(r.GroupSet);
                Assert.Equal(c.Grantee.Id, r.GroupSet.Id);
                Assert.Equal(c.Grantee.Id, r.GranteeId);
            }
        }

        public sealed class GranteeTypeProperty : AutoFixtureTestBase
        {
            [Fact]
            public void User()
            {
                var r = new GranteeCapabilityType()
                {
                    User = new()
                };

                Assert.Equal(GranteeType.User, r.GranteeType);
            }

            [Fact]
            public void Group()
            {
                var r = new GranteeCapabilityType()
                {
                    Group = new()
                };

                Assert.Equal(GranteeType.Group, r.GranteeType);
            }

            [Fact]
            public void GroupSet()
            {
                var r = new GranteeCapabilityType()
                {
                    GroupSet = new()
                };

                Assert.Equal(GranteeType.GroupSet, r.GranteeType);
            }
        }
    }
}
