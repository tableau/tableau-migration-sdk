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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.ContentClients;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that updates the subscription view from the source id to the destination id
    /// </summary>
    internal class SubscriptionTransformer : ContentTransformerBase<ICloudSubscription>
    {
        private readonly IDestinationContentReferenceFinder<IWorkbook> _destinationContentReferenceFinder;
        private readonly IViewsContentClient _sourceViewClient;
        private readonly IWorkbooksContentClient _destinationWorkbookClient;

        public SubscriptionTransformer(
            ISourceEndpoint sourceEndpoint,
            IDestinationEndpoint destinationEndpoint,
            IDestinationContentReferenceFinder<IWorkbook> destinationContentReferenceFinder,
            ISharedResourcesLocalizer localizer,
            ILogger<SubscriptionTransformer> logger) : base(localizer, logger)
        {
            _destinationContentReferenceFinder = destinationContentReferenceFinder;
            _sourceViewClient = (IViewsContentClient)sourceEndpoint.GetContentClient<IView>();
            _destinationWorkbookClient = (IWorkbooksContentClient)destinationEndpoint.GetContentClient<IWorkbook>();
        }

        public override async Task<ICloudSubscription?> TransformAsync(ICloudSubscription itemToTransform, CancellationToken cancel)
        {
            switch (itemToTransform.Content.Type.ToLowerInvariant())
            {
                case "view":
                    return await TransformViewSubscriptionAsync(itemToTransform, cancel).ConfigureAwait(false);

                case "workbook":
                    return await TransformWorkbookSubscriptionAsync(itemToTransform, cancel).ConfigureAwait(false);

                default:
                    throw new NotSupportedException($"Unsupported subscription content type: {itemToTransform.Content.Type}");
            }
        }

        private async Task<ICloudSubscription?> TransformViewSubscriptionAsync(ICloudSubscription itemToTransform, CancellationToken cancel)
        {
            // 1. Get the workbook of the source ViewID
            // 2. Find the mapped location of the source workbook 
            // 3. Get the views of the destination (mapped) workbook
            // 4. Find the view with the same name as the source view

            var sourceView = await _sourceViewClient.GetByIdAsync(itemToTransform.Content.Id, cancel).ConfigureAwait(false);
            if (!sourceView.Success)
            {
                throw new Exception($"Unable to find source view with id {itemToTransform.Content.Id}.");
            }

            // Find the destination reference for the source workbook
            var destinationWorkbookReference = await _destinationContentReferenceFinder.FindBySourceIdAsync(sourceView.Value.ParentWorkbook.Id, cancel).ConfigureAwait(false)
                ?? throw new Exception($"Unable to find source workbook content reference for workbook id {sourceView.Value.ParentWorkbook.Id}.");

            var destinationViews = await _destinationWorkbookClient.GetViewsForWorkbookIdAsync(destinationWorkbookReference.Id, cancel).ConfigureAwait(false);
            if (!destinationViews.Success)
            {
                throw new Exception($"Unable to get views for destination workbook {destinationWorkbookReference.Location}");
            }

            var destinationView = destinationViews.Value.FirstOrDefault(v => v.Name == sourceView.Value.Name)
                ?? throw new Exception($"Unable to find destination view with name {sourceView.Value.Name} in destination workbook {destinationWorkbookReference.Location}.");

            itemToTransform.Content.Id = destinationView.Id;

            return itemToTransform;
        }

        private async Task<ICloudSubscription?> TransformWorkbookSubscriptionAsync(ICloudSubscription itemToTransform, CancellationToken cancel)
        {
            var destinationWorkbookReference = await _destinationContentReferenceFinder.FindBySourceIdAsync(itemToTransform.Content.Id, cancel).ConfigureAwait(false)
                ?? throw new Exception($"Unable to find destination workbook content reference for workbook id {itemToTransform.Content.Id}.");

            itemToTransform.Content.Id = destinationWorkbookReference.Id;

            return itemToTransform;
        }
    }
}
