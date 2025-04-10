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
using System.Text;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Content.Schedules
{
    internal sealed class FrequencyDetails : IFrequencyDetails
    {
        public TimeOnly? StartAt { get; set; }
        public TimeOnly? EndAt { get; set; }

        public IList<IInterval> Intervals { get; set; }

        public FrequencyDetails(IScheduleFrequencyDetailsType response)
            : this(response.Start.ToTimeOrNull(), response.End.ToTimeOrNull(), response.Intervals.Select(i => new Interval(i) as IInterval))
        { }

        public FrequencyDetails(TimeOnly? startAt, TimeOnly? endAt, params IEnumerable<IInterval> intervals)
        {
            StartAt = startAt;
            EndAt = endAt;
            Intervals = intervals.ToList();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"StartAt: {StartAt}");
            sb.AppendLine($"EndAt: {EndAt}");
            sb.AppendLine($"Interval Count {Intervals.Count()}:");
            sb.AppendLine(string.Join(Environment.NewLine, Intervals.Select(i => i.ToString())));
            return sb.ToString();
        }
    }
}
