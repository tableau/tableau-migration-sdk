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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters.Default
{
    /// <summary>
    /// Migration filter that skips server subscriptions for workbook or view content types
    /// if the destination content reference cannot be found, based on the same logic used in ServerSubscriptionTransformer.
    /// </summary>
    public sealed class ServerSubscriptionFilter : AsyncContentFilterBase<IServerSubscription>
    {
        private readonly IDestinationContentReferenceFinder<IWorkbook> _destinationWorkbookFinder;
        private readonly IDestinationViewReferenceFinder _destinationViewFinder;

        /// <summary>
        /// Creates a new <see cref="ServerSubscriptionFilter"/> object.
        /// </summary>
        /// <param name="destinationWorkbookFinder">The destination workbook finder.</param>
        /// <param name="destinationViewFinder">The destination view finder.</param>
        /// <param name="localizer">The shared resource localizer.</param>
        /// <param name="logger">The logger.</param>
        public ServerSubscriptionFilter(
            IDestinationContentReferenceFinder<IWorkbook> destinationWorkbookFinder,
            IDestinationViewReferenceFinder destinationViewFinder,
            ISharedResourcesLocalizer localizer,
            ILogger<IContentFilter<IServerSubscription>> logger)
            : base(localizer, logger)
        {
            _destinationWorkbookFinder = destinationWorkbookFinder;
            _destinationViewFinder = destinationViewFinder;
        }

        /// <inheritdoc />
        public override async Task<bool> ShouldMigrateAsync(ContentMigrationItem<IServerSubscription> item, CancellationToken cancel)
        {
            var subscription = item.SourceItem;
            var contentType = subscription.Content.Type.ToLowerInvariant();

            switch (contentType)
            {
                case "view":
                    var viewResult = await _destinationViewFinder.FindBySourceIdAsync(subscription.Content.Id, cancel).ConfigureAwait(false);
                    return viewResult is not null && viewResult.Success;
                case "workbook":
                    var workbookResult = await _destinationWorkbookFinder.FindBySourceIdAsync(subscription.Content.Id, cancel).ConfigureAwait(false);
                    return workbookResult is not null;
                default:
                    // For unsupported content types, skip the subscription
                    return false;
            }
        }
    }
}
