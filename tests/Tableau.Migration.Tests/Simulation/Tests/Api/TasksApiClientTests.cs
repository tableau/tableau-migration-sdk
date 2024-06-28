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
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models.Cloud;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public class TasksApiClientTests
    {
        public abstract class TasksApiClientTest : ApiClientTestBase
        {
            public TasksApiClientTest(bool isCloud = false)
                : base(isCloud)
            { }
        }

        #region - FromServer Tests -

        public class For_FromServer : TasksApiClientTest
        {
            public For_FromServer()
                : base(false)
            { }

            [Fact]
            public async Task ForServer_Returns_ServerTasksApiClient()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var serverTasks = sitesClient.ServerTasks;

                Assert.NotNull(serverTasks);

                var taskApi = Assert.IsAssignableFrom<ITasksApiClient>(serverTasks);

                Assert.IsAssignableFrom<IServerTasksApiClient>(taskApi.ForServer());
            }

            [Fact]
            public async Task ForCloud_Throws_Exception()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var cloudTasks = sitesClient.ServerTasks;

                Assert.NotNull(cloudTasks);

                var taskApi = Assert.IsAssignableFrom<ITasksApiClient>(cloudTasks);

                Assert.Throws<TableauInstanceTypeNotSupportedException>(taskApi.ForCloud);
            }
        }

        public class GetAllExtractRefreshTasksAsync_FromServer : TasksApiClientTest
        {
            public GetAllExtractRefreshTasksAsync_FromServer()
                : base(false)
            { }

            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                const int EXTRACT_REFRESH_COUNT = 10;

                var tasks = Api.Data.CreateServerExtractRefreshTasks(AutoFixture, EXTRACT_REFRESH_COUNT);

                // Act
                var result = await sitesClient.ServerTasks.GetAllExtractRefreshTasksAsync(Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.NotEmpty(result.Value);
                Assert.Equal(EXTRACT_REFRESH_COUNT, result.Value.Count);
            }

            [Fact]
            public async Task Returns_success_with_empty()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act
                var result = await sitesClient.ServerTasks.GetAllExtractRefreshTasksAsync(Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Empty(result.Value);
            }
        }

        #endregion - FromServer Tests -

        #region  - FromCloud Tests -

        public class For_FromCloud : TasksApiClientTest
        {
            public For_FromCloud()
                : base(true)
            { }

            [Fact]
            public async Task ForServer_Throws_Exception()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var cloudTasks = sitesClient.CloudTasks;

                Assert.NotNull(cloudTasks);

                var taskApi = Assert.IsAssignableFrom<ITasksApiClient>(cloudTasks);

                Assert.Throws<TableauInstanceTypeNotSupportedException>(taskApi.ForServer);
            }

            [Fact]
            public async Task ForCloud_Returns_CloudTasksApiClient()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var cloudTasks = sitesClient.CloudTasks;

                Assert.NotNull(cloudTasks);

                var taskApi = Assert.IsAssignableFrom<ITasksApiClient>(cloudTasks);

                Assert.IsAssignableFrom<ICloudTasksApiClient>(taskApi.ForCloud());
            }
        }

        public class GetAllExtractRefreshTasksAsync_FromCloud : TasksApiClientTest
        {
            public GetAllExtractRefreshTasksAsync_FromCloud()
                : base(true)
            { }

            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                const int EXTRACT_REFRESH_COUNT = 10;

                var tasks = Api.Data.CreateCloudExtractRefreshTasks(AutoFixture, EXTRACT_REFRESH_COUNT);

                // Act
                var result = await sitesClient.CloudTasks.GetAllExtractRefreshTasksAsync(Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.NotEmpty(result.Value);
                Assert.Equal(EXTRACT_REFRESH_COUNT, result.Value.Count);
            }

            [Fact]
            public async Task Returns_success_with_empty()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act
                var result = await sitesClient.CloudTasks.GetAllExtractRefreshTasksAsync(Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Empty(result.Value);
            }
        }

        public class CreateExtractRefreshTaskAsync_FromCloud : TasksApiClientTest
        {
            public CreateExtractRefreshTaskAsync_FromCloud()
                : base(true)
            { }

            [Fact]
            public async Task Returns_success_on_workbook_extract_refresh()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var cloudTasks = sitesClient.CloudTasks;

                var workbook = Api.Data.CreateWorkbook(AutoFixture);
                var frequency = ScheduleFrequencies.Weekly;
                var start = new TimeOnly(20, 0);
                var interval = Interval.WithWeekday(WeekDays.Monday);
                var schedule = new CloudSchedule(
                    frequency,
                    new FrequencyDetails(
                        start,
                        null,
                        [interval]));
                
                var options = new CreateExtractRefreshTaskOptions(
                    ExtractRefreshType.FullRefresh,
                    ExtractRefreshContentType.Workbook,
                    workbook.Id,
                    schedule);

                Assert.DoesNotContain(
                    Api.Data.CloudExtractRefreshTasks, 
                    cert => 
                        cert.ExtractRefresh!.Workbook is not null &&
                        cert.ExtractRefresh!.Workbook.Id == workbook.Id);

                // Act
                var result = await cloudTasks.CreateExtractRefreshTaskAsync(
                    options,
                    Cancel);

                // Assert
                Assert.True(result.Success);
                var extractRefresh = Api.Data.CloudExtractRefreshTasks.FirstOrDefault(cert => cert.ExtractRefresh!.Id == result.Value.Id);

                Assert.NotNull(extractRefresh);
                Assert.NotNull(extractRefresh.ExtractRefresh!.Workbook);
                Assert.Equal(workbook.Id, extractRefresh.ExtractRefresh.Workbook.Id);
                Assert.NotNull(extractRefresh.ExtractRefresh.Schedule);
                Assert.Equal(frequency, extractRefresh.ExtractRefresh.Schedule.Frequency);
                Assert.NotNull(extractRefresh.ExtractRefresh.Schedule.FrequencyDetails);
                Assert.Equal(start.ToString(Constants.FrequencyTimeFormat), extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.Start);
                Assert.Null(extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.End);
                Assert.Single(extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.Intervals);
                Assert.Equal(interval.WeekDay, extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.Intervals[0].WeekDay);
                Assert.Null(extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.Intervals[0].Hours);
                Assert.Null(extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.Intervals[0].Minutes);
                Assert.Null(extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.Intervals[0].MonthDay);
            }

            [Fact]
            public async Task Returns_success_on_datasource_extract_refresh()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var cloudTasks = sitesClient.CloudTasks;

                var datasource = Api.Data.CreateDataSource(AutoFixture);
                var frequency = ScheduleFrequencies.Monthly;
                var start = new TimeOnly(13, 45);
                var interval = Interval.WithMonthDay("10");
                var schedule = new CloudSchedule(
                    frequency,
                    new FrequencyDetails(
                        start,
                        null,
                        [interval]));
                
                var options = new CreateExtractRefreshTaskOptions(
                    ExtractRefreshType.ServerIncrementalRefresh,
                    ExtractRefreshContentType.DataSource,
                    datasource.Id,
                    schedule);

                Assert.DoesNotContain(
                    Api.Data.CloudExtractRefreshTasks, 
                    cert => 
                        cert.ExtractRefresh!.DataSource is not null &&
                        cert.ExtractRefresh!.DataSource.Id == datasource.Id);

                // Act
                var result = await cloudTasks.CreateExtractRefreshTaskAsync(
                    options,
                    Cancel);

                // Assert
                Assert.True(result.Success);
                var extractRefresh = Api.Data.CloudExtractRefreshTasks.FirstOrDefault(cert => cert.ExtractRefresh!.Id == result.Value.Id);

                Assert.NotNull(extractRefresh);
                Assert.NotNull(extractRefresh.ExtractRefresh!.DataSource);
                Assert.Equal(datasource.Id, extractRefresh.ExtractRefresh!.DataSource.Id);
                Assert.NotNull(extractRefresh.ExtractRefresh.Schedule);
                Assert.Equal(frequency, extractRefresh.ExtractRefresh.Schedule.Frequency);
                Assert.NotNull(extractRefresh.ExtractRefresh.Schedule.FrequencyDetails);
                Assert.Equal(start.ToString(Constants.FrequencyTimeFormat), extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.Start);
                Assert.Null(extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.End);
                Assert.Single(extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.Intervals);
                Assert.Equal(interval.MonthDay, extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.Intervals[0].MonthDay);
                Assert.Null(extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.Intervals[0].Hours);
                Assert.Null(extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.Intervals[0].Minutes);
                Assert.Null(extractRefresh.ExtractRefresh.Schedule.FrequencyDetails.Intervals[0].WeekDay);
            }
        }

        public class DeleteExtractRefreshTaskAsync_FromCloud : TasksApiClientTest
        {
            public DeleteExtractRefreshTaskAsync_FromCloud()
                : base(true)
            { }

            [Fact]
            public async Task Returns_success_on_existing_workbook_extract_refresh()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var cloudTasks = sitesClient.CloudTasks;

                var workbook = Api.Data.CreateWorkbook(AutoFixture);
                var extractRefreshTask = Api.Data.CreateCloudExtractRefreshTask(
                    AutoFixture,
                    workbook: workbook);
                Assert.Contains(Api.Data.CloudExtractRefreshTasks, cert => cert.ExtractRefresh!.Id == extractRefreshTask.ExtractRefresh!.Id);

                // Act
                var result = await cloudTasks.DeleteExtractRefreshTaskAsync(
                    extractRefreshTask.ExtractRefresh!.Id,
                    Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.DoesNotContain(Api.Data.CloudExtractRefreshTasks, cert => cert.ExtractRefresh!.Id == extractRefreshTask.ExtractRefresh!.Id);
            }

            [Fact]
            public async Task Returns_success_on_existing_datasource_extract_refresh()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var cloudTasks = sitesClient.CloudTasks;

                var dataSource = Api.Data.CreateDataSource(AutoFixture);
                var extractRefreshTask = Api.Data.CreateCloudExtractRefreshTask(
                    AutoFixture,
                    dataSource: dataSource);
                Assert.Contains(Api.Data.CloudExtractRefreshTasks, cert => cert.ExtractRefresh!.Id == extractRefreshTask.ExtractRefresh!.Id);

                // Act
                var result = await cloudTasks.DeleteExtractRefreshTaskAsync(
                    extractRefreshTask.ExtractRefresh!.Id,
                    Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.DoesNotContain(Api.Data.CloudExtractRefreshTasks, cert => cert.ExtractRefresh!.Id == extractRefreshTask.ExtractRefresh!.Id);
            }
        }

        #endregion  - FromCloud Tests -
    }
}
