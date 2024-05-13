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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.PostPublish
{
    /// <summary>
    /// Context for <see cref="BulkPostPublishContext{TPublish}"/> operations
    /// for published content items.
    /// </summary>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    public class BulkPostPublishContext<TPublish>
    {
        /// <summary>
        /// Gets the content item being published.
        /// </summary>
        public IImmutableList<TPublish> PublishedItems { get; }

        /// <summary>
        /// Creates a new <see cref="BulkPostPublishContext{TPublish}"/> object.
        /// </summary>
        /// <param name="sourceItems">The source content items.</param>
        public BulkPostPublishContext(IEnumerable<TPublish> sourceItems)
        {
            PublishedItems = sourceItems.ToImmutableArray();
        }

        /// <summary>
        /// Creates a task that's successfully completed from the current context.
        /// </summary>
        /// <returns>The successfully completed task.</returns>
        public Task<BulkPostPublishContext<TPublish>?> ToTask()
            => Task.FromResult<BulkPostPublishContext<TPublish>?>(this);
    }
}
