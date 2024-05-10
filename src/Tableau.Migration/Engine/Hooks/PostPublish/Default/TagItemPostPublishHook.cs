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
using Tableau.Migration.Content;

namespace Tableau.Migration.Engine.Hooks.PostPublish.Default
{
    /// <summary>
    /// Tagged content post publish hook. 
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    /// <typeparam name="TResult"><inheritdoc /></typeparam>
    public class TagItemPostPublishHook<TPublish, TResult> : ContentItemPostPublishHookBase<TPublish, TResult>
        where TPublish : IWithTags
        where TResult : IContentReference
    {
        private readonly IMigration _migration;

        /// <summary>
        /// Creates a new <see cref="TagItemPostPublishHook{TPublish, TDestination}"/> object.
        /// </summary>
        /// <param name="migration">The current migration</param>
        public TagItemPostPublishHook(IMigration migration)
        {
            _migration = migration;
        }

        /// <inheritdoc/>
        public override async Task<ContentItemPostPublishContext<TPublish, TResult>?> ExecuteAsync(ContentItemPostPublishContext<TPublish, TResult> ctx, CancellationToken cancel)
        {
            var updateTagsResult = await _migration.Destination.UpdateTagsAsync<TPublish>(ctx.DestinationItem, ctx.PublishedItem.Tags, cancel)
                .ConfigureAwait(false);

            if (!updateTagsResult.Success)
            {
                ctx.ManifestEntry.SetFailed(updateTagsResult.Errors);
            }

            return ctx;
        }
    }
}
