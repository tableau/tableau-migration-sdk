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

using System;
using System.Linq;
using Tableau.Migration.Content.Schedules.Server;
using Xunit;
using ScheduleExtractRefreshTasksResponse = Tableau.Migration.Api.Rest.Models.Responses.Server.ScheduleExtractRefreshTasksResponse;

namespace Tableau.Migration.Tests.Unit.Content.Schedules.Server
{
    public class ScheduleExtractRefreshTasksTests : AutoFixtureTestBase
    {
        [Fact]
        public void Ctor()
        {
            // Arrange            
            var scheduleId = Guid.NewGuid();
            var response = new ScheduleExtractRefreshTasksResponse
            {
                Items = CreateMany<ScheduleExtractRefreshTasksResponse.ExtractType>(3).ToArray()
            };

            // Act
            var scheduleExtracts = new ScheduleExtractRefreshTasks(scheduleId, response);

            // Assert
            Assert.Equal(3, scheduleExtracts.ExtractRefreshTasks.Count);
        }
    }
}