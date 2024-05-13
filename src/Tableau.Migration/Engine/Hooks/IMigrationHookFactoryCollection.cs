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
    public interface IMigrationHookFactoryCollection
    {
        /// <summary>
        /// Gets the <see cref="MigrationHookFactory"/>s for the given hook type.
        /// </summary>
        /// <typeparam name="THook">The hook type.</typeparam>
        /// <returns>An immutable array of the registered hook factories for the given type.</returns>
        ImmutableArray<IMigrationHookFactory> GetHooks<THook>();

        /// <summary>
        /// Gets the <see cref="MigrationHookFactory"/>s for the given hook type.
        /// </summary>
        /// <param name="hookType">The hook type.</param>
        /// <returns>An immutable array of the registered hook factories for the given type.</returns>
        ImmutableArray<IMigrationHookFactory> GetHooks(Type hookType);
    }
}
