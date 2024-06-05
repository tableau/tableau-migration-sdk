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
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers;

namespace Tableau.Migration.Engine.Preparation
{
    /// <summary>
    /// <see cref="IContentItemPreparer{TContent, TPublish}"/> implementation that publishes the source item as-is
    /// and does not require extra pulled information.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class SourceContentItemPreparer<TContent> : ContentItemPreparerBase<TContent, TContent>
        where TContent : class
    {
        /// <summary>
        /// Creates a new <see cref="SourceContentItemPreparer{TContent}"/>.
        /// </summary>
        /// <param name="transformerRunner"><inheritdoc /></param>
        /// <param name="destinationFinderFactory"><inheritdoc /></param>
        public SourceContentItemPreparer(
            IContentTransformerRunner transformerRunner,
            IDestinationContentReferenceFinderFactory destinationFinderFactory)
            : base(transformerRunner, destinationFinderFactory)
        { }

        /// <inheritdoc />
        protected override Task<IResult<TContent>> PullAsync(ContentMigrationItem<TContent> item, CancellationToken cancel)
        {
            var result = Result<TContent>.Succeeded(item.SourceItem);
            return Task.FromResult<IResult<TContent>>(result);
        }
    }
}
