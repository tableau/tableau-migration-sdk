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

using System.Threading.Tasks;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Hooks.PostPublish
{
    /// <summary>
    /// Context for <see cref="ContentItemPostPublishHookBase{TSource, TDestination}"/> operations
    /// for published content items.
    /// </summary>
    /// <typeparam name="TPublish">The publish content type.</typeparam>
    /// <typeparam name="TResult">The post-publish result type.</typeparam>
    public class ContentItemPostPublishContext<TPublish, TResult>
    {
        /// <summary>
        /// Gets the manifest entry for the content item.
        /// </summary>
        public IMigrationManifestEntryEditor ManifestEntry { get; }

        /// <summary>
        /// Gets the content item being published.
        /// </summary>
        public TPublish PublishedItem { get; }

        /// <summary>
        /// Gets the returned content item after publishing.
        /// </summary>
        public TResult DestinationItem { get; }

        /// <summary>
        /// Creates a new <see cref="ContentItemPostPublishContext{TSource, TResult}"/> object.
        /// </summary>
        /// <param name="manifestEntry">The manifest entry for the content item.</param>
        /// <param name="publishedItem">The content item being published.</param>
        /// <param name="destinationItem">The returned content item after publishing.</param>
        public ContentItemPostPublishContext(IMigrationManifestEntryEditor manifestEntry, TPublish publishedItem, TResult destinationItem)
        {
            ManifestEntry = manifestEntry;
            PublishedItem = publishedItem;
            DestinationItem = destinationItem;
        }

        /// <summary>
        /// Creates a task that's successfully completed from the current context.
        /// </summary>
        /// <returns>The successfully completed task.</returns>
        public Task<ContentItemPostPublishContext<TPublish, TResult>?> ToTask()
            => Task.FromResult<ContentItemPostPublishContext<TPublish, TResult>?>(this);
    }
}
