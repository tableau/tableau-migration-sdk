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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Content.Schedules.Server
{
    /// <summary>
    /// Class for Server extract refresh task.
    /// </summary>
    internal sealed class ServerExtractRefreshTask :
        ExtractRefreshTaskBase<IServerSchedule>, IServerExtractRefreshTask
    {
        internal ServerExtractRefreshTask(
            Guid extractRefreshId,
            string type,
            ExtractRefreshContentType contentType,
            IContentReference content,
            IServerSchedule schedule)
            : base(
                extractRefreshId,
                type,
                contentType,
                content,
                schedule)
        { }

        public static async Task<IImmutableList<IServerExtractRefreshTask>> CreateManyAsync(
            ExtractRefreshTasksResponse response,
            IContentReferenceFinderFactory finderFactory,
            IContentCacheFactory contentCacheFactory,
            ILogger logger, ISharedResourcesLocalizer localizer,
            CancellationToken cancel)
            => await CreateManyAsync(
                response,
                response => response.Items.ExceptNulls(i => i.ExtractRefresh),
                async (r, c, cnl) => await CreateAsync(r, c, contentCacheFactory, cnl).ConfigureAwait(false),
                finderFactory, logger, localizer,
                cancel)
                .ConfigureAwait(false);

        private static async Task<IServerExtractRefreshTask> CreateAsync(
            IServerExtractRefreshType response,
            IContentReference content,
            IContentCacheFactory contentCacheFactory,
            CancellationToken cancel)
        {
            var scheduleCache = contentCacheFactory.ForContentType<IServerSchedule>(true);

            var schedule = await scheduleCache.ForIdAsync(response.Schedule.Id, cancel).ConfigureAwait(false);

            Guard.AgainstNull(schedule, nameof(schedule));

            var taskFromCache = schedule.ExtractRefreshTasks.Where(tsk => tsk.Id == response.Id).FirstOrDefault();

            return new ServerExtractRefreshTask(
                response.Id,
                taskFromCache== null ? string.Empty: taskFromCache.Type,
                response.GetContentType(),
                content,
                schedule);
        }
    }
}
