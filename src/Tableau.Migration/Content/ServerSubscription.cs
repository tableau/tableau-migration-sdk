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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Content
{
    internal sealed class ServerSubscription : SubscriptionBase<IServerSchedule>, IServerSubscription
    {
        public ServerSubscription(GetSubscriptionsResponse.SubscriptionType sub, IContentReference user, IServerSchedule schedule)
            : base(sub, user, schedule)
        { }

        private static async Task<IServerSubscription> CreateAsync(
            GetSubscriptionsResponse.SubscriptionType sub,
            IContentReference user,
            IContentCacheFactory contentCacheFactory,
            Func<Guid, CancellationToken, Task<IResult<IServerSchedule>>> getScheduleById,
            CancellationToken cancel)
        {
            var scheduleCache = contentCacheFactory.ForContentType<IServerSchedule>(true);

            var scheduleId = sub.Schedule.Id;

            var schedule = await scheduleCache.ForIdAsync(scheduleId, cancel).ConfigureAwait(false);

            if (schedule == null)
            {

                var getScheduleResult = await getScheduleById(scheduleId, cancel)
                    .ConfigureAwait(false);

                if (!getScheduleResult.Success)
                {
                    throw new InvalidOperationException($"A schedule could not be fetched for Server Subscription. {sub.Id}");
                }

                schedule = getScheduleResult.Value;
                scheduleCache.AddOrUpdate(schedule);
            }

            Guard.AgainstNull(schedule, nameof(schedule));

            return new ServerSubscription(sub, user, schedule);
        }

        public static async Task<IImmutableList<IServerSubscription>> CreateManyAsync(
            GetSubscriptionsResponse response,
            IContentReferenceFinderFactory finderFactory,
            IContentCacheFactory contentCacheFactory,
            Func<System.Guid, CancellationToken, Task<IResult<IServerSchedule>>> getScheduleById,
            ILogger logger, ISharedResourcesLocalizer localizer,
            CancellationToken cancel)
            => await CreateManyAsync(
                        response,
                        response => response.Items.ExceptNulls(),
                        async (r, u, cnl) => await CreateAsync(r, u, contentCacheFactory, getScheduleById, cnl).ConfigureAwait(false),
                        finderFactory, logger, localizer,
                        cancel).ConfigureAwait(false);
    }
}
