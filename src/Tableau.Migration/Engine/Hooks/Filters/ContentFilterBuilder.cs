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
        /// <summary>
        /// Creates a new empty <see cref="MigrationHookBuilder"/> object.
        /// </summary>
        public ContentFilterBuilder()
        { }

        #region - Private Helper Methods -

        private IContentFilterBuilder Add(Type filterType, Func<IServiceProvider, object> initializer)
        {
            AddFactoriesByType(filterType, initializer);
            return this;
        }

        #endregion

        #region - IContentFilterBuilder Implementation -

        /// <inheritdoc />
        public virtual IContentFilterBuilder Clear()
        {
            ClearFactories();
            return this;
        }

        /// <inheritdoc />
        public virtual IContentFilterBuilder Add(Type genericTransformerType, IEnumerable<Type[]> contentTypes)
        {
            if (!genericTransformerType.IsGenericTypeDefinition)
                throw new ArgumentException($"Type {genericTransformerType.FullName} is not a generic type definition.");

            foreach (var contentType in contentTypes)
            {
                var constructedType = genericTransformerType.MakeGenericType(contentType);

                object transformerFactory(IServiceProvider services)
                {
                    return services.GetRequiredService(constructedType);
                }

                Add(constructedType, transformerFactory);
            }

            return this;
        }

        /// <inheritdoc />
        public virtual IContentFilterBuilder Add<TContent>(IContentFilter<TContent> filter)
            where TContent : IContentReference
            => Add(typeof(IContentFilter<TContent>), s => filter);

        /// <inheritdoc />
        public virtual IContentFilterBuilder Add<TFilter, TContent>(Func<IServiceProvider, TFilter>? filterFactory = null)
            where TFilter : IContentFilter<TContent>
            where TContent : IContentReference
        {
            filterFactory ??= services => services.GetRequiredService<TFilter>();
            return Add(typeof(IContentFilter<TContent>), s => filterFactory(s));
        }

        /// <inheritdoc />
        public virtual IContentFilterBuilder Add<TContent>(Func<IEnumerable<ContentMigrationItem<TContent>>, CancellationToken, Task<IEnumerable<ContentMigrationItem<TContent>>?>> callback)
            where TContent : IContentReference
            => Add(typeof(IContentFilter<TContent>),
                   s => new CallbackHookWrapper<IContentFilter<TContent>, IEnumerable<ContentMigrationItem<TContent>>>(callback));

        /// <inheritdoc />
        public IContentFilterBuilder Add<TContent>(Func<IEnumerable<ContentMigrationItem<TContent>>, IEnumerable<ContentMigrationItem<TContent>>?> callback)
            where TContent : IContentReference
            => Add<TContent>(
                (ctx, cancel) => Task.FromResult(
                    callback(ctx)));

        #endregion
    }
}
