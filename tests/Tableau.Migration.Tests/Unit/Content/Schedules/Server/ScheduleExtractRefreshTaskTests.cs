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
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Content.Schedules.Server;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Schedules.Server
{
    public class ScheduleExtractRefreshTaskTests : AutoFixtureTestBase
    {
        [Fact]
        public void Constructor_GivenScheduleExtractsResponse_ExtractType_ShouldSetProperties()
        {
            // Arrange
            var response = Create<ScheduleExtractRefreshTasksResponse.ExtractType>();

            // Act
            var scheduleExtract = new ScheduleExtractRefreshTask(response);

            // Assert
            Assert.Equal(response.Id, scheduleExtract.Id);
            Assert.Equal(response.Priority, scheduleExtract.Priority);
            Assert.Equal(response.Type, scheduleExtract.Type);
            Assert.Equal(response.Workbook?.Id, scheduleExtract.WorkbookId);
            Assert.Equal(response.DataSource?.Id, scheduleExtract.DatasourceId);
        }

        [Fact]
        public void Test_ScheduleExtract_With_Null_Id()
        {
            // Arrange
            var response = Create<ScheduleExtractRefreshTasksResponse.ExtractType>();
            response.Id = default;
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ScheduleExtractRefreshTask(response));
        }

        [Fact]
        public void Test_ScheduleExtract_With_Null_Type()
        {
            // Arrange
            var response = Create<ScheduleExtractRefreshTasksResponse.ExtractType>();
            response.Type = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ScheduleExtractRefreshTask(response));
        }
    }
}
