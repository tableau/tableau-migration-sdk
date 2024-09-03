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

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Default <see cref="IMigrationHookBuilder"/> implementation.
    /// </summary>
    public class MigrationHookBuilder : MigrationHookBuilderBase, IMigrationHookBuilder
    {
        #region - IMigrationHookBuilder Implementation -

        /// <inheritdoc />
        public IMigrationHookBuilder Clear()
        {
            ClearFactories();
            return this;
        }

        /// <inheritdoc />
        new public virtual IMigrationHookBuilder Add(Type genericHookType, IEnumerable<Type[]> contentTypes)
            => (IMigrationHookBuilder)base.Add(genericHookType, contentTypes);

        /// <inheritdoc />
        public virtual IMigrationHookBuilder Add<THook>(THook hook)
            where THook : notnull
            => (IMigrationHookBuilder)Add(typeof(THook), s => hook);

        /// <inheritdoc />
        public virtual IMigrationHookBuilder Add<THook>(Func<IServiceProvider, THook>? hookFactory = null)
            where THook : notnull
        {
            hookFactory ??= services => services.GetRequiredService<THook>();
            return (IMigrationHookBuilder)Add(typeof(THook), s => hookFactory(s));
        }

        /// <inheritdoc />
        public virtual IMigrationHookBuilder Add<THook, TContext>(Func<TContext, CancellationToken, Task<TContext?>> callback)
            where THook : IMigrationHook<TContext>
            => (IMigrationHookBuilder)Add(typeof(THook), s => new CallbackHookWrapper<THook, TContext>(callback));

        /// <inheritdoc />
        public IMigrationHookBuilder Add<THook, TContext>(Func<TContext, TContext?> callback)
            where THook : IMigrationHook<TContext>
            => Add<THook, TContext>((ctx, cancel) => Task.FromResult(callback(ctx)));

        #endregion
    }
}
