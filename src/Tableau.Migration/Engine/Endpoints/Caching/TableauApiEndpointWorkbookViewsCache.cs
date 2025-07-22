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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Caching;

namespace Tableau.Migration.Engine.Endpoints.Caching
{
    /// <summary>
    /// <see cref="IEndpointWorkbookViewsCache"/> implementation that retrieves workbook views through the Tableau API as a fallback.
    /// </summary>
    public class TableauApiEndpointWorkbookViewsCache : ConfigurableMigrationCacheBase<Guid, IImmutableList<IView>>, IEndpointWorkbookViewsCache
    {
        internal const string CACHE_KEY = "workbookViews";

        private readonly IWorkbooksApiClient _workbooksApiClient;

        /// <summary>
        /// Creates a new <see cref="TableauApiEndpointWorkbookViewsCache"/> object.
        /// </summary>
        /// <param name="workbooksApiClient">The workbook API client.</param>
        /// <param name="config">The config reader.</param>
        public TableauApiEndpointWorkbookViewsCache(IWorkbooksApiClient workbooksApiClient, IConfigReader config)
            : base(config, CACHE_KEY)
        {
            _workbooksApiClient = workbooksApiClient;
        }

        /// <inheritdoc />
        protected override async Task<IResult<IImmutableList<IView>>> FindCacheMissAsync(Guid key, CancellationToken cancel)
        {
            var workbook = await _workbooksApiClient.GetWorkbookAsync(key, cancel).ConfigureAwait(false);

            if (!workbook.Success)
            {
                return workbook.CastFailure<IImmutableList<IView>>();
            }

            return Result<IImmutableList<IView>>.Succeeded(workbook.Value.Views);
        }
    }
}
