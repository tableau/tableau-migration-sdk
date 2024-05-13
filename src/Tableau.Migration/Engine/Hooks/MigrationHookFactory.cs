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

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Default <see cref="IMigrationHookFactory"/> implementation that uses an initializer function.
    /// </summary>
    /// <param name="Factory">The initializer function.</param>
    public record MigrationHookFactory(Func<IServiceProvider, object> Factory) : IMigrationHookFactory
    {
        /// <inheritdoc />
        public THook Create<THook>(IServiceProvider services)
        {
            var result = Factory(services);
            if (!(result is THook hook))
            {
                throw new InvalidCastException($"Cannot create hook type {typeof(THook)} from object of type {result.GetType()}.");
            }

            return hook;
        }
    }
}
