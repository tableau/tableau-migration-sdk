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

namespace Tableau.Migration.Engine.Hooks.Transformers
{
    /// <summary>
    /// Default <see cref="IContentTransformerBuilder"/> implementation.
    /// </summary>
    public class ContentTransformerBuilder : ContentTypeHookBuilderBase, IContentTransformerBuilder
    {
        #region - IContentTransformerBuilder Implementation -

        /// <inheritdoc />
        public virtual IContentTransformerBuilder Clear()
        {
            ClearFactories();
            return this;
        }

        /// <inheritdoc />
        new public virtual IContentTransformerBuilder Add(Type genericTransformerType, IEnumerable<Type[]> contentTypes)
            => (IContentTransformerBuilder)base.Add(genericTransformerType, contentTypes);

        /// <inheritdoc />
        public virtual IContentTransformerBuilder Add<TPublish>(IContentTransformer<TPublish> transformer)
            => (IContentTransformerBuilder)Add(typeof(IContentTransformer<TPublish>), s => transformer);

        /// <inheritdoc />
        public virtual IContentTransformerBuilder Add<TTransformer, TPublish>(Func<IServiceProvider, TTransformer>? contentTransformerFactory = null)
            where TTransformer : IContentTransformer<TPublish>
        {
            contentTransformerFactory ??= services => services.GetRequiredService<TTransformer>();
            return (IContentTransformerBuilder)Add(typeof(IContentTransformer<TPublish>), s => contentTransformerFactory(s));
        }

        /// <inheritdoc />
        public virtual IContentTransformerBuilder Add<TPublish>(Func<TPublish, CancellationToken, Task<TPublish?>> callback)
            => (IContentTransformerBuilder)Add(typeof(IContentTransformer<TPublish>),
                   s => new CallbackHookWrapper<IContentTransformer<TPublish>, TPublish>(callback));

        /// <inheritdoc />
        public IContentTransformerBuilder Add<TPublish>(Func<TPublish, TPublish?> callback)
            => Add<TPublish>((ctx, cancel) => Task.FromResult(callback(ctx)));

        #endregion
    }
}
