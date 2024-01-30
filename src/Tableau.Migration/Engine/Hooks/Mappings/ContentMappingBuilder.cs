// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Tableau.Migration.Engine.Hooks.Mappings
{
    /// <summary>
    /// Default <see cref="IContentMappingBuilder"/> implementation.
    /// </summary>
    public class ContentMappingBuilder : ContentTypeHookBuilderBase, IContentMappingBuilder
    {
        /// <summary>
        /// Creates a new empty <see cref="ContentMappingBuilder"/> object.
        /// </summary>
        public ContentMappingBuilder()
        { }

        #region - Private Helper Methods -

        private IContentMappingBuilder Add(Type mappingType, Func<IServiceProvider, object> initializer)
        {
            AddFactoriesByType(mappingType, initializer);
            return this;
        }

        #endregion

        #region - IContentMappingBuilder Implementation -

        /// <inheritdoc />
        public virtual IContentMappingBuilder Clear()
        {
            ClearFactories();
            return this;
        }

        /// <inheritdoc />
        public virtual IContentMappingBuilder Add<TContent>(IContentMapping<TContent> mapping)
            where TContent : IContentReference
            => Add(typeof(IContentMapping<TContent>), s => mapping);

        /// <inheritdoc />
        public virtual IContentMappingBuilder Add<TMapping, TContent>(Func<IServiceProvider, TMapping>? mappingFactory = null)
            where TMapping : IContentMapping<TContent>
            where TContent : IContentReference
        {
            mappingFactory ??= services => services.GetRequiredService<TMapping>();
            return Add(typeof(IContentMapping<TContent>), s => mappingFactory(s));
        }

        /// <inheritdoc />
        public virtual IContentMappingBuilder Add<TContent>(Func<ContentMappingContext<TContent>, CancellationToken, Task<ContentMappingContext<TContent>?>> callback)
            where TContent : IContentReference
            => Add(typeof(IContentMapping<TContent>),
                   s => new CallbackHookWrapper<IContentMapping<TContent>, ContentMappingContext<TContent>>(callback));

        /// <inheritdoc />
        public IContentMappingBuilder Add<TContent>(Func<ContentMappingContext<TContent>, ContentMappingContext<TContent>?> callback)
            where TContent : IContentReference
            => Add<TContent>(
                (ctx, cancel) => Task.FromResult(
                    callback(ctx)));

        #endregion
    }
}
