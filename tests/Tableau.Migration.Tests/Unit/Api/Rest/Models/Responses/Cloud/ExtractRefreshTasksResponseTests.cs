//
//  Copyright (c) 2024, Salesforce, Inc.
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

using System.Linq;
using Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using Tableau.Migration.Content.Schedules;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Responses.Cloud
{
    public class ExtractRefreshTasksResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Serializes()
            {
                var expectedResponse = Create<ExtractRefreshTasksResponse>();
                var expectedResponseItem = expectedResponse.Items.First();

                var expectedExtractRefresh = expectedResponseItem.ExtractRefresh;
                Assert.NotNull(expectedExtractRefresh);

                expectedExtractRefresh.DataSource = null;
                expectedExtractRefresh.Workbook = Create<ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.WorkbookType>();

                var expectedWorkBook = expectedExtractRefresh.Workbook;
                Assert.NotNull(expectedWorkBook);

                var expectedSchedule = expectedExtractRefresh.Schedule;
                Assert.NotNull(expectedSchedule);

                var expectedFrequencyDetails = expectedSchedule.FrequencyDetails;
                Assert.NotNull(expectedFrequencyDetails);

                var interval = new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.ScheduleType.FrequencyDetailsType.IntervalType
                {
                    WeekDay = IntervalValues.WeekDaysValues.ExceptNulls().PickRandom()
                };

                expectedFrequencyDetails.Intervals = [interval];

                var xml = @$"
<tsResponse>
  <tasks>
    <task>
      <extractRefresh 
        id=""{expectedExtractRefresh.Id}"" 
        priority=""{expectedExtractRefresh.Priority}"" 
        consecutiveFailedCount=""{expectedExtractRefresh.ConsecutiveFailedCount}"" 
        type=""RefreshExtractTask"">
        <schedule frequency=""{expectedSchedule.Frequency}"" nextRunAt=""{expectedSchedule.NextRunAt}"">
          <frequencyDetails start=""{expectedFrequencyDetails.Start}"" end=""{expectedFrequencyDetails.End}"">  
            <intervals>
              <interval weekDay=""{expectedFrequencyDetails.Intervals[0].WeekDay}""/>
            </intervals>
          </frequencyDetails>
        </schedule>
        <workbook id=""{expectedWorkBook.Id}""/>
      </extractRefresh>
    </task>
  </tasks>
</tsResponse>
";
                var deserialized = Serializer.DeserializeFromXml<ExtractRefreshTasksResponse>(xml);
                Assert.NotNull(deserialized);
                Assert.Single(deserialized.Items);

                var actualExtractRefresh = deserialized.Items[0].ExtractRefresh;
                Assert.NotNull(actualExtractRefresh);
                Assert.Equal(expectedExtractRefresh.Id, actualExtractRefresh.Id);
                Assert.Equal(expectedExtractRefresh.Priority, actualExtractRefresh.Priority);
                Assert.Equal(expectedExtractRefresh.ConsecutiveFailedCount, actualExtractRefresh.ConsecutiveFailedCount);

                var actualSchedule = actualExtractRefresh.Schedule;
                Assert.NotNull(actualSchedule);
                Assert.Equal(expectedSchedule.Frequency, actualSchedule.Frequency);
                Assert.Equal(expectedSchedule.NextRunAt, actualSchedule.NextRunAt);

                var actualFrequencyDetails= actualSchedule.FrequencyDetails;
                Assert.NotNull(actualFrequencyDetails);
                Assert.Equal(expectedFrequencyDetails.Start, actualFrequencyDetails.Start);
                Assert.Equal(expectedFrequencyDetails.End, actualFrequencyDetails.End);
                Assert.Single(actualFrequencyDetails.Intervals);
                Assert.Equal(expectedFrequencyDetails.Intervals[0].WeekDay, actualFrequencyDetails.Intervals[0].WeekDay);

                var actualWorkbook = actualExtractRefresh.Workbook;
                Assert.NotNull(actualWorkbook);
                Assert.Equal(expectedWorkBook.Id, actualWorkbook.Id);
            }
        }
    }
}
