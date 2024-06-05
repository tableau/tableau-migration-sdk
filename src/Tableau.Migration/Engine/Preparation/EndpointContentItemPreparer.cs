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
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers;

namespace Tableau.Migration.Engine.Preparation
{
    /// <summary>
    /// <see cref="IContentItemPreparer{TContent, TPublish}"/> implementation that pulls
    /// the publish item from the source endpoint.
    /// </summary>
    /// <typeparam name="TContent"><inheritdoc /></typeparam>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    public class EndpointContentItemPreparer<TContent, TPublish> : ContentItemPreparerBase<TContent, TPublish>
        where TPublish : class
    {
        private readonly ISourceEndpoint _source;

        /// <summary>
        /// Creates a new <see cref="EndpointContentItemPreparer{TContent, TPublish}"/> object.
        /// </summary>
        /// <param name="source">The source endpoint.</param>
        /// <param name="transformerRunner"><inheritdoc /></param>
        /// <param name="destinationFinderFactory"><inheritdoc /></param>
        public EndpointContentItemPreparer(
            ISourceEndpoint source,
            IContentTransformerRunner transformerRunner, 
            IDestinationContentReferenceFinderFactory destinationFinderFactory)
            : base(transformerRunner, destinationFinderFactory)
        {
            _source = source;
        }

        /// <inheritdoc />
        protected override async Task<IResult<TPublish>> PullAsync(ContentMigrationItem<TContent> item, CancellationToken cancel)
        {
            return await _source.PullAsync<TContent, TPublish>(item.SourceItem, cancel).ConfigureAwait(false);
        }
    }
}
