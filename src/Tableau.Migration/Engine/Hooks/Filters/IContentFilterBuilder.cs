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

namespace Tableau.Migration.Engine.Hooks.Filters
{
    /// <summary>
    /// Interface with methods to build <see cref="IContentFilter{TContent}"/>.
    /// </summary>
    public interface IContentFilterBuilder : IContentTypeHookBuilder
    {
        /// <summary>
        /// Removes all currently registered filters.
        /// </summary>
        /// <returns>The same filter builder object for fluent API calls.</returns>
        IContentFilterBuilder Clear();

        /// <summary>
        /// Adds a factory to resolve the filter type.
        /// </summary>
        /// <param name="genericFilterType">The generic type definition for the filter to execute.</param>
        /// <param name="contentTypes">The content types used to construct the filter types.</param>
        /// <returns>The same filter builder object for fluent API calls.</returns>
        IContentFilterBuilder Add(Type genericFilterType, IEnumerable<Type[]> contentTypes);

        /// <summary>
        /// Adds an object to be resolved when you build a filter for the content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="filter">The filter to execute.</param>
        /// <returns>The same filter builder object for fluent API calls.</returns>
        IContentFilterBuilder Add<TContent>(IContentFilter<TContent> filter)
           where TContent : IContentReference;

        /// <summary>
        /// Adds a factory to resolve the filter type.
        /// </summary>
        /// <typeparam name="TFilter">The filter type.</typeparam>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="filterFactory">An initializer function to create the object from, potentially from the migration-scoped dependency injection container.</param>
        /// <returns>The same filter builder object for fluent API calls.</returns>
        IContentFilterBuilder Add<TFilter, TContent>(Func<IServiceProvider, TFilter>? filterFactory = null)
            where TFilter : IContentFilter<TContent>
            where TContent : IContentReference;

        /// <summary>
        /// Adds a callback to be executed one the filter for the content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="callback">A callback to call for the filter.</param>
        /// <returns>The same filter builder object for fluent API calls.</returns>
        IContentFilterBuilder Add<TContent>(Func<IEnumerable<ContentMigrationItem<TContent>>, CancellationToken, Task<IEnumerable<ContentMigrationItem<TContent>>?>> callback)
            where TContent : IContentReference;

        /// <summary>
        /// Adds a callback to be executed on the filter for the content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="callback">A synchronously callback to call for the filter.</param>
        /// <returns>The same filter builder object for fluent API calls.</returns>
        IContentFilterBuilder Add<TContent>(Func<IEnumerable<ContentMigrationItem<TContent>>, IEnumerable<ContentMigrationItem<TContent>>?> callback)
            where TContent : IContentReference;

        /// <summary>
        /// Builds an immutable collection from the currently added filters.
        /// </summary>
        /// <returns>The created collection.</returns>
        IMigrationHookFactoryCollection Build();
    }
}