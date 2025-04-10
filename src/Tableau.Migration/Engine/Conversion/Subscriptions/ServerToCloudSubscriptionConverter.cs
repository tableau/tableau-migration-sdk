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

using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Engine.Conversion.Schedules;

namespace Tableau.Migration.Engine.Conversion.Subscriptions
{
    internal class ServerToCloudSubscriptionConverter
        : SubscriptionConverterBase<IServerSubscription, IServerSchedule, ICloudSubscription, ICloudSchedule>
    {
        public ServerToCloudSubscriptionConverter(IScheduleConverter<IServerSchedule, ICloudSchedule> scheduleConverter)
            : base(scheduleConverter)
        { }

        protected override ICloudSubscription ConvertSubscription(IServerSubscription source, ICloudSchedule targetSchedule)
            => new CloudSubscription(
                source.Id, source.Subject, source.AttachImage, source.AttachPdf, source.PageOrientation,
                source.PageSizeOption, source.Suspended, source.Message, source.Content, source.Owner, targetSchedule);
    }
}
