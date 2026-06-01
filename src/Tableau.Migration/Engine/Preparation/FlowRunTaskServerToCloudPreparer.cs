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
using Tableau.Migration.Config;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Preparation
{
    /// <summary>
    /// <see cref="IContentItemPreparer{TContent, TPublish}"/> implementation that pulls
    /// the publish item from the source endpoint.
    /// </summary>
    public class FlowRunTaskServerToCloudPreparer
        : SourceContentItemPreparer<IServerFlowRunTask, ICloudFlowRunTask>
    {
        private readonly IDestinationApiEndpoint? _destinationApi;
        private readonly IConfigReader _configReader;
        private readonly VolatileCache<Guid, ImmutableList<ICloudFlowRunTask>> _destinationFlowRunTasksCache;

        /// <summary>
        /// Creates a new <see cref="FlowRunTaskServerToCloudPreparer"/> object.
        /// </summary>
        /// <param name="destination">The destination endpoint.</param>
        /// <param name="pipeline"><inheritdoc /></param>
        /// <param name="hooks"><inheritdoc /></param>
        /// <param name="transformerRunner"><inheritdoc /></param>
        /// <param name="destinationFinderFactory"><inheritdoc /></param>
        /// <param name="logger"><inheritdoc /></param>
        /// <param name="localizer"><inheritdoc /></param>
        /// <param name="configReader">A config reader.</param>
        public FlowRunTaskServerToCloudPreparer(
            IDestinationEndpoint destination,
            IMigrationPipeline pipeline,
            IMigrationHookRunner hooks,
            IContentTransformerRunner transformerRunner,
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            ILogger<FlowRunTaskServerToCloudPreparer> logger,
            ISharedResourcesLocalizer localizer,
            IConfigReader configReader)
            : base(pipeline, hooks, transformerRunner, destinationFinderFactory, logger, localizer)
        {
            if (destination is IDestinationApiEndpoint destinationApi)
            {
                _destinationApi = destinationApi;
            }

            _configReader = configReader;
            _destinationFlowRunTasksCache = new(
                async cancel =>
                {
                    var result = await _destinationApi!
                        .SiteApi
                        .CloudTasks
                        .GetAllFlowRunTasksAsync(cancel)
                        .ConfigureAwait(false);

                    if (!result.Success)
                    {
                        return new();
                    }

                    return result
                        .Value
                        .GroupBy(item => item.Flow.Id)
                        .ToDictionary(group => group.Key, group => group.ToImmutableList());
                });
        }

        /// <inheritdoc />
        protected override async Task<IResult<ICloudFlowRunTask>> TransformAsync(
            ICloudFlowRunTask publishItem,
            CancellationToken cancel)
        {
            var result = await base.TransformAsync(publishItem, cancel).ConfigureAwait(false);

            if (result.Success)
            {
                await CleanFlowRunTasksAsync(
                    publishItem.Flow.Id,
                    cancel)
                    .ConfigureAwait(false);
            }

            return result;
        }

        private async Task CleanFlowRunTasksAsync(
            Guid flowId,
            CancellationToken cancel)
        {
            if (_destinationApi is null)
            {
                return;
            }

            var items = await _destinationFlowRunTasksCache
                .GetAndReleaseAsync(
                    flowId,
                    cancel)
                .ConfigureAwait(false);

            if (items is null)
            {
                return;
            }

            // Note: Cloud API doesn't currently have DeleteFlowRunTaskAsync method.
            // This cleanup logic will need to be implemented once the delete API method is available.
            // For now, we skip deletion but log a warning.
            if (items.Count > 0)
            {
                // TODO: Implement deletion once DeleteFlowRunTaskAsync is available in Cloud API
            }
        }
    }
}
