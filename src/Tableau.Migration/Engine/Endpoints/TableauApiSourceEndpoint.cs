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
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// <see cref="ISourceEndpoint"/> implementation that uses Tableau Server/Cloud APIs.
    /// </summary>
    public class TableauApiSourceEndpoint : TableauApiEndpointBase, ISourceApiEndpoint
    {
        /// <summary>
        /// Creates a new <see cref="TableauApiSourceEndpoint"/> object.
        /// </summary>
        /// <param name="serviceScopeFactory">A service scope factory to define an API client scope with.</param>
        /// <param name="config">The configuration options for connecting to the source endpoint APIs.</param>
        /// <param name="finderFactory">A source manifest finder factory.</param>
        /// <param name="fileStore">The file store to use.</param>
        /// <param name="localizer">A string localizer.</param>
        public TableauApiSourceEndpoint(IServiceScopeFactory serviceScopeFactory,
            ITableauApiEndpointConfiguration config,
            ISourceContentReferenceFinderFactory finderFactory,
            IContentFileStore fileStore,
            ISharedResourcesLocalizer localizer)
            : base(serviceScopeFactory, config, finderFactory, fileStore, localizer)
        { }

        /// <inheritdoc />
        public async Task<IResult<TPublish>> PullAsync<TContent, TPublish>(TContent contentItem, CancellationToken cancel)
            where TPublish : class
        {
            var apiClient = SiteApi.GetPullApiClient<TContent, TPublish>();
            return await apiClient.PullAsync(contentItem, cancel).ConfigureAwait(false);
        }
    }
}
