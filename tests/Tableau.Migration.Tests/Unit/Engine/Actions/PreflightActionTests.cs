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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.InitializeMigration;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Actions
{
    public class PreflightActionTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            protected PreflightOptions Options { get; }

            protected Mock<IServerSession> MockSourceSession { get; set; }

            protected Func<IResult<IServerSession>> SourceSessionResult { get; set; }

            protected Mock<ISite> MockSourceSite { get; set; }

            protected Func<IResult<ISite>> SourceSiteResult { get; set; }

            protected Mock<ISourceEndpoint> MockSource { get; set; }

            protected Mock<IServerSession> MockDestinationSession { get; set; }

            protected Func<IResult<IServerSession>> DestinationSessionResult { get; set; }

            protected Mock<ISite> MockDestinationSite { get; set; }

            protected Func<IResult<ISite>> DestinationSiteResult { get; set; }

            protected Mock<IDestinationEndpoint> MockDestination { get; set; }

            protected IResult<ISite> UpdateResult { get; set; }

            protected Mock<ILogger<PreflightAction>> MockLogger { get; }

            protected Mock<IMigrationHookRunner> MockHookRunner { get; }

            public ExecuteAsync()
            {
                Options = Freeze<PreflightOptions>();
                Options.ValidateSettings = PreflightOptions.Defaults.VALIDATE_SETTINGS;

                MockSourceSession = Create<Mock<IServerSession>>();
                MockSourceSession.SetupGet(x => x.IsAdministrator).Returns(true);

                SourceSessionResult = () => Result<IServerSession>.Succeeded(MockSourceSession.Object);

                MockSourceSite = Create<Mock<ISite>>();
                SourceSiteResult = () => Result<ISite>.Succeeded(MockSourceSite.Object);

                MockSource = Freeze<Mock<ISourceEndpoint>>();
                MockSource.Setup(x => x.GetSessionAsync(Cancel)).ReturnsAsync(() => SourceSessionResult());
                MockSource.Setup(x => x.GetCurrentSiteAsync(Cancel)).ReturnsAsync(() => SourceSiteResult());

                MockDestinationSession = Create<Mock<IServerSession>>();
                MockDestinationSession.SetupGet(x => x.IsAdministrator).Returns(true);

                DestinationSessionResult = () => Result<IServerSession>.Succeeded(MockDestinationSession.Object);

                MockDestinationSite = Create<Mock<ISite>>();
                DestinationSiteResult = () => Result<ISite>.Succeeded(MockDestinationSite.Object);

                MockDestination = Freeze<Mock<IDestinationEndpoint>>();
                MockDestination.Setup(x => x.GetSessionAsync(Cancel)).ReturnsAsync(() => DestinationSessionResult());
                MockDestination.Setup(x => x.GetCurrentSiteAsync(Cancel)).ReturnsAsync(() => DestinationSiteResult());

                UpdateResult = Result<ISite>.Succeeded(Create<ISite>());
                MockDestination.Setup(x => x.UpdateSiteSettingsAsync(It.IsAny<ISiteSettingsUpdate>(), Cancel))
                    .ReturnsAsync(() => UpdateResult);

                MockLogger = Freeze<Mock<ILogger<PreflightAction>>>();

                MockHookRunner = Freeze<Mock<IMigrationHookRunner>>();
                MockHookRunner.Setup(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel))
                    .ReturnsAsync((IInitializeMigrationHookResult ctx, CancellationToken cancel) => ctx);
            }

            [Fact]
            public async Task SettingValidationDisabledAsync()
            {
                Options.ValidateSettings = false;

                var action = Create<PreflightAction>();

                var result = await action.ExecuteAsync(Cancel);

                result.AssertSuccess();
                Assert.True(result.PerformNextAction);

                MockSource.Verify(x => x.GetSessionAsync(Cancel), Times.Once);
                MockDestination.Verify(x => x.GetSessionAsync(Cancel), Times.Once);

                MockLogger.VerifyWarnings(Times.Never);

                MockHookRunner.Verify(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel), Times.Once);
            }

            [Fact]
            public async Task SourceSessionFailedAsync()
            {
                SourceSessionResult = () => Result<IServerSession>.Failed(new Exception());

                var action = Create<PreflightAction>();

                var result = await action.ExecuteAsync(Cancel);

                result.AssertFailure();
                Assert.False(result.PerformNextAction);

                MockSource.Verify(x => x.GetSessionAsync(Cancel), Times.Once);
                MockDestination.Verify(x => x.GetSessionAsync(Cancel), Times.Once);

                MockLogger.VerifyWarnings(Times.Never);

                MockHookRunner.Verify(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel), Times.Never);
            }

            [Fact]
            public async Task DestinationSessionFailedAsync()
            {
                DestinationSessionResult = () => Result<IServerSession>.Failed(new Exception());

                var action = Create<PreflightAction>();

                var result = await action.ExecuteAsync(Cancel);

                result.AssertFailure();
                Assert.False(result.PerformNextAction);

                MockSource.Verify(x => x.GetSessionAsync(Cancel), Times.Once);
                MockDestination.Verify(x => x.GetSessionAsync(Cancel), Times.Once);

                MockLogger.VerifyWarnings(Times.Never);

                MockHookRunner.Verify(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel), Times.Never);
            }

            [Fact]
            public async Task SourceSiteFailedAsync()
            {
                SourceSiteResult = () => Result<ISite>.Failed(new Exception());

                var action = Create<PreflightAction>();

                var result = await action.ExecuteAsync(Cancel);

                result.AssertFailure();
                Assert.False(result.PerformNextAction);

                MockSource.Verify(x => x.GetSessionAsync(Cancel), Times.Once);
                MockDestination.Verify(x => x.GetSessionAsync(Cancel), Times.Once);

                MockLogger.VerifyWarnings(Times.Never);

                MockHookRunner.Verify(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel), Times.Never);
            }

            [Fact]
            public async Task DestinationSiteFailedAsync()
            {
                DestinationSiteResult = () => Result<ISite>.Failed(new Exception());

                var action = Create<PreflightAction>();

                var result = await action.ExecuteAsync(Cancel);

                result.AssertFailure();
                Assert.False(result.PerformNextAction);

                MockSource.Verify(x => x.GetSessionAsync(Cancel), Times.Once);
                MockDestination.Verify(x => x.GetSessionAsync(Cancel), Times.Once);

                MockLogger.VerifyWarnings(Times.Never);

                MockHookRunner.Verify(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel), Times.Never);
            }

            [Fact]
            public async Task CombinesEndpointErrorsAsync()
            {
                var sourceEx = new Exception();
                var destinationEx = new Exception();

                SourceSessionResult = () => Result<IServerSession>.Failed(sourceEx);
                DestinationSessionResult = () => Result<IServerSession>.Failed(destinationEx);

                var action = Create<PreflightAction>();

                var result = await action.ExecuteAsync(Cancel);

                result.AssertFailure();
                Assert.False(result.PerformNextAction);

                Assert.Contains(sourceEx, result.Errors);
                Assert.Contains(destinationEx, result.Errors);

                MockSource.Verify(x => x.GetSessionAsync(Cancel), Times.Once);
                MockDestination.Verify(x => x.GetSessionAsync(Cancel), Times.Once);

                MockLogger.VerifyWarnings(Times.Never);

                MockHookRunner.Verify(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel), Times.Never);
            }

            [Fact]
            public async Task NotSourceAdminAsync()
            {
                MockSourceSession.SetupGet(x => x.IsAdministrator).Returns(false);

                var action = Create<PreflightAction>();

                var result = await action.ExecuteAsync(Cancel);

                result.AssertSuccess();
                Assert.True(result.PerformNextAction);

                MockSource.Verify(x => x.GetSessionAsync(Cancel), Times.Once);
                MockDestination.Verify(x => x.GetSessionAsync(Cancel), Times.Once);

                MockLogger.VerifyWarnings(Times.Never);

                MockHookRunner.Verify(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel), Times.Once);
            }

            [Fact]
            public async Task NotDestinationAdminAsync()
            {
                MockDestinationSession.SetupGet(x => x.IsAdministrator).Returns(false);

                var action = Create<PreflightAction>();

                var result = await action.ExecuteAsync(Cancel);

                result.AssertSuccess();
                Assert.True(result.PerformNextAction);

                MockSource.Verify(x => x.GetSessionAsync(Cancel), Times.Once);
                MockDestination.Verify(x => x.GetSessionAsync(Cancel), Times.Once);

                MockLogger.VerifyWarnings(Times.Never);

                MockHookRunner.Verify(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel), Times.Once);
            }

            [Fact]
            public async Task ValidSameSettingAsync()
            {
                MockSourceSession.SetupGet(x => x.Settings!.ExtractEncryptionMode).Returns(ExtractEncryptionModes.Enabled);
                MockDestinationSession.SetupGet(x => x.Settings!.ExtractEncryptionMode).Returns(ExtractEncryptionModes.Enabled);

                var action = Create<PreflightAction>();

                var result = await action.ExecuteAsync(Cancel);

                result.AssertSuccess();
                Assert.True(result.PerformNextAction);

                MockSource.Verify(x => x.GetSessionAsync(Cancel), Times.Once);
                MockDestination.Verify(x => x.GetSessionAsync(Cancel), Times.Once);

                MockLogger.VerifyWarnings(Times.Never);

                MockHookRunner.Verify(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel), Times.Once);
            }

            [Fact]
            public async Task ExtractEncryptionValidSourceDisabledAsync()
            {
                MockSourceSession.SetupGet(x => x.Settings!.ExtractEncryptionMode).Returns(ExtractEncryptionModes.Disabled);
                MockDestinationSession.SetupGet(x => x.Settings!.ExtractEncryptionMode).Returns(ExtractEncryptionModes.Enabled);

                var action = Create<PreflightAction>();

                var result = await action.ExecuteAsync(Cancel);

                result.AssertSuccess();
                Assert.True(result.PerformNextAction);

                MockSource.Verify(x => x.GetSessionAsync(Cancel), Times.Once);
                MockDestination.Verify(x => x.GetSessionAsync(Cancel), Times.Once);

                MockLogger.VerifyWarnings(Times.Never);

                MockHookRunner.Verify(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel), Times.Once);
            }

            [Fact]
            public async Task ExtractEncryptionValidCompatibleAsync()
            {
                MockSourceSession.SetupGet(x => x.Settings!.ExtractEncryptionMode).Returns(ExtractEncryptionModes.Enforced);
                MockDestinationSession.SetupGet(x => x.Settings!.ExtractEncryptionMode).Returns(ExtractEncryptionModes.Enabled);

                var action = Create<PreflightAction>();

                var result = await action.ExecuteAsync(Cancel);

                result.AssertSuccess();
                Assert.True(result.PerformNextAction);

                MockSource.Verify(x => x.GetSessionAsync(Cancel), Times.Once);
                MockDestination.Verify(x => x.GetSessionAsync(Cancel), Times.Once);

                MockLogger.VerifyWarnings(Times.Never);

                MockHookRunner.Verify(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel), Times.Once);
            }

            [Fact]
            public async Task InitializeHookFailsAsync()
            {
                MockHookRunner.Setup(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel))
                    .ReturnsAsync((IInitializeMigrationHookResult ctx, CancellationToken cancel) => ctx.ToFailure(new Exception()));

                var action = Create<PreflightAction>();

                var result = await action.ExecuteAsync(Cancel);

                result.AssertFailure();
                Assert.False(result.PerformNextAction);

                MockSource.Verify(x => x.GetSessionAsync(Cancel), Times.Once);
                MockDestination.Verify(x => x.GetSessionAsync(Cancel), Times.Once);

                MockLogger.VerifyWarnings(Times.Never);

                MockHookRunner.Verify(x => x.ExecuteAsync<IInitializeMigrationHook, IInitializeMigrationHookResult>(It.IsAny<IInitializeMigrationHookResult>(), Cancel), Times.Once);
            }
        }
    }
}
