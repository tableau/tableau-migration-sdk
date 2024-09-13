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
using Moq;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Xunit;

using ExtractRefreshType = Tableau.Migration.Api.Rest.Models.Responses.Server.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType;

namespace Tableau.Migration.Tests.Unit.Content.Schedules.Server
{
    public sealed class ServerExtractRefreshTaskTests
    {
        public abstract class ServerExtractRefreshTaskTest : ExtractRefreshTaskTestBase
        {
            protected readonly Mock<IContentCacheFactory> MockContentCacheFactory = new();

            public ServerExtractRefreshTaskTest()
            {
                MockContentCacheFactory
                    .Setup(f => f.ForContentType<IServerSchedule>(true))
                    .Returns(MockScheduleCache.Object);
            }

            protected ExtractRefreshType CreateExtractRefreshResponse(
                string? type = null,
                ExtractRefreshContentType? contentType = null,
                Guid? contentId = null,
                Action<ExtractRefreshType>? configure = null)
                => CreateExtractRefreshResponse<ExtractRefreshType, ExtractRefreshType.WorkbookType, ExtractRefreshType.DataSourceType>(type, contentType, contentId, configure);

            internal ServerExtractRefreshTask CreateExtractRefreshTask(
                IServerExtractRefreshType? response = null,
                string? type = null,
                IContentReference? content = null,
                IServerSchedule? schedule = null)
            {
                response ??= CreateExtractRefreshResponse();

                return new(
                    response.Id,
                    type ?? GetRandomType(),
                    response.GetContentType(),
                    content ?? CreateContentReference(),
                    schedule ?? CreateServerSchedule());
            }
        }

        public sealed class Ctor : ServerExtractRefreshTaskTest
        {
            [Theory, ExtractRefreshContentTypeData]
            public void Initializes(ExtractRefreshContentType contentType)
            {
                var type = GetRandomType();
                var response = CreateExtractRefreshResponse(type, contentType);
                var content = CreateContentReference();
                var schedule = CreateServerSchedule();

                var task = new ServerExtractRefreshTask(response.Id, type, response.GetContentType(), content, schedule);

                Assert.Same(content, task.Content);
                Assert.Equal(type, task.Type);
                Assert.Equal(contentType, task.ContentType);
                Assert.Same(schedule, task.Schedule);
            }
        }

        public sealed class CreateManyAsync : ServerExtractRefreshTaskTest
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

                var tasks = await ServerExtractRefreshTask.CreateManyAsync(response, MockFinderFactory.Object, MockContentCacheFactory.Object, Logger, Localizer, Cancel);

                Assert.Equal(response.Items.Length - 1, tasks.Count);
            }
        }
    }
}
