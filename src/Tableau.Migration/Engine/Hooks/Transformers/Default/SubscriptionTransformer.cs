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

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that updates the subscription view from the source id to the destination id
    /// </summary>
    internal sealed class SubscriptionTransformer : ContentTransformerBase<ICloudSubscription>
    {
        private readonly IDestinationContentReferenceFinder<IWorkbook> _destinationWorkbookFinder;
        private readonly IDestinationViewReferenceFinder _destinationViewFinder;

        public SubscriptionTransformer(
            IDestinationContentReferenceFinder<IWorkbook> destinationWorkbookFinder,
            IDestinationViewReferenceFinder destinationViewFinder,
            ISharedResourcesLocalizer localizer, ILogger<SubscriptionTransformer> logger)
            : base(localizer, logger)
        {
            _destinationWorkbookFinder = destinationWorkbookFinder;
            _destinationViewFinder = destinationViewFinder;
        }

        public override async Task<ICloudSubscription?> TransformAsync(ICloudSubscription itemToTransform, CancellationToken cancel)
        {
            IContentReference destinationItem;

            switch (itemToTransform.Content.Type.ToLowerInvariant())
            {
                case "view":
                    destinationItem = (await _destinationViewFinder.FindBySourceIdAsync(itemToTransform.Content.Id, cancel).ConfigureAwait(false))
                        .ThrowOnMissingContentReference<IView>(Localizer, "subscription view", itemToTransform.Content.Id);
                    break;
                case "workbook":
                    destinationItem = (await _destinationWorkbookFinder.FindBySourceIdAsync(itemToTransform.Content.Id, cancel).ConfigureAwait(false))
                        .ThrowOnMissingContentReference<IWorkbook>(Localizer, "subscription workbook", itemToTransform.Content.Id);
                    break;
                default:
                    throw new NotSupportedException($"Unable to transform unsupported subscription content reference type: {itemToTransform.Content.Type}. Update {nameof(SubscriptionTransformer)} to support the content reference type.");
            }

            itemToTransform.Content = itemToTransform.Content.ForReference(destinationItem);
            return itemToTransform;
        }
    }
}
