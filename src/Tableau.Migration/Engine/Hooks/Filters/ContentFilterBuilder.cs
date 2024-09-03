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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Default <see cref="IContentFilterBuilder"/> implementation.
    /// </summary>
    public class ContentFilterBuilder : ContentTypeHookBuilderBase, IContentFilterBuilder
    {
        #region - IContentFilterBuilder Implementation -

        /// <inheritdoc />
        public virtual IContentFilterBuilder Clear()
        {
            ClearFactories();
            return this;
        }

        /// <inheritdoc />
        new public virtual IContentFilterBuilder Add(Type genericFilterType, IEnumerable<Type[]> contentTypes)
            => (IContentFilterBuilder)base.Add(genericFilterType, contentTypes);

        /// <inheritdoc />
        public virtual IContentFilterBuilder Add<TContent>(IContentFilter<TContent> filter)
            where TContent : IContentReference
            => (IContentFilterBuilder)Add(typeof(IContentFilter<TContent>), s => filter);

        /// <inheritdoc />
        public virtual IContentFilterBuilder Add<TFilter, TContent>(Func<IServiceProvider, TFilter>? filterFactory = null)
            where TFilter : IContentFilter<TContent>
            where TContent : IContentReference
        {
            filterFactory ??= services => services.GetRequiredService<TFilter>();
            return (IContentFilterBuilder)Add(typeof(IContentFilter<TContent>), s => filterFactory(s));
        }

        /// <inheritdoc />
        public virtual IContentFilterBuilder Add<TContent>(Func<IEnumerable<ContentMigrationItem<TContent>>, CancellationToken, Task<IEnumerable<ContentMigrationItem<TContent>>?>> callback)
            where TContent : IContentReference
            => (IContentFilterBuilder)Add(typeof(IContentFilter<TContent>),
                   s => new CallbackHookWrapper<IContentFilter<TContent>, IEnumerable<ContentMigrationItem<TContent>>>(callback));

        /// <inheritdoc />
        public IContentFilterBuilder Add<TContent>(Func<IEnumerable<ContentMigrationItem<TContent>>, IEnumerable<ContentMigrationItem<TContent>>?> callback)
            where TContent : IContentReference
            => Add<TContent>((ctx, cancel) => Task.FromResult(callback(ctx)));

        #endregion
    }
}
