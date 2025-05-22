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
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Resources;
using Xunit;


namespace Tableau.Migration.Tests.Unit.Engine.Hooks
{
    public class EmbeddedCredentialsCapabilityManagerTests : AutoFixtureTestBase
    {
        private readonly Mock<ISharedResourcesLocalizer> _mockLocalizer = new();
        internal readonly Mock<ILogger<EmbeddedCredentialsCapabilityManager>> MockLogger = new();
        internal Mock<MigrationCapabilities> MockMigrationCapabilities;
        protected readonly Mock<IMigration> MockMigration;

        internal readonly EmbeddedCredentialsCapabilityManager EmbeddedCredsCapabilityManager;

        public EmbeddedCredentialsCapabilityManagerTests()
        {
            MockMigrationCapabilities = new Mock<MigrationCapabilities> { CallBase = true };
            MockMigration = new Mock<IMigration>();

            MockMigration.SetupGet(m => m.Plan.Destination).Returns(Create<ITableauApiEndpointConfiguration>());
            MockMigration.Setup(m => m.Destination.GetSessionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((IResult<IServerSession>)Result<IServerSession>.Succeeded(Create<IServerSession>())));

            EmbeddedCredsCapabilityManager = new EmbeddedCredentialsCapabilityManager(
                _mockLocalizer.Object,
                MockLogger.Object,
                MockMigrationCapabilities.Object,
                MockMigration.Object);
        }

        private void SetupRetrieveKeychainAsync(IEmbeddedCredentialKeychainResult keychainResult)
        {
            MockMigration.Setup(m => m.Source.RetrieveKeychainsAsync<IDataSource>(
                It.IsAny<Guid>(),
                It.IsAny<IDestinationSiteInfo>(),
                It.IsAny<CancellationToken>()))
                .Returns(
                Task.FromResult(
                    (IResult<IEmbeddedCredentialKeychainResult>)Result<IEmbeddedCredentialKeychainResult>
                    .Succeeded(keychainResult)));
        }

        private void SetupRetrieveKeychainAsyncFail(Exception error)
        {
            MockMigration.Setup(m => m.Source.RetrieveKeychainsAsync<IDataSource>(
                It.IsAny<Guid>(),
                It.IsAny<IDestinationSiteInfo>(),
                It.IsAny<CancellationToken>()))
                .Returns(
                Task.FromResult(
                    (IResult<IEmbeddedCredentialKeychainResult>)Result<IEmbeddedCredentialKeychainResult>
                    .Failed(error)));
        }

        protected void SetupRetrieveKeychainAsync(IEnumerable<string> encryptedKeychains)
        {
            var response = new RetrieveKeychainResponse(encryptedKeychains, null);
            SetupRetrieveKeychainAsync(new EmbeddedCredentialKeychainResult(response));
        }

        public class SetMigrationCapabilityAsync : EmbeddedCredentialsCapabilityManagerTests
        {
            [Fact]
            public async Task Doesnot_disable_when_embedded_creds_setup()
            {
                // Arrange
                SetupRetrieveKeychainAsync(CreateMany<string>());

                // Act
                var result = await EmbeddedCredsCapabilityManager.SetMigrationCapabilityAsync(Create<IServerSession>(), new CancellationToken());

                // Assert
                Assert.True(result.Success);
                Assert.False(MockMigrationCapabilities.Object.EmbeddedCredentialsDisabled);
                MockLogger.VerifyWarnings(Times.Never());
            }

            [Fact]
            public async Task Disables_when_embedded_creds_not_setup()
            {
                // Arrange
                var restException = CreateRestException(RestErrorCodes.FEATURE_DISABLED);
                SetupRetrieveKeychainAsyncFail(restException);

                // Act
                var result = await EmbeddedCredsCapabilityManager.SetMigrationCapabilityAsync(Create<IServerSession>(), new CancellationToken());

                // Assert
                Assert.True(result.Success);
                Assert.True(MockMigrationCapabilities.Object.EmbeddedCredentialsDisabled);
                MockLogger.VerifyWarnings(Times.Once);
            }

            [Fact]
            public async Task Doesnot_disable_on_other_errors()
            {
                // Arrange
                var restException = CreateRestException(RestErrorCodes.BAD_REQUEST);
                SetupRetrieveKeychainAsyncFail(restException);

                // Act
                var result = await EmbeddedCredsCapabilityManager.SetMigrationCapabilityAsync(Create<IServerSession>(), new CancellationToken());

                // Assert
                Assert.True(result.Success);
                Assert.False(MockMigrationCapabilities.Object.EmbeddedCredentialsDisabled);
                MockLogger.VerifyWarnings(Times.Never());
            }

            private RestException CreateRestException(string errorCode)
            {
                var error = AutoFixture.Build<Error>()
                    .With(e => e.Code, errorCode)
                    .Create();

                var restException = new RestException(
                    httpMethod: HttpMethod.Get,
                    Create<Uri>(),
                    Create<string>(),
                    error, Create<string>(), Create<ISharedResourcesLocalizer>());
                return restException;
            }
        }
    }
}
