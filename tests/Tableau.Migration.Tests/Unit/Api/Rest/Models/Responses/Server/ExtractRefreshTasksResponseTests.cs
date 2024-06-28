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
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Responses.Server
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

                var expectedSchedule = expectedExtractRefresh.Schedule;
                Assert.NotNull(expectedSchedule);

                var expectedDataSource = expectedExtractRefresh.DataSource;
                Assert.NotNull(expectedDataSource);

                var xml = @$"
<tsResponse>
  <tasks>
    <task>
      <extractRefresh 
        id=""{expectedExtractRefresh.Id}""
        priority=""{expectedExtractRefresh.Priority}""
        consecutiveFailedCount=""{expectedExtractRefresh.ConsecutiveFailedCount}""
        type=""REFRESH_EXTRACT"">
          <schedule id=""{expectedSchedule.Id}""
            name=""{expectedSchedule.Name}""
            state=""{expectedSchedule.State}""
            priority=""{expectedSchedule.Priority}""
            createdAt=""{expectedSchedule.CreatedAt}""
            updatedAt=""{expectedSchedule.UpdatedAt}""
            type=""Extract""
            frequency=""{expectedSchedule.Frequency}""
            nextRunAt=""{expectedSchedule.NextRunAt}"" />
          <datasource id=""{expectedExtractRefresh.DataSource?.Id}"" />
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
                Assert.Equal(expectedSchedule.Id, actualSchedule.Id);
                Assert.Equal(expectedSchedule.Name, actualSchedule.Name);
                Assert.Equal(expectedSchedule.State, actualSchedule.State);
                Assert.Equal(expectedSchedule.Priority, actualSchedule.Priority);
                Assert.Equal(expectedSchedule.CreatedAt, actualSchedule.CreatedAt);
                Assert.Equal(expectedSchedule.UpdatedAt, actualSchedule.UpdatedAt);
                Assert.Equal(expectedSchedule.Frequency, actualSchedule.Frequency);
                Assert.Equal(expectedSchedule.NextRunAt, actualSchedule.NextRunAt);

                var actualDataSource = actualExtractRefresh.DataSource;
                Assert.NotNull(actualDataSource);
                Assert.Equal(expectedDataSource.Id, actualDataSource.Id);
            }
        }
    }
}
