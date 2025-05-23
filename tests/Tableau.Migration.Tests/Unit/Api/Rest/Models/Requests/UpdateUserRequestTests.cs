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
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    public sealed class UpdateUserRequestTests
    {
        public sealed class Ctor
        {
            [Fact]
            public void Optional()
            {
                var r = new UpdateUserRequest("siteRole");

                Assert.NotNull(r.User);
                Assert.Equal("siteRole", r.User.SiteRole);

                Assert.Null(r.User.FullName);
                Assert.Null(r.User.Email);
                Assert.Null(r.User.Password);
                Assert.Null(r.User.AuthSetting);
                Assert.Null(r.User.IdpConfigurationId);
            }

            [Fact]
            public void FullName()
            {
                var r = new UpdateUserRequest("siteRole", "full name");

                Assert.NotNull(r.User);
                Assert.Equal("full name", r.User.FullName);
            }

            [Fact]
            public void Email()
            {
                var r = new UpdateUserRequest("siteRole", newEmail: "email");

                Assert.NotNull(r.User);
                Assert.Equal("email", r.User.Email);
            }

            [Fact]
            public void Password()
            {
                var r = new UpdateUserRequest("siteRole", newPassword: "pass");

                Assert.NotNull(r.User);
                Assert.Equal("pass", r.User.Password);
            }

            [Fact]
            public void UpdateIdpConfigurationId()
            {
                var auth = UserAuthenticationType.ForConfigurationId(Guid.NewGuid());
                var r = new UpdateUserRequest("siteRole", newAuthentication: auth);

                Assert.NotNull(r.User);
                Assert.Null(r.User.AuthSetting);
                Assert.Equal(auth.IdpConfigurationId.ToString(), r.User.IdpConfigurationId);
            }

            [Fact]
            public void AuthSetting()
            {
                var auth = UserAuthenticationType.ForAuthenticationType("authType");
                var r = new UpdateUserRequest("siteRole", newAuthentication: auth);

                Assert.NotNull(r.User);
                Assert.Equal(auth.AuthenticationType, r.User.AuthSetting);
                Assert.Null(r.User.IdpConfigurationId);
            }
        }
    }
}
