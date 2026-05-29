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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that maps the flow reference from a given flow run task to the destination flow.
    /// </summary>
    public sealed class FlowRunTaskFlowIdTransformer : ContentTransformerBase<ICloudFlowRunTask>
    {
        private readonly IDestinationContentReferenceFinder<IFlow> _flowFinder;

        /// <summary>
        /// Creates a new <see cref="FlowRunTaskFlowIdTransformer"/> object.
        /// </summary>
        /// <param name="destinationFinderFactory">Destination content finder factory object.</param>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">The logger used to log messages.</param>
        public FlowRunTaskFlowIdTransformer(
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            ISharedResourcesLocalizer localizer,
            ILogger<FlowRunTaskFlowIdTransformer> logger)
            : base(localizer, logger)
        {
            _flowFinder = destinationFinderFactory.ForDestinationContentType<IFlow>();
        }

        /// <inheritdoc />
        public override async Task<ICloudFlowRunTask?> TransformAsync(
            ICloudFlowRunTask flowRunTask,
            CancellationToken cancel)
        {
            if (flowRunTask.Flow is null)
            {
                return flowRunTask;
            }

            var destinationFlow = (await _flowFinder.FindBySourceIdAsync(flowRunTask.Flow.Id, cancel).ConfigureAwait(false))
                .ThrowOnMissingContentReference<IFlow>(Localizer, "flow run task flow", flowRunTask.Flow.Location);

            flowRunTask.Flow = destinationFlow;
            return flowRunTask;
        }
    }
}
