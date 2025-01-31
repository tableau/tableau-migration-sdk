﻿//
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

using Tableau.Migration.Api.Rest.Models.Responses.Cloud;

namespace Tableau.Migration.Content.Schedules.Cloud
{
    internal sealed class CloudSchedule : ScheduleBase, ICloudSchedule
    {
        public CloudSchedule(ICloudScheduleType response)
            : base(response)
        { }

        public CloudSchedule(string frequency, IFrequencyDetails frequencyDetails)
            : base(frequency, frequencyDetails, null)
        { }

        public CloudSchedule(ISchedule schedule)
            : base(schedule.Frequency, schedule.FrequencyDetails, schedule.NextRunAt)
        { }
    }
}