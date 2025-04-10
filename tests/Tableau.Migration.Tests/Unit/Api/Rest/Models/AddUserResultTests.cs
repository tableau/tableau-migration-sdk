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
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public sealed class AddUserResultTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void UserRequired()
            {
                var response = Create<AddUserResponse>();
                response.Item = null;

                Assert.Throws<ArgumentNullException>(() => new AddUserResult(response));
            }

            [Fact]
            public void IdRequired()
            {
                var response = Create<AddUserResponse>();
                response.Item!.Id = Guid.Empty;

                Assert.Throws<ArgumentException>(() => new AddUserResult(response));
            }

            [Fact]
            public void NameRequired()
            {
                var response = Create<AddUserResponse>();
                response.Item!.Name = string.Empty;

                Assert.Throws<ArgumentException>(() => new AddUserResult(response));
            }

            [Fact]
            public void SiteRoleRequired()
            {
                var response = Create<AddUserResponse>();
                response.Item!.SiteRole = string.Empty;

                Assert.Throws<ArgumentException>(() => new AddUserResult(response));
            }

            [Fact]
            public void ExposeFullAuthInfo()
            {
                var id = Guid.NewGuid();

                var response = Create<AddUserResponse>();
                response.Item!.IdpConfigurationId = id.ToString();

                var result = new AddUserResult(response);

                Assert.Equal(response.Item.AuthSetting, result.Authentication.AuthenticationType);
                Assert.Equal(id, result.Authentication.IdpConfigurationId);
            }

            [Fact]
            public void PreferIdpConfigurationId()
            {
                var id = Guid.NewGuid();

                var response = Create<AddUserResponse>();
                response.Item!.AuthSetting = null;
                response.Item.IdpConfigurationId = id.ToString();

                var result = new AddUserResult(response);

                Assert.Equal(UserAuthenticationType.ForConfigurationId(id), result.Authentication);
            }

            [Fact]
            public void AuthSetting()
            {
                var response = Create<AddUserResponse>();
                response.Item!.IdpConfigurationId = null;

                var result = new AddUserResult(response);

                Assert.Equal(UserAuthenticationType.ForAuthenticationType(response.Item.AuthSetting!), result.Authentication);
            }

            [Fact]
            public void NoAuth()
            {
                var response = Create<AddUserResponse>();
                response.Item!.AuthSetting = null;
                response.Item.IdpConfigurationId = null;

                var result = new AddUserResult(response);

                Assert.Equal(UserAuthenticationType.Default, result.Authentication);
            }
        }
    }
}
