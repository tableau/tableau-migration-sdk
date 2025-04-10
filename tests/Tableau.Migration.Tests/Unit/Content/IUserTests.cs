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

using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public sealed class IUserTests
    {
        public sealed class AuthenticationType : AutoFixtureTestBase
        {
            [Fact]
            public void SetsAuthType()
            {
                IUser user = new User(Create<UsersResponse.UserType>());

                var type = Create<string>();
                user.AuthenticationType = type;

                Assert.Null(user.Authentication.IdpConfigurationId);
                Assert.Equal(type, user.Authentication.AuthenticationType);
                Assert.Equal(user.AuthenticationType, user.Authentication.AuthenticationType);
            }

            [Fact]
            public void NullDoesNotOverwriteIdpId()
            {
                IUser user = new User(Create<UsersResponse.UserType>());

                var id = user.Authentication.IdpConfigurationId;
                user.AuthenticationType = null;

                Assert.Equal(id, user.Authentication.IdpConfigurationId);
                Assert.Null(user.Authentication.AuthenticationType);
                Assert.Null(user.AuthenticationType);
            }
        }
    }
}
