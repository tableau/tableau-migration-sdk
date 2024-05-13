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

namespace Tableau.Migration.Engine.Options
{
    /// <summary>
    /// Default <see cref="IMigrationPlanOptionsCollection"/> implementation.
    /// </summary>
    public class MigrationPlanOptionsCollection : IMigrationPlanOptionsCollection
    {
        /// <summary>
        /// Gets an empty <see cref="MigrationPlanOptionsCollection"/>.
        /// </summary>
        public static readonly MigrationPlanOptionsCollection Empty = new(ImmutableDictionary<Type, Func<IServiceProvider, object?>>.Empty);

        private readonly ImmutableDictionary<Type, Func<IServiceProvider, object?>> _optionFactories;

        /// <summary>
        /// Creates a new <see cref="MigrationPlanOptionsCollection"/> object.
        /// </summary>
        /// <param name="optionFactories">The options factories by option type.</param>
        public MigrationPlanOptionsCollection(ImmutableDictionary<Type, Func<IServiceProvider, object?>> optionFactories)
        {
            _optionFactories = optionFactories;
        }

        /// <inheritdoc />
        public TOptions? Get<TOptions>(IServiceProvider services)
        {
            if (_optionFactories.TryGetValue(typeof(TOptions), out var optionsFactory))
            {
                return (TOptions?)optionsFactory(services);
            }

            return default;
        }
    }
}
