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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
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
            switch (itemToTransform.Content.Type.ToLowerInvariant())
            {
                case "view":
                    await TransformViewSubscriptionAsync(itemToTransform, cancel).ConfigureAwait(false);
                    break;
                case "workbook":
                    await TransformWorkbookSubscriptionAsync(itemToTransform, cancel).ConfigureAwait(false);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported subscription content type: {itemToTransform.Content.Type}");
            }

            return itemToTransform;
        }

        private async Task TransformViewSubscriptionAsync(ICloudSubscription itemToTransform, CancellationToken cancel)
        {
            var destinationView = (await _destinationViewFinder.FindBySourceIdAsync(itemToTransform.Content.Id, cancel).ConfigureAwait(false))
                .ThrowOnMissingContentReference("Missing destination subscription view reference.");

            itemToTransform.Content.Id = destinationView.Id;
        }

        private async Task TransformWorkbookSubscriptionAsync(ICloudSubscription itemToTransform, CancellationToken cancel)
        {
            var destinationWorkbook = (await _destinationWorkbookFinder.FindBySourceIdAsync(itemToTransform.Content.Id, cancel).ConfigureAwait(false))
                .ThrowOnMissingContentReference("Missing destination subscription workbook reference.");

            itemToTransform.Content.Id = destinationWorkbook.Id;
        }
    }
}
