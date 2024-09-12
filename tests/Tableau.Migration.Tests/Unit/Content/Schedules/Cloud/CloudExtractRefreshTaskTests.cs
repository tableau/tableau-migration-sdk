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
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Xunit;

using ExtractRefreshType = Tableau.Migration.Api.Rest.Models.Responses.Cloud.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType;

namespace Tableau.Migration.Tests.Unit.Content.Schedules.Cloud
{
    public sealed class CloudExtractRefreshTaskTests
    {
        public abstract class CloudExtractRefreshTaskTest : ExtractRefreshTaskTestBase
        {
            protected ExtractRefreshType CreateExtractRefreshResponse(
                string? type = null,
                ExtractRefreshContentType? contentType = null,
                Guid? contentId = null,
                Action<ExtractRefreshType>? configure = null)
                => CreateExtractRefreshResponse<ExtractRefreshType, ExtractRefreshType.WorkbookType, ExtractRefreshType.DataSourceType>(type, contentType, contentId, configure);

            internal CloudExtractRefreshTask CreateExtractRefreshTask(
                ICloudExtractRefreshType? response = null,
                string? type = null,
                IContentReference? content = null,
                ICloudSchedule? schedule = null)
            {
                response ??= CreateExtractRefreshResponse();

                return new(
                    response.Id,
                    type ?? GetRandomType(),
                    response.GetContentType(),
                    content ?? CreateContentReference(),
                    schedule ?? CreateCloudSchedule());
            }
        }

        public sealed class Ctor : CloudExtractRefreshTaskTest
        {
            [Theory, ExtractRefreshContentTypeData]
            public void Initializes(ExtractRefreshContentType contentType)
            {
                var type = GetRandomType();
                var response = CreateExtractRefreshResponse(type, contentType);
                var content = CreateContentReference();
                var schedule = CreateCloudSchedule();

                var task = new CloudExtractRefreshTask(response.Id, type, response.GetContentType(), content, schedule);

                Assert.Same(content, task.Content);
                Assert.Equal(type, task.Type);
                Assert.Equal(contentType, task.ContentType);
                Assert.Same(schedule, task.Schedule);
            }
        }

        public sealed class CreateManyAsync : CloudExtractRefreshTaskTest
        {
            [Theory, ExtractRefreshContentTypeData]
            public async Task IgnoresPersonalSpaceTasksAsync(ExtractRefreshContentType contentType)
            {
                var response = new ExtractRefreshTasksResponse
                {
                    Items = Enumerable.Range(1, 10)
                        .Select(i => new ExtractRefreshTasksResponse.TaskType
                            {
                                ExtractRefresh = CreateExtractRefreshResponse(GetRandomType(), contentType)
                            })
                        .ToArray()
                };

                ExtractRefreshTestCaches.SetupExtractRefreshContentFinder(response.Items.Select(i => i.ExtractRefresh).ExceptNulls().Skip(1));

                var tasks = await CloudExtractRefreshTask.CreateManyAsync(response, MockFinderFactory.Object, Logger, Localizer, Cancel);

                Assert.Equal(response.Items.Length - 1, tasks.Count);
            }
        }
    }
}
