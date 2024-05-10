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
    /// Hook that updates a content item's owner after publish.
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    /// <typeparam name="TResult"><inheritdoc /></typeparam>
    public class OwnerItemPostPublishHook<TPublish, TResult> : ContentItemPostPublishHookBase<TPublish, TResult>
        where TPublish : IRequiresOwnerUpdate
        where TResult : IWithOwner
    {
        private readonly IMigration _migration;

        /// <summary>
        /// Creates a new <see cref="OwnerItemPostPublishHook{TPublish, TDestination}"/> object.
        /// </summary>
        /// <param name="migration">The current migration</param>
        public OwnerItemPostPublishHook(IMigration migration)
        {
            _migration = migration;
        }

        /// <inheritdoc/>
        public override async Task<ContentItemPostPublishContext<TPublish, TResult>?> ExecuteAsync(ContentItemPostPublishContext<TPublish, TResult> ctx, CancellationToken cancel)
        {
            //Don't apply ownership if its a no-op
            //or if the source object has system ownership, which we can't assign on the destination.
            if (ctx.PublishedItem.Owner.Id == ctx.DestinationItem.Owner.Id
                || ctx.PublishedItem.Owner.Location == Constants.SystemUserLocation)
            {
                return ctx;
            }

            //The owner has already been mapped by our default OwnershipTransformer.
            var updateOwnerResult = await _migration.Destination.UpdateOwnerAsync<TPublish>(ctx.DestinationItem, ctx.PublishedItem.Owner, cancel)
                .ConfigureAwait(false);

            if (!updateOwnerResult.Success)
            {
                ctx.ManifestEntry.SetFailed(updateOwnerResult.Errors);
            }

            return ctx;
        }
    }
}
