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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Content
{
    internal sealed class CloudSubscription : SubscriptionBase<ICloudSchedule>, ICloudSubscription
    {
        public CloudSubscription(ISubscriptionType subscription, IContentReference user, ICloudScheduleType schedule)
            : base(subscription, user, new CloudSchedule(schedule))
        { }

        public CloudSubscription(Guid id, string? subject, bool attachImage, bool attachPdf,
            string? pageOrientation, string? pageSizeOption, bool suspended, string? message,
            ISubscriptionContent content, IContentReference user, ICloudSchedule schedule)
            : base(id, subject, attachImage, attachPdf, pageOrientation, pageSizeOption, suspended, message, content,
                  user, schedule)
        { }

        public static async Task<IImmutableList<ICloudSubscription>> CreateManyAsync(
            GetSubscriptionsResponse response,
            IContentReferenceFinderFactory finderFactory,
            ILogger logger, ISharedResourcesLocalizer localizer,
            CancellationToken cancel)
            => await CreateManyAsync(
                response,
                response => response.Items.ExceptNulls(),
                (r, u, cnl) => Task.FromResult<ICloudSubscription>(new CloudSubscription(r, u, Guard.AgainstNull(r.Schedule, () => r.Schedule))),
                finderFactory, logger, localizer,
                cancel).ConfigureAwait(false);
    }
}
