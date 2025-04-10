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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks
{
    internal class SubscriptionsCapabilityManager : MigrationCapabilityManagerBase, ISubscriptionsCapabilityManager
    {
        private const string DUMMY_SUBSCRIPTION_NAME = "Migration SDK Dummy Subscription";
        private const string DUMMY_SUBSCRIPTION_MESSAGE = "This is a dummy subscription created by the Migration SDK. You can safely delete it.";
        private const string DUMMY_SCHEDULE_START_TIME = "00:00:00";
        private const string DUMMY_SCHEDULE_END_TIME = "1:00:00";

        private readonly IDestinationEndpoint _destinationEndpoint;
        private readonly IScheduleValidator<ICloudSchedule> _scheduleValidator;

        public SubscriptionsCapabilityManager(
            IDestinationEndpoint destinationEndpoint,
            IScheduleValidator<ICloudSchedule> scheduleValidator,
            IMigrationCapabilitiesEditor capabilities,
            ISharedResourcesLocalizer localizer,
            ILogger<SubscriptionsCapabilityManager> logger) : base(localizer, logger, capabilities)
        {
            _destinationEndpoint = destinationEndpoint;
            _scheduleValidator = scheduleValidator;
        }

        /// <inheritdoc/>
        public override bool IsMigrationCapabilityDisabled()
        {
            return CapabilitiesEditor.ContentTypesDisabledAtDestination.Contains(typeof(IServerSubscription));
        }

        /// <inheritdoc/>
        public override async Task<IResult> SetMigrationCapabilityAsync(CancellationToken cancel)
        {
            var workbook = await GetRandomWorkbook(cancel).ConfigureAwait(false);

            if (workbook is null)
            {
                return Result.Succeeded();
            }

            var createResult = await CreateDummySubscription(workbook, cancel).ConfigureAwait(false);

            if (createResult.Success)
            {
                var deleteResult = await DeleteDummySubscription(createResult.Value.Id, cancel).ConfigureAwait(false);

                if (deleteResult.Success)
                {
                    return Result.Succeeded();
                }

                var errors = new List<Exception>()
                {
                    new($"Could not delete dummy subscription {DUMMY_SUBSCRIPTION_NAME} in {nameof(SubscriptionsCapabilityManager)}")
                };
                errors.AddRange(deleteResult.Errors);

                return Result.Failed(errors);
            }

            if (SubscriptionsDisabled(createResult))
            {
                CapabilitiesEditor.ContentTypesDisabledAtDestination.Add(typeof(IServerSubscription));
                LogCapabilityDisabled(typeof(IServerSubscription).GetFormattedName(), Localizer[SharedResourceKeys.SubscriptionsDisabledReason]);
                return Result.Succeeded();
            }
            else
            {
                return Result.Failed(createResult.Errors);
            }
        }

        private async Task<IWorkbook?> GetRandomWorkbook(CancellationToken cancel)
        {
            var destinationPager = _destinationEndpoint.GetPager<IWorkbook>(1);

            var destinationPage = await destinationPager.NextPageAsync(cancel).ConfigureAwait(false);

            var workbook = destinationPage.Value?.FirstOrDefault();
            return workbook;
        }

        private async Task<IResult> DeleteDummySubscription(Guid dummySubscriptionId, CancellationToken cancel)
        {
            return await _destinationEndpoint
                    .DeleteAsync<ICloudSubscription>(dummySubscriptionId, cancel)
                    .ConfigureAwait(false);
        }

        private async Task<IResult<ICloudSubscription>> CreateDummySubscription(IWorkbook workbook, CancellationToken cancel)
        {
            var dummySubscription = new CloudSubscription(
                id: Guid.NewGuid(),
                subject: DUMMY_SUBSCRIPTION_NAME,
                attachImage: true,
                attachPdf: false,
                pageOrientation: null,
                pageSizeOption: null,
                suspended: true,
                message: DUMMY_SUBSCRIPTION_MESSAGE,
                content: new SubscriptionContent(workbook.Id, "Workbook", false),
                user: workbook.Owner,
                schedule: CreateDummySchedule());

            return await _destinationEndpoint.PublishAsync<ICloudSubscription, ICloudSubscription>(
                dummySubscription,
                cancel)
                .ConfigureAwait(false);
        }

        private CloudSchedule CreateDummySchedule()
        {
            var result = new CloudSchedule(
                ScheduleFrequencies.Daily,
                new FrequencyDetails(
                    startAt: TimeOnly.Parse(DUMMY_SCHEDULE_START_TIME),
                    endAt: TimeOnly.Parse(DUMMY_SCHEDULE_END_TIME),
                    intervals: [Interval.WithHours(24), Interval.WithWeekday(WeekDays.Tuesday)]));

            _scheduleValidator.Validate(result);

            return result;
        }

        private static bool SubscriptionsDisabled(IResult<ICloudSubscription> createSubscriptionResult)
        {
            return createSubscriptionResult.Errors
                .Where(e => e is RestException)
                .Select(e => e as RestException)
                .Any(e => RestErrorCodes.Equals(e?.Code, RestErrorCodes.GENERIC_CREATE_SUBSCRIPTION_ERROR));
        }
    }
}
