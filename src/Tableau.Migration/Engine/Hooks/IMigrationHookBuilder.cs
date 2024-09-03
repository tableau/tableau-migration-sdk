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

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Interface for an object that contains the hooks to execute at various points during the migration, determined by hook type.
    /// </summary>
    public interface IMigrationHookBuilder
    {
        /// <summary>
        /// Removes all currently registered hooks.
        /// </summary>
        /// <returns>The same hook builder object for fluent API calls.</returns>
        IMigrationHookBuilder Clear();

        /// <summary>
        /// Adds a factory to resolve the hook type.
        /// </summary>
        /// <param name="genericHookType">The generic type definition for the hook to execute.</param>
        /// <param name="contentTypes">The content types used to construct the hook types.</param>
        /// <returns>The same hook builder object for fluent API calls.</returns>
        IMigrationHookBuilder Add(Type genericHookType, IEnumerable<Type[]> contentTypes);

        /// <summary>
        /// Adds an object to be resolved when you build a hook for the content type.
        /// </summary>
        /// <typeparam name="THook">The generic type to detect all hook types from.</typeparam>
        /// <param name="hook">The hook to execute.</param>
        /// <returns>The same hook builder object for fluent API calls.</returns>
        IMigrationHookBuilder Add<THook>(THook hook)
            where THook : notnull;

        /// <summary>
        /// Adds a factory to resolve the hook type.
        /// </summary>
        /// <typeparam name="THook">The generic type to detect all hook types from.</typeparam>
        /// <param name="hookFactory">An initializer function to create the object from, potentially from the migration-scoped dependency injection container.</param>
        /// <returns>The same hook builder object for fluent API calls.</returns>
        IMigrationHookBuilder Add<THook>(Func<IServiceProvider, THook>? hookFactory = null)
            where THook : notnull;

        /// <summary>
        /// Adds a callback to be executed on the hook for the content type.
        /// </summary>
        /// <typeparam name="THook">The generic type to detect all hook types from.</typeparam>
        /// <typeparam name="TContext">The hook's context type.</typeparam>
        /// <param name="callback">A callback to call for the hook.</param>
        /// <returns>The same hook builder object for fluent API calls.</returns>
        IMigrationHookBuilder Add<THook, TContext>(Func<TContext, CancellationToken, Task<TContext?>> callback)
            where THook : IMigrationHook<TContext>;

        /// <summary>
        /// Adds a callback to be executed on the hook for the content type.
        /// </summary>
        /// <typeparam name="THook">The generic type to detect all hook types from.</typeparam>
        /// <typeparam name="TContext">The hook's context type.</typeparam>
        /// <param name="callback">A synchronous callback to call for the hook.</param>
        /// <returns>The same hook builder object for fluent API calls.</returns>
        IMigrationHookBuilder Add<THook, TContext>(Func<TContext, TContext?> callback)
            where THook : IMigrationHook<TContext>;

        /// <summary>
        /// Builds an immutable collection from the currently added hooks.
        /// </summary>
        /// <returns>The created collection.</returns>
        IMigrationHookFactoryCollection Build();
    }
}
