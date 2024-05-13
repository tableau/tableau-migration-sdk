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

namespace Tableau.Migration.Engine.Hooks.PostPublish
{
    /// <summary>
    /// Base class for <see cref="IContentItemPostPublishHook{TSource, TDestination}"/> implementations.
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    /// <typeparam name="TResult"><inheritdoc /></typeparam>
    public abstract class ContentItemPostPublishHookBase<TPublish, TResult> : IContentItemPostPublishHook<TPublish, TResult>
    {
        /// <inheritdoc/>
        public abstract Task<ContentItemPostPublishContext<TPublish, TResult>?> ExecuteAsync(
            ContentItemPostPublishContext<TPublish, TResult> ctx,
            CancellationToken cancel);
    }

    /// <summary>
    /// Base class for <see cref="IContentItemPostPublishHook{TSource, TDestination}"/> implementations.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public abstract class ContentItemPostPublishHookBase<TContent> : ContentItemPostPublishHookBase<TContent, TContent>
    { }
}
