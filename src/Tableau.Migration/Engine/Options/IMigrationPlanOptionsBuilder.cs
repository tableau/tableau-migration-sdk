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

namespace Tableau.Migration.Engine.Options
{
    /// <summary>
    /// Interface for an object that can build a set of per-plan options.
    /// </summary>
    public interface IMigrationPlanOptionsBuilder
    {
        /// <summary>
        /// Sets the configuration for a given options type.
        /// </summary>
        /// <typeparam name="TOptions">The options type.</typeparam>
        /// <param name="opts">The options.</param>
        /// <returns>The same options builder, for fluent API usage.</returns>
        IMigrationPlanOptionsBuilder Configure<TOptions>(TOptions opts);

        /// <summary>
        /// Sets the configuration for a given options type.
        /// </summary>
        /// <typeparam name="TOptions">The options type.</typeparam>
        /// <param name="factory">A factory function to create the options type.</param>
        /// <returns>The same options builder, for fluent API usage.</returns>
        IMigrationPlanOptionsBuilder Configure<TOptions>(Func<IServiceProvider, TOptions> factory);

        /// <summary>
        /// Builds the options collection.
        /// </summary>
        /// <returns>The options collection.</returns>
        IMigrationPlanOptionsCollection Build();
    }
}
