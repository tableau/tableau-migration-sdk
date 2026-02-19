//
//  Copyright (c) 2026, Salesforce, Inc.
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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using Tableau.Migration.Content.Schedules.Cloud;

namespace Tableau.Migration.Content
{
    internal sealed class CloudSubscription : SubscriptionBase<ICloudSchedule>, ICloudSubscription
    {
        public CloudSubscription(ISubscriptionType subscription, IContentReference user, ICloudScheduleType schedule, IContentReference contentReference)
            : base(subscription, user, new CloudSchedule(schedule), contentReference)
        { }

        public CloudSubscription(Guid id, string? subject, bool attachImage, bool attachPdf,
            string? pageOrientation, string? pageSizeOption, bool suspended, string? message,
            ISubscriptionContent content, IContentReference user, ICloudSchedule schedule)
            : base(id, subject, attachImage, attachPdf, pageOrientation, pageSizeOption, suspended, message, content,
                  user, schedule)
        { }
    }
}
