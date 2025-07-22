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
using Tableau.Migration.Api;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Caching;

namespace Tableau.Migration.Engine.Endpoints.Caching
{
    /// <summary>
    /// <see cref="IEndpointViewCache"/> implementation that retrieves views through the Tableau API as a fallback.
    /// </summary>
    public class TableauApiEndpointViewCache : ConfigurableMigrationCacheBase<Guid, IView>, IEndpointViewCache
    {
        internal const string CACHE_KEY = "views";

        private readonly IViewsApiClient _viewsApiClient;

        /// <summary>
        /// Creates a new <see cref="TableauApiEndpointViewCache"/> object.
        /// </summary>
        /// <param name="config">The configuration reader.</param>
        /// <param name="viewsApiClient">The views API client.</param>
        public TableauApiEndpointViewCache(IConfigReader config, IViewsApiClient viewsApiClient)
            : base(config, CACHE_KEY)
        {
            _viewsApiClient = viewsApiClient;
        }

        /// <inheritdoc />
        protected override async Task<IResult<IView>> FindCacheMissAsync(Guid key, CancellationToken cancel)
            => await _viewsApiClient.GetByIdAsync(key, cancel).ConfigureAwait(false);
    }
}
