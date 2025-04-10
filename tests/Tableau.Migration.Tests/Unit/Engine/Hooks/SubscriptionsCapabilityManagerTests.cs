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
using System.Collections.Immutable;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks
{
    public class SubscriptionsCapabilityManagerTests : AutoFixtureTestBase
    {
        internal readonly SubscriptionsCapabilityManager SubscriptionsCapabilityManager;
        internal readonly MigrationCapabilities MigrationCapabilities = new();
        internal readonly Mock<IScheduleValidator<ICloudSchedule>> MockScheduleValidator = new();
        internal readonly Mock<IDestinationApiEndpoint> MockDestinationEndpoint = new();
        internal readonly Mock<ISharedResourcesLocalizer> MockLocalizer = new();
        internal readonly Mock<ILogger<SubscriptionsCapabilityManager>> MockLogger = new();

        public SubscriptionsCapabilityManagerTests()
        {
            SubscriptionsCapabilityManager = new SubscriptionsCapabilityManager(
                MockDestinationEndpoint.Object, MockScheduleValidator.Object, MigrationCapabilities,
                MockLocalizer.Object, MockLogger.Object);
        }

        protected void SetupDeleteCloudSubscription()
        {
            MockDestinationEndpoint.Setup(de
                => de.DeleteAsync<ICloudSubscription>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((IResult)Result.Succeeded()));
        }

        protected void SetupCreateSubscriptionSuccess(ICloudSubscription subscription)
        {
            MockDestinationEndpoint.Setup(de
                => de.PublishAsync<ICloudSubscription, ICloudSubscription>(
                    It.IsAny<ICloudSubscription>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((IResult<ICloudSubscription>)Result<ICloudSubscription>.Succeeded(subscription)));
        }

        protected void SetupCreateSubscriptionFailure(Exception exception)
        {
            MockDestinationEndpoint.Setup(de
                => de.PublishAsync<ICloudSubscription, ICloudSubscription>(
                    It.IsAny<ICloudSubscription>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((IResult<ICloudSubscription>)Result<ICloudSubscription>.Failed(exception)));
        }

        protected void SetupDestinationPager(ImmutableArray<IWorkbook> workbooks)
        {
            MockDestinationEndpoint.Setup(de => de.GetPager<IWorkbook>(It.IsAny<int>()))
                .Returns((int pageSize) => new MemoryPager<IWorkbook>(workbooks, pageSize));
        }

        private void SetupScheduleValidator()
        {
            MockScheduleValidator.Setup(sv => sv.Validate(It.IsAny<ICloudSchedule>())).Verifiable();
        }

        public class SetMigrationCapabilityAsync : SubscriptionsCapabilityManagerTests
        {
            [Fact]
            public async Task True_when_subscriptions_enabled()
            {
                var workbooks = AutoFixture.CreateMany<IWorkbook>().ToImmutableArray();
                var subscription = AutoFixture.Create<ICloudSubscription>();

                SetupScheduleValidator();

                SetupDestinationPager(workbooks);

                SetupCreateSubscriptionSuccess(subscription);

                SetupDeleteCloudSubscription();

                var result = await SubscriptionsCapabilityManager.SetMigrationCapabilityAsync(new CancellationToken());

                Assert.NotNull(result);
                Assert.Empty(MigrationCapabilities.ContentTypesDisabledAtDestination);
                Assert.DoesNotContain(typeof(IServerSubscription), MigrationCapabilities.ContentTypesDisabledAtDestination);

            }

            [Fact]
            public async Task False_when_subscriptions_disabled()
            {
                var workbooks = AutoFixture.CreateMany<IWorkbook>().ToImmutableArray();
                var subscription = AutoFixture.Create<ICloudSubscription>();

                SetupScheduleValidator();

                SetupDestinationPager(workbooks);

                var error = new RestException(
                    HttpMethod.Post,
                    new Uri("http://dummy/uri"),
                    Guid.NewGuid().ToString(),
                    new Migration.Api.Rest.Models.Error() { Code = RestErrorCodes.GENERIC_CREATE_SUBSCRIPTION_ERROR },
                    string.Empty,
                    "Subscriptions not enabled");

                SetupCreateSubscriptionFailure(error);

                SetupDeleteCloudSubscription();

                var result = await SubscriptionsCapabilityManager.SetMigrationCapabilityAsync(new CancellationToken());

                Assert.NotNull(result);
                Assert.NotEmpty(MigrationCapabilities.ContentTypesDisabledAtDestination);
                Assert.Contains(typeof(IServerSubscription), MigrationCapabilities.ContentTypesDisabledAtDestination);

            }

        }
    }
}
