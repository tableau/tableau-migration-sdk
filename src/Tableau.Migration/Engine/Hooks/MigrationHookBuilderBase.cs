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
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Base implementation for <see cref="IMigrationHookBuilder"/>
    /// </summary>
    public abstract class MigrationHookBuilderBase
    {
        private readonly Dictionary<Type, ImmutableArray<IMigrationHookFactory>.Builder> _factoriesByType;

        /// <summary>
        /// Creates a new empty <see cref="MigrationHookBuilderBase"/> object.
        /// </summary>
        protected MigrationHookBuilderBase() => _factoriesByType = new();

        #region - Private Hook Type Detection Methods -

        private static ImmutableArray<Type> GetHookTypes(Type t)
        {
            var results = t.GetInterfaces()
                .Where(IsHookLikeInterface);

            if (IsHookLikeInterface(t))
            {
                results = results.Append(t);
            }

            return results.Distinct()
                .ToImmutableArray();
        }


        private static bool IsHookInterface(Type t)
            => t.IsInterface && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IMigrationHook<>);

        private static bool IsHookLikeInterface(Type t)
        {
            if (!t.IsInterface)
            {
                return false;
            }

            //Don't want an exact match to IMigrationHook<T>, which could be ambiguous if context type if shared between different hooks.
            if (IsHookInterface(t))
            {
                return false;
            }

            //interface derived from IMigrationHook<T>
            return t.GetInterfaces().Any(IsHookInterface);
        }

        #endregion

        #region - Protected Methods -

        /// <summary>
        /// Clears all registered hook factories for all hook types.
        /// </summary>
        protected void ClearFactories()
        {
            _factoriesByType.Clear();
        }

        /// <summary>
        /// Adds a hook factory for the given hook type(s).
        /// </summary>
        /// <param name="hookType">The type to detect hook types from.</param>
        /// <param name="initializer">The hook factory.</param>
        /// <exception cref="ArgumentException">If <paramref name="hookType"/> does not implement any hook types.</exception>
        protected void AddFactoriesByType(Type hookType, Func<IServiceProvider, object> initializer)
        {
            var hookInterfaceTypes = GetHookTypes(hookType);
            if (hookInterfaceTypes.IsNullOrEmpty())
                throw new ArgumentException($"Type {hookType} does not implement any migration hook types.");

            var factory = new MigrationHookFactory(initializer);
            void AddForHookInterface(Type hookInterfaceType)
            {
                if (!_factoriesByType.TryGetValue(hookInterfaceType, out var factoryList))
                {
                    _factoriesByType.Add(hookInterfaceType, factoryList = ImmutableArray.CreateBuilder<IMigrationHookFactory>());
                }

                factoryList.Add(factory);
            }

            foreach (var hookInterfaceType in hookInterfaceTypes)
            {
                AddForHookInterface(hookInterfaceType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterType"></param>
        /// <param name="initializer"></param>
        /// <returns></returns>
        protected MigrationHookBuilderBase Add(Type filterType, Func<IServiceProvider, object> initializer)
        {
            AddFactoriesByType(filterType, initializer);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="genericHookType"></param>
        /// <param name="contentTypes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        protected MigrationHookBuilderBase Add(Type genericHookType, IEnumerable<Type[]> contentTypes)
        {
            if (!genericHookType.IsGenericTypeDefinition)
                throw new ArgumentException($"Type {genericHookType.FullName} is not a generic type definition.");

            foreach (var contentType in contentTypes)
            {
                var constructedType = genericHookType.MakeGenericType(contentType);

                object hookFactory(IServiceProvider services)
                {
                    return services.GetRequiredService(constructedType);
                }

                Add(constructedType, hookFactory);
            }

            return this;
        }

        #endregion

        /// <inheritdoc />
        public virtual IMigrationHookFactoryCollection Build()
        {
            var immutableDict = ImmutableDictionary.CreateBuilder<Type, ImmutableArray<IMigrationHookFactory>>();
            foreach (var kvp in _factoriesByType)
            {
                immutableDict.Add(kvp.Key, kvp.Value.ToImmutable());
            }

            return new MigrationHookFactoryCollection(immutableDict.ToImmutable());
        }

        /// <summary>
        /// Gets the currently registered hook factories.
        /// </summary>
        /// <returns>The hook factories, by their hook type.</returns>
        protected IEnumerable<KeyValuePair<Type, IEnumerable<IMigrationHookFactory>>> GetFactories()
        {
            foreach (var kvp in _factoriesByType)
            {
                yield return new KeyValuePair<Type, IEnumerable<IMigrationHookFactory>>(kvp.Key, kvp.Value);
            }
        }
    }
}