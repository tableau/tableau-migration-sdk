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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models.Cloud;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Tests.Unit;
using Tableau.Migration.Tests.Unit.Content.Schedules;
using Xunit;

using CloudResponses = Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using ServerResponses = Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Tests.Unit.Api
{
    public sealed class TasksApiClientTests
    {
        public abstract class TasksApiClientTest : ApiClientTestBase<ITasksApiClient>
        {
            internal virtual TasksApiClient TasksApiClient => GetApiClient<TasksApiClient>();

            protected TableauInstanceType CurrentInstanceType { get; set; }

            protected ExtractRefreshTestCaches ExtractRefreshTestCaches { get; }

            public TasksApiClientTest()
            {
                MockSessionProvider.SetupGet(p => p.InstanceType).Returns(() => CurrentInstanceType);

                ExtractRefreshTestCaches = new(AutoFixture, MockDataSourceFinder, MockWorkbookFinder, MockScheduleFinder, MockScheduleCache);
            }

            protected static List<TExtractRefreshTask> AssertSuccess<TExtractRefreshTask, TSchedule>(IResult<IImmutableList<TExtractRefreshTask>> result)
                where TExtractRefreshTask : IExtractRefreshTask<TSchedule>
                where TSchedule : ISchedule
            {
                Assert.NotNull(result);
                Assert.Empty(result.Errors);

                var actualExtractRefreshes = result.Value?.ToList();
                Assert.NotNull(actualExtractRefreshes);
                return actualExtractRefreshes;
            }
        }

        public class ForServer : TasksApiClientTest
        {
            [Theory]
            [EnumData<TableauInstanceType>(TableauInstanceType.Server)]
            public void Fails_when_current_instance_is_not_server(TableauInstanceType instanceType)
            {
                CurrentInstanceType = instanceType;

                var exception = Assert.Throws<TableauInstanceTypeNotSupportedException>(TasksApiClient.ForServer);

                Assert.Equal(instanceType, exception.UnsupportedInstanceType);

                MockSessionProvider.VerifyGet(p => p.InstanceType, Times.Once);
            }

            [Fact]
            public void Returns_client_when_current_instance_is_server()
            {
                CurrentInstanceType = TableauInstanceType.Server;

                var client = TasksApiClient.ForServer();

                MockSessionProvider.VerifyGet(p => p.InstanceType, Times.Once);
            }
        }

        public class ForCloud : TasksApiClientTest
        {
            [Theory]
            [EnumData<TableauInstanceType>(TableauInstanceType.Cloud)]
            public void Fails_when_current_instance_is_not_cloud(TableauInstanceType instanceType)
            {
                CurrentInstanceType = instanceType;

                var exception = Assert.Throws<TableauInstanceTypeNotSupportedException>(TasksApiClient.ForCloud);

                Assert.Equal(instanceType, exception.UnsupportedInstanceType);

                MockSessionProvider.VerifyGet(p => p.InstanceType, Times.Once);
            }

            [Fact]
            public void Returns_client_when_current_instance_is_cloud()
            {
                CurrentInstanceType = TableauInstanceType.Cloud;

                var client = TasksApiClient.ForCloud();

                MockSessionProvider.VerifyGet(p => p.InstanceType, Times.Once);
            }
        }

        #region - DeleteExtractRefreshTaskAsync -

        public class DeleteExtractRefreshTaskAsync : TasksApiClientTest
        {
            [Fact]
            public async Task SuccessAsync()
            {
                //Setup
                var extractRefreshTaskId = Guid.NewGuid();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.NoContent));

                //Act
                var result = await TasksApiClient.DeleteExtractRefreshTaskAsync(extractRefreshTaskId, Cancel);

                //Test
                result.AssertSuccess();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                    r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/tasks/extractRefreshes/{extractRefreshTaskId}");
                });
            }

            [Fact]
            public async Task FailureAsync()
            {
                //Setup
                var extractRefreshTaskId = Guid.NewGuid();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.InternalServerError));

                //Act
                var result = await TasksApiClient.DeleteExtractRefreshTaskAsync(extractRefreshTaskId, Cancel);

                //Test
                result.AssertFailure();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                    r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/tasks/extractRefreshes/{extractRefreshTaskId}");
                });
            }
        }

        #endregion

        #region - Cloud -

        public class Cloud
        {
            public abstract class CloudTasksApiClientTest : TasksApiClientTest
            {
                internal ICloudTasksApiClient CloudTasksApiClient => TasksApiClient;

                public CloudTasksApiClientTest()
                {
                    CurrentInstanceType = TableauInstanceType.Cloud;
                }

                protected CloudResponses.ExtractRefreshTasksResponse CreateCloudResponse(ExtractRefreshContentType contentType)
                {
                    if (contentType == ExtractRefreshContentType.DataSource)
                    {
                        AutoFixture.Customize<CloudResponses.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType>(
                            composer => composer.With(j => j.Workbook, () => null));
                    }
                    else
                    {
                        AutoFixture.Customize<CloudResponses.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType>(
                            composer => composer.With(j => j.DataSource, () => null));
                    }

                    return AutoFixture.CreateResponse<CloudResponses.ExtractRefreshTasksResponse>();
                }
            }

            #region - GetAllExtractRefreshTasksAsync -

            public class GetAllExtractRefreshTasksAsync : CloudTasksApiClientTest
            {
                [Fact]
                public async Task GetsDatasourceExtractRefreshesAsync()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);

                    var response = CreateCloudResponse(ExtractRefreshContentType.DataSource);

                    ExtractRefreshTestCaches.SetupExtractRefreshContentFinder(response.Items.Select(i => i.ExtractRefresh).ExceptNulls());

                    SetupSuccessResponse(response);

                    var result = await CloudTasksApiClient.GetAllExtractRefreshTasksAsync(Cancel);

                    var actualExtractRefreshes = AssertSuccess<ICloudExtractRefreshTask, ICloudSchedule>(result);
                    var expectedExtractRefreshes = response.Items.ToList();

                    Assert.Equal(expectedExtractRefreshes.Count, actualExtractRefreshes.Count);
                    Assert.DoesNotContain(actualExtractRefreshes, item => item.ContentType == ExtractRefreshContentType.Workbook);
                }

                [Fact]
                public async Task GetsWorkbookExtractRefreshesAsync()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);

                    var response = CreateCloudResponse(ExtractRefreshContentType.Workbook);

                    ExtractRefreshTestCaches.SetupExtractRefreshContentFinder(response.Items.Select(i => i.ExtractRefresh).ExceptNulls());

                    SetupSuccessResponse(response);

                    var result = await CloudTasksApiClient.GetAllExtractRefreshTasksAsync(Cancel);

                    var actualExtractRefreshes = AssertSuccess<ICloudExtractRefreshTask, ICloudSchedule>(result);
                    var expectedExtractRefreshes = response.Items.ToList();

                    Assert.Equal(expectedExtractRefreshes.Count, actualExtractRefreshes.Count);
                    Assert.DoesNotContain(actualExtractRefreshes, item => item.ContentType == ExtractRefreshContentType.DataSource);
                }

                [Fact]
                public async Task IgnoresPersonalSpacesWorkbookTasksAsync()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);

                    var response = CreateCloudResponse(ExtractRefreshContentType.Workbook);

                    ExtractRefreshTestCaches.SetupExtractRefreshContentFinder(response.Items.Select(i => i.ExtractRefresh).ExceptNulls().Skip(1));

                    SetupSuccessResponse(response);

                    var result = await CloudTasksApiClient.GetAllExtractRefreshTasksAsync(Cancel);

                    var actualExtractRefreshes = AssertSuccess<ICloudExtractRefreshTask, ICloudSchedule>(result);
                    var expectedExtractRefreshes = response.Items.ToList();

                    Assert.Equal(expectedExtractRefreshes.Count - 1, actualExtractRefreshes.Count);
                }
            }

            #endregion

            #region - CreateExtractRefreshTaskAsync -

            public class CreateExtractRefreshTaskAsync : CloudTasksApiClientTest
            {
                [Fact]
                public async Task CreatesExtractRefreshForWorkbookSuccessfullyAsync()
                {
                    // Arrange
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);
                    var contentReference = AutoFixture.Create<IContentReference>();
                    var cloudSchedule = AutoFixture.Create<ICloudSchedule>();
                    var createTaskOptions = new CreateExtractRefreshTaskOptions(
                        ExtractRefreshType.FullRefresh,
                        ExtractRefreshContentType.Workbook,
                        contentReference.Id,
                        cloudSchedule);

                    var response = AutoFixture.CreateResponse<CloudResponses.CreateExtractRefreshTaskResponse>();
                    response.Item!.DataSource = null;
                    response.Item.Workbook!.Id = contentReference.Id;
                    response.Schedule!.Frequency = cloudSchedule.Frequency;

                    SetupSuccessResponse(response);
                    ExtractRefreshTestCaches.SetupExtractRefreshContentFinder(response.Item);

                    // Act
                    var result = await CloudTasksApiClient.CreateExtractRefreshTaskAsync(
                        createTaskOptions,
                        Cancel);

                    // Assert
                    Assert.True(result.Success);
                    Assert.Equal(cloudSchedule.Frequency, result.Value.Schedule.Frequency);
                    Assert.Equal(ExtractRefreshContentType.Workbook, result.Value.ContentType);
                    Assert.Equal(contentReference.Id, result.Value.Content.Id);
                }

                [Fact]
                public async Task CreatesExtractRefreshForDatasourceSuccessfullyAsync()
                {
                    // Arrange
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);
                    var contentReference = AutoFixture.Create<IContentReference>();
                    var cloudSchedule = AutoFixture.Create<ICloudSchedule>();
                    var createTaskOptions = new CreateExtractRefreshTaskOptions(
                        ExtractRefreshType.ServerIncrementalRefresh,
                        ExtractRefreshContentType.DataSource,
                        contentReference.Id,
                        cloudSchedule);

                    var response = AutoFixture.CreateResponse<CloudResponses.CreateExtractRefreshTaskResponse>();
                    response.Item!.Workbook = null;
                    response.Item.DataSource!.Id = contentReference.Id;
                    response.Schedule!.Frequency = cloudSchedule.Frequency;

                    SetupSuccessResponse(response);
                    ExtractRefreshTestCaches.SetupExtractRefreshContentFinder(response.Item);

                    // Act
                    var result = await CloudTasksApiClient.CreateExtractRefreshTaskAsync(
                        createTaskOptions,
                        Cancel);

                    // Assert
                    Assert.True(result.Success);
                    Assert.Equal(cloudSchedule.Frequency, result.Value.Schedule.Frequency);
                    Assert.Equal(ExtractRefreshContentType.DataSource, result.Value.ContentType);
                    Assert.Equal(contentReference.Id, result.Value.Content.Id);
                }

                [Fact]
                public async Task FailsToCreateExtractRefreshAsync()
                {
                    // Arrange
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);
                    var contentReference = AutoFixture.Create<IContentReference>();
                    var cloudSchedule = AutoFixture.Create<ICloudSchedule>();
                    var createTaskOptions = new CreateExtractRefreshTaskOptions(
                        ExtractRefreshType.FullRefresh,
                        ExtractRefreshContentType.DataSource,
                        contentReference.Id,
                        cloudSchedule);

                    SetupErrorResponse<CloudResponses.CreateExtractRefreshTaskResponse>(error => error.Code = RestErrorCodes.BAD_REQUEST);

                    // Act
                    var result = await CloudTasksApiClient.CreateExtractRefreshTaskAsync(
                        createTaskOptions,
                        Cancel);

                    // Assert
                    Assert.False(result.Success);
                    Assert.Null(result.Value);
                }

                [Fact]
                public async Task FailsContentReferenceNotFoundAsync()
                {
                    // Arrange
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);
                    var contentReference = AutoFixture.Create<IContentReference>();
                    var cloudSchedule = AutoFixture.Create<ICloudSchedule>();
                    var createTaskOptions = new CreateExtractRefreshTaskOptions(
                        ExtractRefreshType.FullRefresh,
                        ExtractRefreshContentType.Workbook,
                        contentReference.Id,
                        cloudSchedule);

                    var response = AutoFixture.CreateResponse<CloudResponses.CreateExtractRefreshTaskResponse>();
                    response.Item!.DataSource = null;
                    response.Item.Workbook!.Id = contentReference.Id;
                    response.Schedule!.Frequency = cloudSchedule.Frequency;

                    SetupSuccessResponse(response);

                    // Act
                    var result = await CloudTasksApiClient.CreateExtractRefreshTaskAsync(createTaskOptions, Cancel);

                    // Assert
                    result.AssertFailure();
                    Assert.IsType<ArgumentNullException>(result.Errors.Single());
                }
            }

            #endregion

            #region - CreateCloudFlowTaskAsync -

            public class CreateCloudFlowTaskAsync : CloudTasksApiClientTest
            {
                [Fact]
                public async Task SucceedsAsync()
                {
                    // Arrange
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);
                    var flowReference = AutoFixture.Create<IContentReference>();
                    var cloudSchedule = AutoFixture.Create<ICloudSchedule>();
                    var createTaskOptions = new CreateCloudFlowTaskOptions(
                        flowReference.Id,
                        cloudSchedule);

                    var response = AutoFixture.CreateResponse<CloudResponses.CreateCloudFlowTaskResponse>();
                    response.Item!.FlowRun!.Flow!.Id = flowReference.Id;
                    response.Item.FlowRun.Schedule!.Frequency = cloudSchedule.Frequency;

                    SetupSuccessResponse(response);
                    var mockFlowFinder = MockContentFinderFactory.SetupMockFinder<IFlow>(AutoFixture);
                    mockFlowFinder.Setup(f => f.FindByIdAsync(flowReference.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(flowReference);

                    // Act
                    var result = await CloudTasksApiClient.CreateCloudFlowTaskAsync(
                        createTaskOptions,
                        Cancel);

                    // Assert
                    Assert.True(result.Success);
                    Assert.Equal(cloudSchedule.Frequency, result.Value.Schedule.Frequency);
                    Assert.Equal(flowReference.Id, result.Value.Flow.Id);
                }

                [Fact]
                public async Task FailsToCreateCloudFlowTaskAsync()
                {
                    // Arrange
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);
                    var flowReference = AutoFixture.Create<IContentReference>();
                    var cloudSchedule = AutoFixture.Create<ICloudSchedule>();
                    var createTaskOptions = new CreateCloudFlowTaskOptions(
                        flowReference.Id,
                        cloudSchedule);

                    SetupErrorResponse<CloudResponses.CreateCloudFlowTaskResponse>(error => error.Code = RestErrorCodes.BAD_REQUEST);

                    // Act
                    var result = await CloudTasksApiClient.CreateCloudFlowTaskAsync(
                        createTaskOptions,
                        Cancel);

                    // Assert
                    Assert.False(result.Success);
                    Assert.Null(result.Value);
                }

                [Fact]
                public async Task FailsFlowReferenceNotFoundAsync()
                {
                    // Arrange
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);
                    var flowReference = AutoFixture.Create<IContentReference>();
                    var cloudSchedule = AutoFixture.Create<ICloudSchedule>();
                    var createTaskOptions = new CreateCloudFlowTaskOptions(
                        flowReference.Id,
                        cloudSchedule);

                    var response = AutoFixture.CreateResponse<CloudResponses.CreateCloudFlowTaskResponse>();
                    response.Item!.FlowRun!.Flow!.Id = flowReference.Id;
                    response.Item.FlowRun.Schedule!.Frequency = cloudSchedule.Frequency;

                    SetupSuccessResponse(response);
                    var mockFlowFinder = MockContentFinderFactory.SetupMockFinder<IFlow>(AutoFixture);
                    mockFlowFinder.Setup(f => f.FindByIdAsync(flowReference.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync((IContentReference?)null);

                    // Act
                    var result = await CloudTasksApiClient.CreateCloudFlowTaskAsync(createTaskOptions, Cancel);

                    // Assert
                    result.AssertFailure();
                    Assert.IsType<ArgumentNullException>(result.Errors.Single());
                }
            }

            #endregion

            #region - GetAllFlowRunTasksAsync -

            public class GetAllFlowRunTasksAsync : CloudTasksApiClientTest
            {
                [Fact]
                public async Task GetsFlowRunTasksAsync()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);
                    var response = AutoFixture.CreateResponse<ServerResponses.FlowRunTasksResponse>();

                    // Setup flow finders for all flow IDs in the response
                    var flowIds = response.Items
                        .Where(t => t.FlowRun?.Flow?.Id != null && t.FlowRun.Flow.Id != Guid.Empty)
                        .Select(t => t.FlowRun!.Flow!.Id)
                        .Distinct();

                    var mockFlowFinder = MockContentFinderFactory.SetupMockFinder<IFlow>(AutoFixture);
                    foreach (var flowId in flowIds)
                    {
                        var mockFlowReference = AutoFixture.Create<Mock<IContentReference>>();
                        mockFlowReference.SetupGet(r => r.Id).Returns(flowId);
                        var flowReference = mockFlowReference.Object;
                        mockFlowFinder.Setup(f => f.FindByIdAsync(flowId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(flowReference);
                    }

                    SetupSuccessResponse(response);

                    var result = await CloudTasksApiClient.GetAllFlowRunTasksAsync(Cancel);

                    Assert.NotNull(result);
                    Assert.True(result.Success);
                    Assert.Empty(result.Errors);

                    var actualFlowRunTasks = result.Value?.ToList();
                    Assert.NotNull(actualFlowRunTasks);
                    Assert.Equal(response.Items.Count(t => t.FlowRun != null), actualFlowRunTasks.Count);

                    MockHttpClient.AssertSingleRequest(r =>
                    {
                        r.AssertHttpMethod(HttpMethod.Get);
                        r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/tasks/runFlow");
                    });
                }

                [Fact]
                public async Task ReturnsEmptyListWhenNoTasksAsync()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);
                    var response = AutoFixture.CreateResponse<ServerResponses.FlowRunTasksResponse>();
                    response.Items = Array.Empty<ServerResponses.FlowRunTasksResponse.TaskType>();

                    SetupSuccessResponse(response);

                    var result = await CloudTasksApiClient.GetAllFlowRunTasksAsync(Cancel);

                    Assert.NotNull(result);
                    Assert.True(result.Success);
                    Assert.Empty(result.Errors);
                    Assert.Empty(result.Value!);
                }

                [Fact]
                public async Task IgnoresTasksWithMissingFlowReferencesAsync()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);
                    var response = AutoFixture.CreateResponse<ServerResponses.FlowRunTasksResponse>();

                    // Setup flow finder to return null for all flows (simulating missing references)
                    var mockFlowFinder = MockContentFinderFactory.SetupMockFinder<IFlow>(AutoFixture);
                    var flowIds = response.Items
                        .Where(t => t.FlowRun?.Flow?.Id != null && t.FlowRun.Flow.Id != Guid.Empty)
                        .Select(t => t.FlowRun!.Flow!.Id)
                        .Distinct();

                    foreach (var flowId in flowIds)
                    {
                        mockFlowFinder.Setup(f => f.FindByIdAsync(flowId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync((IContentReference?)null);
                    }

                    SetupSuccessResponse(response);

                    var result = await CloudTasksApiClient.GetAllFlowRunTasksAsync(Cancel);

                    Assert.NotNull(result);
                    Assert.True(result.Success);
                    Assert.Empty(result.Errors);
                    // All tasks should be filtered out due to missing flow references
                    Assert.Empty(result.Value!);
                }

                [Fact]
                public async Task ReturnsFailureOnErrorAsync()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Cloud);

                    SetupErrorResponse<ServerResponses.FlowRunTasksResponse>(error => error.Code = RestErrorCodes.BAD_REQUEST);

                    var result = await CloudTasksApiClient.GetAllFlowRunTasksAsync(Cancel);

                    Assert.NotNull(result);
                    Assert.False(result.Success);
                    Assert.Null(result.Value);

                    MockHttpClient.AssertSingleRequest(r =>
                    {
                        r.AssertHttpMethod(HttpMethod.Get);
                        r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/tasks/runFlow");
                    });
                }
            }

            #endregion
        }

        #endregion

        #region - Server -

        public class Server
        {
            public abstract class ServerTasksApiClientTest : TasksApiClientTest
            {
                internal IServerTasksApiClient ServerTasksApiClient => TasksApiClient;

                public ServerTasksApiClientTest()
                {
                    CurrentInstanceType = TableauInstanceType.Server;
                }

                protected ServerResponses.ExtractRefreshTasksResponse CreateServerResponse(ExtractRefreshContentType contentType)
                {
                    if (contentType == ExtractRefreshContentType.DataSource)
                    {
                        AutoFixture.Customize<ServerResponses.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType>(
                            composer => composer.With(j => j.Workbook, () => null));
                    }
                    else
                    {
                        AutoFixture.Customize<ServerResponses.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType>(
                            composer => composer.With(j => j.DataSource, () => null));
                    }

                    return AutoFixture.CreateResponse<ServerResponses.ExtractRefreshTasksResponse>();
                }
            }

            #region - GetAllExtractRefreshTasksAsync -

            public class GetAllExtractRefreshTasksAsync : ServerTasksApiClientTest
            {
                [Fact]
                public async Task GetsWorkbookExtractRefreshesAsync()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Server);
                    var response = CreateServerResponse(ExtractRefreshContentType.Workbook);

                    ExtractRefreshTestCaches.SetupExtractRefreshContentFinder(response.Items.Select(i => i.ExtractRefresh).ExceptNulls());

                    SetupSuccessResponse(response);

                    var result = await ServerTasksApiClient.GetAllExtractRefreshTasksAsync(Cancel);

                    var actualExtractRefreshes = AssertSuccess<IServerExtractRefreshTask, IServerSchedule>(result);
                    var expectedExtractRefreshes = response.Items.ToList();

                    Assert.Equal(expectedExtractRefreshes.Count, actualExtractRefreshes.Count);
                    Assert.DoesNotContain(actualExtractRefreshes, item => item.ContentType == ExtractRefreshContentType.DataSource);
                }

                [Fact]
                public async Task GetsDatasourceExtractRefreshesAsync()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Server);

                    var response = CreateServerResponse(ExtractRefreshContentType.DataSource);

                    ExtractRefreshTestCaches.SetupExtractRefreshContentFinder(response.Items.Select(i => i.ExtractRefresh).ExceptNulls());

                    SetupSuccessResponse(response);

                    var result = await ServerTasksApiClient.GetAllExtractRefreshTasksAsync(Cancel);

                    var actualExtractRefreshes = AssertSuccess<IServerExtractRefreshTask, IServerSchedule>(result);
                    var expectedExtractRefreshes = response.Items.ToList();

                    Assert.Equal(expectedExtractRefreshes.Count, actualExtractRefreshes.Count);
                    Assert.DoesNotContain(actualExtractRefreshes, item => item.ContentType == ExtractRefreshContentType.Workbook);
                }

                [Fact]
                public async Task IgnoresPersonalSpacesWorkbookTasksAsync()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Server);

                    var response = CreateServerResponse(ExtractRefreshContentType.Workbook);

                    ExtractRefreshTestCaches.SetupExtractRefreshContentFinder(response.Items.Select(i => i.ExtractRefresh).ExceptNulls().Skip(1));

                    SetupSuccessResponse(response);

                    var result = await ServerTasksApiClient.GetAllExtractRefreshTasksAsync(Cancel);

                    var actualExtractRefreshes = AssertSuccess<IServerExtractRefreshTask, IServerSchedule>(result);
                    var expectedExtractRefreshes = response.Items.ToList();

                    Assert.Equal(expectedExtractRefreshes.Count - 1, actualExtractRefreshes.Count);
                }
            }

            #endregion

            #region - GetAllFlowRunTasksAsync -

            public class GetAllFlowRunTasksAsync : ServerTasksApiClientTest
            {
                [Fact]
                public async Task SucceedsAsync()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Server);
                    var response = AutoFixture.CreateResponse<ServerResponses.FlowRunTasksResponse>();

                    SetupSuccessResponse(response);

                    var result = await ServerTasksApiClient.GetAllFlowRunTasksAsync(Cancel);

                    Assert.NotNull(result);
                    Assert.True(result.Success);
                    Assert.Empty(result.Errors);

                    var actualFlowRunTasks = result.Value?.ToList();
                    Assert.NotNull(actualFlowRunTasks);
                    Assert.Equal(response.Items.Count(t => t.FlowRun != null), actualFlowRunTasks.Count);

                    MockHttpClient.AssertSingleRequest(r =>
                    {
                        r.AssertHttpMethod(HttpMethod.Get);
                        r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/tasks/runFlow");
                    });
                }

                [Fact]
                public async Task ReturnsEmptyListWhenNoTasksAsync()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Server);
                    var response = AutoFixture.CreateResponse<ServerResponses.FlowRunTasksResponse>();
                    response.Items = Array.Empty<ServerResponses.FlowRunTasksResponse.TaskType>();

                    SetupSuccessResponse(response);

                    var result = await ServerTasksApiClient.GetAllFlowRunTasksAsync(Cancel);

                    Assert.NotNull(result);
                    Assert.True(result.Success);
                    Assert.Empty(result.Errors);
                    Assert.Empty(result.Value!);

                    MockHttpClient.AssertSingleRequest(r =>
                    {
                        r.AssertHttpMethod(HttpMethod.Get);
                        r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/tasks/runFlow");
                    });
                }

                [Fact]
                public async Task ReturnsFailureOnErrorAsync()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Server);

                    SetupErrorResponse<ServerResponses.FlowRunTasksResponse>(error => error.Code = RestErrorCodes.BAD_REQUEST);

                    var result = await ServerTasksApiClient.GetAllFlowRunTasksAsync(Cancel);

                    Assert.NotNull(result);
                    Assert.False(result.Success);
                    Assert.Null(result.Value);

                    MockHttpClient.AssertSingleRequest(r =>
                    {
                        r.AssertHttpMethod(HttpMethod.Get);
                        r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/tasks/runFlow");
                    });
                }
            }

            #endregion
        }

        #endregion
    }
}
