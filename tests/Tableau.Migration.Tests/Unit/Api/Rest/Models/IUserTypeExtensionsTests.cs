//
//  Copyright (c) 2025, Salesforce, Inc.
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
using Moq;
using Tableau.Migration.Api.Rest.Models;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public sealed class IUserTypeExtensionsTests
    {
        public sealed class GetIdpConfigurationId : AutoFixtureTestBase
        {
            [Fact]
            public void Parses()
            {
                var id = Guid.NewGuid();
                var mockUser = Create<Mock<IUserType>>();
                mockUser.SetupGet(x => x.IdpConfigurationId).Returns(id.ToString());

                var result = mockUser.Object.GetIdpConfigurationId();

                Assert.NotNull(result);
                Assert.Equal(id, result);
            }

            [Fact]
            public void Null()
            {
                var mockUser = Create<Mock<IUserType>>();
                mockUser.SetupGet(x => x.IdpConfigurationId).Returns((string?)null);

                var result = mockUser.Object.GetIdpConfigurationId();

                Assert.Null(result);
            }

            [Fact]
            public void ParseError()
            {
                var mockUser = Create<Mock<IUserType>>();
                mockUser.SetupGet(x => x.IdpConfigurationId).Returns("test");

                var result = mockUser.Object.GetIdpConfigurationId();

                Assert.Null(result);
            }
        }

        public sealed class GetAuthenticationType : AutoFixtureTestBase
        {
            [Fact]
            public void BuildsFullAuthInfo()
            {
                var id = Guid.NewGuid();

                var mockUser = Create<Mock<IUserType>>();
                mockUser.SetupGet(x => x.IdpConfigurationId).Returns(id.ToString());

                var result = mockUser.Object.GetAuthenticationType();

                Assert.Equal(mockUser.Object.AuthSetting, result.AuthenticationType);
                Assert.Equal(id, result.IdpConfigurationId);
            }
        }
    }
}
