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
using System.Collections.Immutable;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Interface for an object that contains <see cref="MigrationHookFactory"/>s registered for each hook type.
    /// </summary>
    public class MigrationHookFactoryCollection : IMigrationHookFactoryCollection
    {
        private readonly ImmutableDictionary<Type, ImmutableArray<IMigrationHookFactory>> _factoriesByType;

        /// <summary>
        /// Gets an empty <see cref="MigrationHookFactoryCollection"/>.
        /// </summary>
        public static readonly MigrationHookFactoryCollection Empty = new(ImmutableDictionary<Type, ImmutableArray<IMigrationHookFactory>>.Empty);

        /// <summary>
        /// Creates a new <see cref="MigrationHookFactoryCollection"/> object.
        /// </summary>
        /// <param name="factoriesByType">The hook factories registered per hook type.</param>
        public MigrationHookFactoryCollection(ImmutableDictionary<Type, ImmutableArray<IMigrationHookFactory>> factoriesByType)
        {
            _factoriesByType = factoriesByType;
        }

        /// <inheritdoc />
        public ImmutableArray<IMigrationHookFactory> GetHooks<THook>() => GetHooks(typeof(THook));

        /// <inheritdoc />
        public ImmutableArray<IMigrationHookFactory> GetHooks(Type hookType)
        {
            if (!_factoriesByType.TryGetValue(hookType, out var results))
            {
                return ImmutableArray<IMigrationHookFactory>.Empty;
            }

            return results;
        }
    }
}
