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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that changes the workbook reference for a content item. 
    /// It sets the workbook reference to that of the destination.
    /// </summary>
    public class WorkbookReferenceTransformer<TContent>
        : ContentTransformerBase<TContent>
        where TContent : IWithWorkbook
    {
        private readonly IDestinationContentReferenceFinder<IWorkbook> _workbookFinder;

        /// <summary>
        /// Creates a new <see cref="WorkbookReferenceTransformer{TContent}"/> object.
        /// </summary>
        /// <param name="destinationFinderFactory">The destination finder factory.</param>
        /// <param name="logger">The logger used to log messages.</param>
        /// <param name="localizer">The string localizer.</param>        
        public WorkbookReferenceTransformer(
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            ILogger<WorkbookReferenceTransformer<TContent>> logger,
            ISharedResourcesLocalizer localizer)
            : base(localizer, logger)
        {
            _workbookFinder = destinationFinderFactory.ForDestinationContentType<IWorkbook>();
        }

        /// <inheritdoc/>
        public override async Task<TContent?> TransformAsync(TContent ctx, CancellationToken cancel)
        {
            var destinationWorkbook = await _workbookFinder.FindBySourceLocationAsync(
                ctx.Workbook.Location,
                cancel)
                .ConfigureAwait(false);

            if (destinationWorkbook is not null)
            {
                ctx.Workbook = destinationWorkbook;
                return ctx;
            }

            Logger.LogDebug(Localizer[SharedResourceKeys.SourceWorkbookNotFoundLogMessage], ctx.Name, ctx.Id);
            return ctx;
        }
    }
}
