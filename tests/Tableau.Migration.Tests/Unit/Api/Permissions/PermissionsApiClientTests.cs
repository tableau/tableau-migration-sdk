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
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Permissions
{
    public class PermissionsApiClientTests
    {
        public abstract class PermissionsApiClientTest : AutoFixtureTestBase
        {
            protected readonly Mock<IRestRequestBuilderFactory> MockRestRequestBuilderFactory = new();
            protected readonly Mock<IHttpContentSerializer> MockSerializer = new();
            protected readonly Mock<IPermissionsUriBuilder> MockUriBuilder = new();
            protected readonly Mock<ISharedResourcesLocalizer> MockSharedResourcesLocalizer = new();

            internal readonly Mock<PermissionsApiClient> MockPermissionsClient;
            internal readonly PermissionsApiClient PermissionsClient;

            public PermissionsApiClientTest()
            {
                MockPermissionsClient = new Mock<PermissionsApiClient>(
                    MockRestRequestBuilderFactory.Object,
                    MockSerializer.Object,
                    MockUriBuilder.Object,
                    MockSharedResourcesLocalizer.Object)
                {
                    CallBase = true
                };

                PermissionsClient = MockPermissionsClient.Object;
            }

            #region - Test Helpers -

            public void SetupGetCapabilitiesToDelete(IGranteeCapability[] capabilities)
            {
                var deleteSetup = MockPermissionsClient
                                    .Setup(x =>
                                        x.GetCapabilitiesToDelete(
                                            It.IsAny<IGranteeCapability[]>(),
                                            It.IsAny<IGranteeCapability[]>()));

                deleteSetup.Returns(capabilities.ToImmutableArray());
            }

            public void SetupDeleteCapabilityAsync(bool success = true)
            {
                var deleteSetup = MockPermissionsClient
                    .Setup(c => c.DeleteCapabilityAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<Guid>(),
                        It.IsAny<GranteeType>(),
                        It.IsAny<ICapability>(),
                        Cancel));

                if (success)
                {
                    deleteSetup.Returns(Task.FromResult<IResult>(Result.Succeeded()));
                    return;
                }

                deleteSetup.Returns(Task.FromResult<IResult>(Result.Failed(new Exception())));
            }

            public void VerifyDeleteCapabilityAsync(Times times)
            {
                MockPermissionsClient
                    .Verify(c => c.DeleteCapabilityAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<Guid>(),
                        It.IsAny<GranteeType>(),
                        It.IsAny<ICapability>(),
                        Cancel),
                        times);
            }
            public void SetupDeletePermissionsAsync(bool success = true)
            {
                var deleteSetup = MockPermissionsClient
                    .Setup(c => c.DeletePermissionsAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<IPermissions>(),
                        It.IsAny<IPermissions>(),
                        Cancel));

                if (success)
                {
                    deleteSetup.Returns(Task.FromResult<IResult>(Result.Succeeded()));
                    return;
                }

                deleteSetup.Returns(Task.FromResult<IResult>(Result.Failed(new Exception())));
            }

            public void SetupCreatePermissionsAsync(bool success, IPermissions? permissions = null)
            {
                var setup = MockPermissionsClient
                    .Setup(c => c.CreatePermissionsAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<IPermissions>(),
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

            public void VerifyCreatePermissionsAsync(Times times)
            {
                MockPermissionsClient
                    .Verify(c => c.CreatePermissionsAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<IPermissions>(),
                        Cancel),
                        times);
            }

            public void SetupUpdatePermissionsAsync(bool success, IPermissions? permissions = null)
            {
                var setup = MockPermissionsClient
                    .Setup(c => c.UpdatePermissionsAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<IPermissions>(),
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

            public void SetupGetPermissionsAsync(bool success, IPermissions? permissions = null)
            {
                var setup = MockPermissionsClient
                    .Setup(c => c.GetPermissionsAsync(
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
                    .Verify(c => c.GetPermissionsAsync(
                        It.IsAny<Guid>(),
                        Cancel),
                        times);
            }


            #endregion
        }

        public class CreatePermissionsAsync : PermissionsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                var destinationPermissions = Create<IPermissions>();

                SetupCreatePermissionsAsync(true, destinationPermissions);

                var result = await PermissionsClient.CreatePermissionsAsync(
                    Guid.NewGuid(),
                    destinationPermissions,
                    Cancel);

                Assert.True(result.Success);
            }

            [Fact]
            public async Task Does_not_create_when_no_grantees()
            {
                var destinationPermissions = Create<IPermissions>();
                destinationPermissions.GranteeCapabilities = Array.Empty<IGranteeCapability>();

                var result = await PermissionsClient.CreatePermissionsAsync(
                    Guid.NewGuid(),
                    destinationPermissions,
                    Cancel);

                Assert.True(result.Success);

                MockRestRequestBuilderFactory.VerifyNoOtherCalls();
            }
        }

        public class DeleteAllPermissionsAsync : PermissionsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                var destinationPermissions = Create<IPermissions>();

                SetupDeleteCapabilityAsync(true);

                var result = await PermissionsClient.DeleteAllPermissionsAsync(
                    Guid.NewGuid(),
                    destinationPermissions,
                    Cancel);

                Assert.True(result.Success);
            }

            [Fact]
            public async Task Failure()
            {
                var destinationPermissions = Create<IPermissions>();

                SetupDeleteCapabilityAsync(false);

                var result = await PermissionsClient.DeleteAllPermissionsAsync(
                    Guid.NewGuid(),
                    destinationPermissions,
                    Cancel);

                Assert.False(result.Success);
            }
        }

        public class DeleteCapabilityAsync : PermissionsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                SetupDeleteCapabilityAsync(true);

                var result = await PermissionsClient.DeleteCapabilityAsync(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    GranteeType.Group,
                    new Capability(new CapabilityType()
                    {
                        Name = PermissionsCapabilityNames.Read,
                        Mode = PermissionsCapabilityModes.Allow
                    }),
                    Cancel);
                Assert.True(result.Success);
            }

            [Fact]
            public async Task Failure()
            {
                SetupDeleteCapabilityAsync(false);

                var result = await PermissionsClient.DeleteCapabilityAsync(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    GranteeType.Group,
                    new Capability(new CapabilityType()
                    {
                        Name = PermissionsCapabilityNames.Read,
                        Mode = PermissionsCapabilityModes.Allow
                    }),
                    Cancel);
                Assert.False(result.Success);
            }
        }

        public class DeletePermissionsAsync : PermissionsApiClientTest
        {
            public DeletePermissionsAsync()
            { }

            [Fact]
            public async Task Success()
            {
                var sourcePermissions = Create<IPermissions>();
                var destinationPermissions = Create<IPermissions>();

                SetupDeleteCapabilityAsync(true);

                var result = await PermissionsClient.DeletePermissionsAsync(
                    Guid.NewGuid(),
                    sourcePermissions,
                    destinationPermissions,
                    Cancel);

                Assert.True(result.Success);
            }

            [Fact]
            public async Task Success_with_no_destination_permissions()
            {
                var sourcePermissions = Create<IPermissions>();
                var destinationPermissions = Create<IPermissions>();
                destinationPermissions.GranteeCapabilities = Array.Empty<IGranteeCapability>();

                SetupDeletePermissionsAsync(true);

                var result = await PermissionsClient.DeletePermissionsAsync(
                    Guid.NewGuid(),
                    sourcePermissions,
                    destinationPermissions,
                    Cancel);

                Assert.True(result.Success);
            }

            [Fact]
            public async Task Fails_upon_delete_capability_fail()
            {
                var sourcePermissions = Create<IPermissions>();
                var destinationPermissions = Create<IPermissions>();

                SetupDeleteCapabilityAsync(false);
                SetupGetCapabilitiesToDelete(destinationPermissions.GranteeCapabilities);

                var result = await PermissionsClient.DeletePermissionsAsync(
                    Guid.NewGuid(),
                    sourcePermissions,
                    destinationPermissions,
                    Cancel);

                Assert.False(result.Success);
                Assert.True(result.Errors.Count > 0);
            }
        }

        public class UpdatePermissionsAsync : PermissionsApiClientTest
        {
            public UpdatePermissionsAsync()
            { }

            [Fact]
            public async Task Success()
            {
                var sourcePermissions = Create<IPermissions>();
                var destinationPermissions = Create<IPermissions>();

                SetupGetPermissionsAsync(true, destinationPermissions);
                SetupGetCapabilitiesToDelete(destinationPermissions.GranteeCapabilities);
                SetupDeleteCapabilityAsync(true);
                SetupCreatePermissionsAsync(true, destinationPermissions);

                var result = await PermissionsClient.UpdatePermissionsAsync(
                    Guid.NewGuid(),
                    sourcePermissions,
                    Cancel);

                Assert.True(result.Success);

                // Get permissions is called once.
                VerifyGetPermissionsAsync(Times.Once());

                // Delete capabilities is called the same number of times as
                // the count of destination capabilities.
                var capabilityCount = destinationPermissions.GranteeCapabilities.SelectMany(gc => gc.Capabilities).Count();
                VerifyDeleteCapabilityAsync(Times.Exactly(capabilityCount));

                // Create permissions is called once.
                VerifyCreatePermissionsAsync(Times.Once());
            }

            [Fact]
            public async Task Fails_when_get_permissions_fails()
            {
                var sourcePermissions = Create<IPermissions>();
                var destinationPermissions = Create<IPermissions>();

                SetupGetPermissionsAsync(false);
                SetupDeletePermissionsAsync(true);
                SetupCreatePermissionsAsync(true, destinationPermissions);

                var result = await PermissionsClient.UpdatePermissionsAsync(
                    Guid.NewGuid(),
                    sourcePermissions,
                    Cancel);

                Assert.False(result.Success);

                // Get permissions is called once.
                VerifyGetPermissionsAsync(Times.Once());

                // Delete capability is never called.
                VerifyDeleteCapabilityAsync(Times.Never());

                // Create permissions is never called.
                VerifyCreatePermissionsAsync(Times.Never());
            }
        }
    }
}
