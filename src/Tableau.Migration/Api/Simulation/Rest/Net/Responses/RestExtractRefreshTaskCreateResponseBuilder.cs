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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Requests.Cloud;
using Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestExtractRefreshTaskCreateResponseBuilder : RestApiResponseBuilderBase<CreateExtractRefreshTaskResponse>
    {
        public RestExtractRefreshTaskCreateResponseBuilder(TableauData data, IHttpContentSerializer serializer)
            : base(data, serializer, requiresAuthentication: true)
        { }

        protected override ValueTask<(CreateExtractRefreshTaskResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(
            HttpRequestMessage request, 
            CancellationToken cancel)
        {
            if (request?.Content is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest, 
                    0, 
                    "Request or content cannot be null.", 
                    "");
            }
            
            var createRequest = request.GetTableauServerRequest<CreateExtractRefreshTaskRequest>();
            var extractRefresh = createRequest?.ExtractRefresh;
            var schedule = createRequest?.Schedule;
            
            if (extractRefresh is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest,
                    0,
                    $"The request property {nameof(CreateExtractRefreshTaskRequest.ExtractRefreshType)} must not be null",
                    "");
            }

            if (extractRefresh.Workbook is null &&
                extractRefresh.DataSource is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest,
                    0,
                    $"The request must contain a single reference to a Workbook or a Data Source",
                    "");
            }
            else if (extractRefresh.Workbook is not null &&
                    extractRefresh.DataSource is not null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest,
                    0,
                    $"The request must contain a single reference to a Workbook or a Data Source",
                    "");
            }
            else if (extractRefresh.Workbook is not null &&
                Data.Workbooks.SingleOrDefault(w => w.Id == extractRefresh.Workbook.Id) is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.NotFound, 
                    0, 
                    $"The workbook with ID {extractRefresh.Workbook.Id} could not be found.", 
                    "");    
            }
            else if (extractRefresh.DataSource is not null &&
                Data.DataSources.SingleOrDefault(w => w.Id == extractRefresh.DataSource.Id) is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.NotFound, 
                    0, 
                    $"The data source with ID {extractRefresh.DataSource.Id} could not be found.", 
                    "");    
            }

            if (schedule is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest,
                    0,
                    $"The request property {nameof(CreateExtractRefreshTaskRequest.ScheduleType)} must not be null",
                    "");
            }

            if (schedule.FrequencyDetails is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest,
                    0,
                    $"The request property {nameof(CreateExtractRefreshTaskRequest.ScheduleType.FrequencyDetailsType)} must not be null",
                    "");
            }

            var extractRefreshTask = new ExtractRefreshTasksResponse.TaskType
            {
                ExtractRefresh = new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType
                {
                    Id = Guid.NewGuid(),
                    Type = extractRefresh.Type,
                    Workbook = extractRefresh.Workbook is not null
                        ? new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.WorkbookType
                        {
                            Id = extractRefresh.Workbook.Id
                        }
                        : null,
                    DataSource = extractRefresh.DataSource is not null
                        ? new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.DataSourceType
                        {
                            Id = extractRefresh.DataSource.Id
                        }
                        : null,
                    Schedule = new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.ScheduleType
                    {
                        Frequency = schedule.Frequency,
                        FrequencyDetails = new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.ScheduleType.FrequencyDetailsType
                        { 
                            Start = schedule.FrequencyDetails.Start,
                            End = schedule.FrequencyDetails.End,
                            Intervals = schedule
                                .FrequencyDetails
                                .Intervals
                                .Select(interval => new ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.ScheduleType.FrequencyDetailsType.IntervalType
                                    { 
                                        Hours = interval.Hours,
                                        Minutes = interval.Minutes,
                                        MonthDay = interval.MonthDay,
                                        WeekDay = interval.WeekDay
                                    })
                                .ToArray()
                        }
                    }
                }
            };

            Data.CloudExtractRefreshTasks.Add(extractRefreshTask);

            return ValueTask.FromResult((new CreateExtractRefreshTaskResponse
            {
                Item = new CreateExtractRefreshTaskResponse.ExtractRefreshType
                {
                    Id = extractRefreshTask.ExtractRefresh.Id,
                    Type = extractRefreshTask.ExtractRefresh.Type,
                    Workbook = extractRefreshTask.ExtractRefresh.Workbook is null
                        ? null
                        : new CreateExtractRefreshTaskResponse.ExtractRefreshType.WorkbookType
                        {
                            Id = extractRefreshTask.ExtractRefresh.Workbook.Id
                        },
                    DataSource = extractRefreshTask.ExtractRefresh.DataSource is null
                        ? null
                        : new CreateExtractRefreshTaskResponse.ExtractRefreshType.DataSourceType
                        {
                            Id = extractRefreshTask.ExtractRefresh.DataSource.Id
                        }
                },
                Schedule = new CreateExtractRefreshTaskResponse.ScheduleType
                {
                    Frequency = extractRefreshTask.ExtractRefresh.Schedule.Frequency,
                    FrequencyDetails = new CreateExtractRefreshTaskResponse.ScheduleType.FrequencyDetailsType
                    {
                        Start = extractRefreshTask.ExtractRefresh.Schedule.FrequencyDetails.Start,
                        End = extractRefreshTask.ExtractRefresh.Schedule.FrequencyDetails.End,
                        Intervals = extractRefreshTask
                                .ExtractRefresh
                                .Schedule
                                .FrequencyDetails
                                .Intervals
                                .Select(interval => new CreateExtractRefreshTaskResponse.ScheduleType.FrequencyDetailsType.IntervalType
                                {
                                    Hours = interval.Hours,
                                    Minutes = interval.Minutes,
                                    MonthDay = interval.MonthDay,
                                    WeekDay = interval.WeekDay
                                })
                                .ToArray()
                    }
                }
            },
            HttpStatusCode.Created));
        }
    }
}
