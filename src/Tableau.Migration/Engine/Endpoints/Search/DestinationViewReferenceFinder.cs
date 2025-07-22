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
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.ContentClients;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// Default <see cref="IDestinationViewReferenceFinder"/> implementation.
    /// </summary>
    public class DestinationViewReferenceFinder : IDestinationViewReferenceFinder
    {
        private readonly IViewsContentClient _sourceViewClient;

        private readonly IDestinationContentReferenceFinder<IWorkbook> _destinationWorkbookFinder;
        private readonly IWorkbooksContentClient _destinationWorkbookClient;

        /// <summary>
        /// Creates a new <see cref="DestinationViewReferenceFinder"/> object.
        /// </summary>
        /// <param name="sourceEndpoint">The source endpoint.</param>
        /// <param name="destinationEndpoint">The destination endpoint.</param>
        /// <param name="destinationWorkbookFinder">A destination workbook finder.</param>
        public DestinationViewReferenceFinder(ISourceEndpoint sourceEndpoint, IDestinationEndpoint destinationEndpoint,
            IDestinationContentReferenceFinder<IWorkbook> destinationWorkbookFinder)
        {
            _sourceViewClient = sourceEndpoint.GetViewsContentClient();

            _destinationWorkbookFinder = destinationWorkbookFinder;
            _destinationWorkbookClient = destinationEndpoint.GetWorkbookContentClient();
        }

        /// <inheritdoc />
        public async Task<IResult<IContentReference>> FindBySourceIdAsync(Guid sourceViewId, CancellationToken cancel)
        {
            /*
             * 1. Get the workbook of the source view ID.
             * 2. Find the mapped location of the source workbook. 
             * 3. Get the views of the destination (mapped) workbook.
             * 4. Find the view with the same name as the source view.
             */

            var sourceViewResult = await _sourceViewClient.GetByIdAsync(sourceViewId, cancel).ConfigureAwait(false);
            if (!sourceViewResult.Success)
            {
                return sourceViewResult.CastFailure<IContentReference>();
            }

            var sourceView = sourceViewResult.Value;

            // Find the destination reference for the source workbook.
            var destinationWorkbookReference = await _destinationWorkbookFinder.FindBySourceIdAsync(sourceView.ParentWorkbook.Id, cancel).ConfigureAwait(false);
            if(destinationWorkbookReference is null)
            {
                return Result<IContentReference>.Failed(new Exception($"Unable to find source workbook content reference for workbook ID {sourceView.ParentWorkbook.Id}."));
            }

            var destinationViewResult = await _destinationWorkbookClient.GetViewsForWorkbookIdAsync(destinationWorkbookReference.Id, cancel).ConfigureAwait(false);
            if (!destinationViewResult.Success)
            {
                return destinationViewResult.CastFailure<IContentReference>();
            }

            var destinationView = destinationViewResult.Value.FirstOrDefault(v => string.Equals(sourceView.Name, v.Name, StringComparison.Ordinal));
            if(destinationView is null)
            {
                return Result<IContentReference>.Failed(new Exception($"Unable to find destination view with name {sourceView.Name} in destination workbook {destinationWorkbookReference.Location}."));
            }

            return Result<IContentReference>.Succeeded(destinationView);
        }
    }
}
