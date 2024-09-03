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

namespace Tableau.Migration.Engine.Hooks.Mappings
{
    /// <summary>
    /// Interface with methods to build <see cref="IContentMapping{TContent}"/>.
    /// </summary>
    public interface IContentMappingBuilder : IContentTypeHookBuilder
    {
        /// <summary>
        /// Removes all currently registered mappings.
        /// </summary>
        /// <returns>The same mapping builder object for fluent API calls.</returns>
        IContentMappingBuilder Clear();

        /// <summary>
        /// Adds a factory to resolve the mapping type.
        /// </summary>
        /// <param name="genericMappingType">The generic type definition for the mapping to execute.</param>
        /// <param name="contentTypes">The content types used to construct the mapping types.</param>
        /// <returns>The same mapping builder object for fluent API calls.</returns>
        IContentMappingBuilder Add(Type genericMappingType, IEnumerable<Type[]> contentTypes);

        /// <summary>
        /// Adds an object to be resolved when you build a mapping for the content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="mapping">The mapping to execute.</param>
        /// <returns>The same mapping builder object for fluent API calls.</returns>
        IContentMappingBuilder Add<TContent>(IContentMapping<TContent> mapping)
            where TContent : IContentReference;

        /// <summary>
        /// Adds a factory to resolve the mapping type.
        /// </summary>
        /// <typeparam name="TMapping">The mapping type.</typeparam>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="mappingFactory">An initializer function to create the object from, potentially from the migration-scoped dependency injection container.</param>
        /// <returns>The same mapping builder object for fluent API calls.</returns>
        IContentMappingBuilder Add<TMapping, TContent>(Func<IServiceProvider, TMapping>? mappingFactory = null)
            where TMapping : IContentMapping<TContent>
            where TContent : IContentReference;

        /// <summary>
        /// Adds a callback to be executed on the mapping for the content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="callback">A callback to call for the mapping.</param>
        /// <returns>The same mapping builder object for fluent API calls.</returns>
        IContentMappingBuilder Add<TContent>(Func<ContentMappingContext<TContent>, CancellationToken, Task<ContentMappingContext<TContent>?>> callback)
            where TContent : IContentReference;

        /// <summary>
        /// Adds a callback to be executed on the mapping for the content type.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="callback">A synchronously callback to call for the mapping.</param>
        /// <returns>The same mapping builder object for fluent API calls.</returns>
        IContentMappingBuilder Add<TContent>(Func<ContentMappingContext<TContent>, ContentMappingContext<TContent>?> callback)
            where TContent : IContentReference;

        /// <summary>
        /// Builds an immutable collection from the currently added mappings.
        /// </summary>
        /// <returns>The created collection.</returns>
        IMigrationHookFactoryCollection Build();
    }
}