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

using System;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class ViewsApiClientTest : AutoFixtureTestBase
    {
        internal readonly Mock<IViewsApiClient> MockPermissionsClient;


        public ViewsApiClientTest()
        {
            MockPermissionsClient = new Mock<IViewsApiClient>();
        }

        #region - Test Helpers -

        public void SetupGetPermissionsAsync(bool success, IPermissions? permissions = null)
        {
            var setup = MockPermissionsClient
                .Setup(c => c.Permissions.GetPermissionsAsync(
                    It.IsAny<Guid>(),
                    Cancel));

            if (success)
            {
                Assert.NotNull(permissions);

                setup.Returns(
                    Task.FromResult<IResult<IPermissions>>(
                        Result<IPermissions>.Create(Result.Succeeded(), permissions)));
                return;
            }

            setup.Returns(Task.FromResult<IResult<IPermissions>>(Result<IPermissions>.Failed(new Exception())));
        }

        public void VerifyGetPermissionsAsync(Times times)
        {
            MockPermissionsClient
                .Verify(c => c.Permissions.GetPermissionsAsync(
                        It.IsAny<Guid>(),
                        Cancel),
                    times);
        }

        #endregion

        public class GetPermissionsAsync : ViewsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                var sourcePermissions = Create<IPermissions>();
                var destinationPermissions = Create<IPermissions>();

                SetupGetPermissionsAsync(true, destinationPermissions);

                var result = await MockPermissionsClient.Object.Permissions.GetPermissionsAsync(
                    Guid.NewGuid(),
                    Cancel);

                Assert.True(result.Success);

                // Get permissions is called once.
                VerifyGetPermissionsAsync(Times.Once());
            }
        }
    }
}