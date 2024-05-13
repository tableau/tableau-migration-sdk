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

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class UserTableauCloudAuthenticationTypeTransformerTests
    {
        public class ExecuteAsync : OptionsHookTestBase<UserAuthenticationTypeTransformerOptions>
        {
            [Fact]
            public async Task SetsServerDefaultAuthType()
            {
                var mockUser = new Mock<IUser>();
                var mockLogger = new Mock<ILogger<UserAuthenticationTypeTransformer>>();
                var mockLocalizer = new MockSharedResourcesLocalizer();

                Options = new UserAuthenticationTypeTransformerOptions
                {
                    AuthenticationType = AuthenticationTypes.TableauIdWithMfa
                };

                var t = new UserAuthenticationTypeTransformer(MockOptionsProvider.Object, mockLocalizer.Object, mockLogger.Object);

                var resultUser = await t.ExecuteAsync(mockUser.Object, default);

                Assert.Same(resultUser, mockUser.Object);
                mockUser.VerifySet(x => x.AuthenticationType = AuthenticationTypes.TableauIdWithMfa, Times.Once);
            }
        }
    }
}
