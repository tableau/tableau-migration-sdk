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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Content.Schedules.Cloud
{
    internal sealed class CloudExtractRefreshTask :
        ExtractRefreshTaskBase<ICloudSchedule>, ICloudExtractRefreshTask
    {
        internal CloudExtractRefreshTask(
            Guid extractRefreshId,
            string type,
            ExtractRefreshContentType contentType,
            IContentReference contentReference,
            ICloudSchedule schedule)
            : base(extractRefreshId, type, contentType, contentReference, schedule)
        { }

        public static async Task<IImmutableList<ICloudExtractRefreshTask>> CreateManyAsync(
            ExtractRefreshTasksResponse response,
            IContentReferenceFinderFactory finderFactory,
            ILogger logger, ISharedResourcesLocalizer localizer,
            CancellationToken cancel)
            => await CreateManyAsync(
                response,
                response => response.Items.ExceptNulls(i => i.ExtractRefresh),
                (r, c, cnl) => Task.FromResult(Create(r, r.Schedule, c)),
                finderFactory, logger, localizer,
                cancel)
                .ConfigureAwait(false);

        public static ICloudExtractRefreshTask Create(
            IExtractRefreshType response,
            ICloudScheduleType schedule,
            IContentReference content)
        {
            return new CloudExtractRefreshTask(
                    response.Id,
                    response.Type!,
                    response.GetContentType(),
                    content,
                    new CloudSchedule(schedule));
        }
    }
}
