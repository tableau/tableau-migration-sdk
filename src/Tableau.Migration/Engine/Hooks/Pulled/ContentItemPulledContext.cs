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

using System.Threading.Tasks;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Engine.Hooks.Pulled
{
    /// <summary>
    /// Context for <see cref="IContentItemPulledHook{TPrepare}"/> operations
    /// for pulled content items.
    /// </summary>
    /// <typeparam name="TPrepare">The content type that has been pulled and will be prepared.</typeparam>
    public class ContentItemPulledContext<TPrepare> : IContentItemFilterResult
        where TPrepare : IContentReference
    {
        /// <summary>
        /// Gets the manifest entry for the content item.
        /// </summary>
        public IMigrationManifestEntryEditor ManifestEntry { get; }

        /// <summary>
        /// Gets the content item that was pulled.
        /// </summary>
        public TPrepare PulledItem { get; }

        /// <summary>
        /// Gets or sets the current filtering status for the pulled content item.
        /// </summary>
        public FilterStatus Status { get; set; } = FilterStatus.Migrate;

        /// <summary>
        /// Creates a new <see cref="ContentItemPulledContext{TPrepare}"/> object.
        /// </summary>
        /// <param name="manifestEntry">The manifest entry for the content item.</param>
        /// <param name="pulledItem">The content item that was pulled.</param>
        public ContentItemPulledContext(IMigrationManifestEntryEditor manifestEntry, TPrepare pulledItem)
        {
            ManifestEntry = manifestEntry;
            PulledItem = pulledItem;
        }

        /// <summary>
        /// Creates a task that's successfully completed from the current context.
        /// </summary>
        /// <returns>The successfully completed task.</returns>
        public Task<ContentItemPulledContext<TPrepare>?> ToTask()
            => Task.FromResult<ContentItemPulledContext<TPrepare>?>(this);
    }
}
