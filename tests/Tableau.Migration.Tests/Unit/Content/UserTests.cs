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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class UserTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            protected UsersResponse.UserType CreateTestResponse()
            {
                return new UsersResponse.UserType
                {
                    Domain = new()
                    {
                        Name = Create<string>()
                    },
                    Id = Create<Guid>(),
                    Name = Create<string>(),
                    FullName = Create<string>(),
                    Email = Create<string>(),
                    SiteRole = Create<string>(),
                    Language = Create<string>(),
                    Locale = Create<string?>(),
                    AuthSetting = Create<string?>(),
                };
            }

            [Fact]
            public void DomainObjectRequired()
            {
                var response = CreateTestResponse();
                response.Domain = null;

                Assert.Throws<ArgumentNullException>(() => new User(response));
            }

            [Fact]
            public void EmptyId()
            {
                var response = CreateTestResponse();
                response.Id = Guid.Empty;

                Assert.Throws<ArgumentException>(() => new User(response));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void UsernameRequired(string? name)
            {
                var response = CreateTestResponse();
                response.Name = name;

                Assert.Throws<ArgumentException>(() => new User(response));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void DomainNameRequired(string? name)
            {
                var response = CreateTestResponse();
                response.Domain!.Name = name;

                Assert.Throws<ArgumentException>(() => new User(response));
            }

            [Fact]
            public void BuildsLocation()
            {
                var response = CreateTestResponse();

                var user = new User(response);

                Assert.Equal(ContentLocation.ForUsername(response.Domain?.Name!, response.Name!), user.Location);
            }

            [Fact]
            public void ExposeFullAuthInfo()
            {
                var response = CreateTestResponse();
                response.IdpConfigurationId = Guid.NewGuid().ToString();
                var user = new User(response);

                Assert.Equal(response.GetAuthenticationType(), user.Authentication);
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void AuthTypeOptional(string? authSetting)
            {
                var response = CreateTestResponse();
                response.AuthSetting = authSetting;

                var user = new User(response);

                Assert.Equal(authSetting, user.Authentication.AuthenticationType);
            }

            [Fact]
            public void IdpConfigurationId()
            {
                var id = Guid.NewGuid();

                var response = CreateTestResponse();
                response.AuthSetting = null;
                response.IdpConfigurationId = id.ToString();

                var user = new User(response);

                Assert.Null(user.Authentication.AuthenticationType);
                Assert.Equal(id, user.Authentication.IdpConfigurationId);
            }

            [Fact]
            public void UpdateUserResult()
            {
                var id = Guid.NewGuid();
                var result = Create<IUpdateUserResult>();

                var user = new User(id, result);

                Assert.Equal(id, user.Id);
                Assert.Empty(user.Domain);
                Assert.Equal(result.Email, user.Email);
                Assert.Equal(result.FullName, user.FullName);
                Assert.Equal(result.SiteRole, user.SiteRole);
                Assert.Equal(result.Authentication, user.Authentication);
            }
        }
    }
}
