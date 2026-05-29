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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Caching;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Content.Schedules.Server
{
    internal sealed class ServerFlowRunTask : ContentBase, IServerFlowRunTask
    {
        public string Type { get; set; }

        public int Priority { get; set; }

        public int ConsecutiveFailedCount { get; set; }

        public IContentReference Flow { get; set; }

        public IServerSchedule Schedule { get; }

        internal ServerFlowRunTask(
            Guid flowTaskId,
            string type,
            int priority,
            int consecutiveFailedCount,
            IContentReference flow,
            IServerSchedule schedule)
            : base(
                new ContentReferenceStub(
                    flowTaskId,
                    string.Empty,
                    new(flowTaskId.ToString())))
        {
            Type = type;
            Priority = priority;
            ConsecutiveFailedCount = consecutiveFailedCount;
            Flow = flow;
            Schedule = schedule;
        }

        public static async Task<IImmutableList<IServerFlowRunTask>> CreateManyAsync(
            FlowRunTasksResponse response,
            IContentReferenceFinderFactory finderFactory,
            IContentCacheFactory contentCacheFactory,
            ILogger logger,
            CancellationToken cancel)
        {
            var items = response.Items
                .Where(t => t.FlowRun != null)
                .Select(t => t.FlowRun!)
                .ToImmutableArray();

            var tasks = ImmutableArray.CreateBuilder<IServerFlowRunTask>(items.Length);
            var flowFinder = finderFactory.ForContentType<IFlow>();
            var scheduleCache = contentCacheFactory.ForContentType<IServerSchedule>(true);

            foreach (var item in items)
            {
                if (item.Flow?.Id == null || item.Flow.Id == Guid.Empty)
                {
                    logger.LogWarning("Flow reference not found for flow run task {TaskId}", item.Id);
                    continue;
                }

                if (item.Schedule?.Id == null || item.Schedule.Id == Guid.Empty)
                {
                    logger.LogWarning("Schedule reference not found for flow run task {TaskId}", item.Id);
                    continue;
                }

                var flowReference = await flowFinder.FindByIdAsync(item.Flow.Id, cancel).ConfigureAwait(false);
                var schedule = await scheduleCache.ForIdAsync(item.Schedule.Id, cancel).ConfigureAwait(false);

                /*
                 * Flow reference is null when the referenced flow
                 * is in a private space or other "pre-manifest" filter.
                 * 
                 * We similarly filter out those flow run tasks.
                 */
                if (flowReference is not null && schedule is not null)
                {
                    tasks.Add(new ServerFlowRunTask(
                        item.Id,
                        item.Type ?? string.Empty,
                        item.Priority,
                        item.ConsecutiveFailedCount,
                        flowReference,
                        schedule));
                }
            }

            return tasks.ToImmutable();
        }
    }
}
