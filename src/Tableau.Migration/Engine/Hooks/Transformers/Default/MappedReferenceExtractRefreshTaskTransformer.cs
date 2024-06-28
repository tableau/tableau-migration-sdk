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
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers.Default
{
    /// <summary>
    /// Transformer that maps the workbook/data source from a given extract refresh task.
    /// </summary>
    public class MappedReferenceExtractRefreshTaskTransformer 
        : ContentTransformerBase<ICloudExtractRefreshTask>
    {
        private readonly IDestinationContentReferenceFinder<IDataSource> _dataSourceFinder;
        private readonly IDestinationContentReferenceFinder<IWorkbook> _workbookFinder;

        /// <summary>
        /// Creates a new <see cref="MappedReferenceExtractRefreshTaskTransformer"/> object.
        /// </summary>
        /// <param name="destinationFinderFactory">Destination content finder factory object.</param>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">The logger used to log messages.</param>
        public MappedReferenceExtractRefreshTaskTransformer(
            IDestinationContentReferenceFinderFactory destinationFinderFactory,
            ISharedResourcesLocalizer localizer,
            ILogger<MappedReferenceExtractRefreshTaskTransformer> logger) 
            : base(localizer, logger)
        {
            _dataSourceFinder = destinationFinderFactory.ForDestinationContentType<IDataSource>();
            _workbookFinder = destinationFinderFactory.ForDestinationContentType<IWorkbook>();
        }

        /// <inheritdoc />
        public override async Task<ICloudExtractRefreshTask?> TransformAsync(
            ICloudExtractRefreshTask extractRefreshTask,
            CancellationToken cancel)
        {
            var destinationReference = await FindDestinationReferenceAsync(
                extractRefreshTask.ContentType,
                extractRefreshTask.Content,
                cancel)
                .ConfigureAwait(false);

            if (destinationReference is null)
            {
                Logger.LogWarning(
                    Localizer[SharedResourceKeys.MappedReferenceExtractRefreshTaskTransformerCannotFindReferenceWarning], 
                    extractRefreshTask.Id, 
                    extractRefreshTask.ContentType,
                    extractRefreshTask.Content);
            }
            else
            {
                extractRefreshTask.Content = destinationReference;
            }

            return extractRefreshTask;
        }

        private async Task<IContentReference?> FindDestinationReferenceAsync(
            ExtractRefreshContentType extractRefreshContentType,
            IContentReference sourceContentReference,
            CancellationToken cancel)
        {
            switch (extractRefreshContentType)
            {
                case ExtractRefreshContentType.Workbook:
                    return await _workbookFinder
                        .FindBySourceIdAsync(sourceContentReference.Id, cancel)
                        .ConfigureAwait(false);
                case ExtractRefreshContentType.DataSource:
                    return await _dataSourceFinder
                        .FindBySourceIdAsync(sourceContentReference.Id, cancel)
                        .ConfigureAwait(false);
            }

            return null;
        }
    }
}
