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
using Tableau.Migration.Config;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers;

namespace Tableau.Migration.Engine.Preparation
{
    /// <summary>
    /// <see cref="IContentItemPreparer{TContent, TPublish}"/> implementation that pulls
    /// the publish item from the source endpoint.
    /// </summary>
    public class ExtractRefreshTaskServerToCloudPreparer
        : EndpointContentItemPreparer<IServerExtractRefreshTask, ICloudExtractRefreshTask>
    {
        private readonly IDestinationApiEndpoint? _destinationApi;
        private readonly IConfigReader _configReader;
        private readonly VolatileCache<(ExtractRefreshContentType, Guid), ImmutableList<ICloudExtractRefreshTask>> _destinationExtractRefreshTasksCache;

        /// <summary>
        /// Creates a new <see cref="ExtractRefreshTaskServerToCloudPreparer"/> object.
        /// </summary>
        /// <param name="source">The source endpoint.</param>
        /// <param name="destination">The destination endpoint.</param>
        /// <param name="transformerRunner"><inheritdoc /></param>
        /// <param name="destinationFinderFactory"><inheritdoc /></param>
        /// <param name="configReader">A config reader.</param>
        public ExtractRefreshTaskServerToCloudPreparer(
            ISourceEndpoint source,
            IDestinationEndpoint destination,
            IContentTransformerRunner transformerRunner,
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            IConfigReader configReader)
            : base(
                  source, 
                  transformerRunner,
                  destinationFinderFactory)
        {
            if (destination is IDestinationApiEndpoint destinationApi)
            {
                _destinationApi = destinationApi;
            }
            
            _configReader = configReader;
            _destinationExtractRefreshTasksCache = new(
                async cancel =>
                {
                    var result = await _destinationApi!
                        .SiteApi
                        .CloudTasks
                        .GetAllExtractRefreshTasksAsync(cancel)
                        .ConfigureAwait(false);

                    if (!result.Success)
                    {
                        return new();
                    }

                    return result
                        .Value
                        .GroupBy(item => (item.ContentType, item.Content.Id))
                        .ToDictionary(group => group.Key, group => group.ToImmutableList());
                });
        }

        /// <inheritdoc />
        protected override async Task<IResult<ICloudExtractRefreshTask>> TransformAsync(
            ICloudExtractRefreshTask publishItem, 
            CancellationToken cancel)
        {
            var result = await base.TransformAsync(publishItem, cancel).ConfigureAwait(false);

            if (result.Success)
            {
                await CleanExtractRefreshTasks(
                    publishItem.ContentType,
                    publishItem.Content.Id,
                    cancel)
                    .ConfigureAwait(false);
            }

            return result;
        }

        private async Task CleanExtractRefreshTasks(
            ExtractRefreshContentType contentType,
            Guid contentId,
            CancellationToken cancel)
        {
            if (_destinationApi is null)
            {
                return;
            }

            var items = await _destinationExtractRefreshTasksCache
                .GetAndRelease(
                    (contentType, contentId),
                    cancel)
                .ConfigureAwait(false);

            if (items is null)
            {
                return;
            }

            await Parallel
                .ForEachAsync(
                    items,
                    new ParallelOptions
                    {
                        CancellationToken = cancel,
                        MaxDegreeOfParallelism = _configReader.Get().MigrationParallelism
                    },
                    async (item, itemCancel) =>
                    {
                        await _destinationApi
                            .SiteApi
                            .CloudTasks
                            .DeleteExtractRefreshTaskAsync(
                                item.Id, 
                                cancel)
                            .ConfigureAwait(false);
                    })
                .ConfigureAwait(false);
        }
    }
}
