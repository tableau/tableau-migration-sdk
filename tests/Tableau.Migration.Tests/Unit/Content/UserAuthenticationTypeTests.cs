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
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public sealed class UserAuthenticationTypeTests
    {
        public sealed class Default
        {
            [Fact]
            public void HasNullValues()
            {
                Assert.Null(UserAuthenticationType.Default.AuthenticationType);
                Assert.Null(UserAuthenticationType.Default.IdpConfigurationId);
            }
        }

        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void AuthType()
            {
                var authType = Create<string>();

                var t = new UserAuthenticationType(authType, null);

                Assert.Equal(authType, t.AuthenticationType);
                Assert.Null(t.IdpConfigurationId);
            }

            [Fact]
            public void IdpConfigurationId()
            {
                var id = Guid.NewGuid();

                var t = new UserAuthenticationType(null, id);

                Assert.Equal(id, t.IdpConfigurationId);
                Assert.Null(t.AuthenticationType);
            }
        }

        public sealed class ForAuthenticationType : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var authType = Create<string>();

                var t = UserAuthenticationType.ForAuthenticationType(authType);

                Assert.Equal(authType, t.AuthenticationType);
                Assert.Null(t.IdpConfigurationId);
            }
        }

        public sealed class ForConfigurationId : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var id = Guid.NewGuid();

                var t = UserAuthenticationType.ForConfigurationId(id);

                Assert.Equal(id, t.IdpConfigurationId);
                Assert.Null(t.AuthenticationType);
            }
        }
    }
}
