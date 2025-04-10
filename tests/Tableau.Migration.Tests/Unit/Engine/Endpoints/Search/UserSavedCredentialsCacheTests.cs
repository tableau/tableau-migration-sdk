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
using Tableau.Migration.Api.Models;
using Tableau.Migration.Engine.Endpoints.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public class UserSavedCredentialsCacheTests : AutoFixtureTestBase
    {
        protected readonly IUserSavedCredentialsCache Cache;

        protected readonly Guid UserIdWithData;

        protected readonly IEmbeddedCredentialKeychainResult SavedCredentials;

        protected readonly Guid UserIdWithoutData;

        public UserSavedCredentialsCacheTests()
        {
            UserIdWithData = Guid.NewGuid();
            UserIdWithoutData = Guid.NewGuid();
            SavedCredentials = Create<IEmbeddedCredentialKeychainResult>();

            Cache = new UserSavedCredentialsCache();
            Cache.AddOrUpdate(UserIdWithData, SavedCredentials);
        }
        public class Get : UserSavedCredentialsCacheTests
        {
            [Fact]
            public void Null_on_no_match()
            {
                var result = Cache.Get(Guid.NewGuid());
                Assert.Null(result);
            }

            [Fact]
            public void Returns_value()
            {
                var result = Cache.Get(UserIdWithData);
                Assert.Equal(result, SavedCredentials);
            }

            [Fact]
            public void Returns_value_after_addition()
            {
                var userId = Guid.NewGuid();
                var savedCreds = Create<IEmbeddedCredentialKeychainResult>();
                Cache.AddOrUpdate(userId, savedCreds);

                var result = Cache.Get(userId);
                Assert.Equal(result, savedCreds);
            }
        }

        public class AddOrUpdate : UserSavedCredentialsCacheTests
        {
            [Fact]
            public void Adds_successfully()
            {
                var userId = Guid.NewGuid();
                var savedCreds = Create<IEmbeddedCredentialKeychainResult>();

                var result = Cache.AddOrUpdate(userId, savedCreds);
                Assert.Equal(result, savedCreds);
            }
        }
    }
}
