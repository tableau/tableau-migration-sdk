//
//  Copyright (c) 2026, Salesforce, Inc.
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

using System.Collections.Immutable;

namespace Tableau.Migration.Content.Search
{
    /// <summary>
    /// Record representing the result of loading content references into a cache.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <param name="Items">The items that were loaded.</param>
    /// <param name="IsSupported">True if the search mode was supported by the store, otherwise false.</param>
    public readonly record struct ContentReferenceLoadResult<TContent>
        (IImmutableList<TContent> Items, bool IsSupported)
        where TContent : IContentReference
    {
        /// <summary>
        /// The result for when the search mode was unsupported, leading to the load being canceled.
        /// </summary>
        public static ContentReferenceLoadResult<TContent> Unsupported = new([], false);

        /// <summary>
        /// The result for when the search mode was supported but did not find any results.
        /// </summary>
        public static ContentReferenceLoadResult<TContent> Empty = new([], true);

        /// <summary>
        /// Creates a new <see cref="ContentReferenceLoadResult{TContent}"/> object.
        /// </summary>
        /// <param name="items">The found items.</param>
        public ContentReferenceLoadResult(IImmutableList<TContent> items)
            : this(items, true)
        { }
    }
}
