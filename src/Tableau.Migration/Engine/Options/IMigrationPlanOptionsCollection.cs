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
    /// Interface for an object that contains plan-specific options objects.
    /// </summary>
    public interface IMigrationPlanOptionsCollection
    {
        /// <summary>
        /// Gets the options for the given type, 
        /// or null if no options for the given type have been registered.
        /// </summary>
        /// <typeparam name="TOptions">The options type.</typeparam>
        /// <param name="services">A service provider.</param>
        /// <returns>The options for the given type, or null.</returns>
        TOptions? Get<TOptions>(IServiceProvider services);
    }
}
