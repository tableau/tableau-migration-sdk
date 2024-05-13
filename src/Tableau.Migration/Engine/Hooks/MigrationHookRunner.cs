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
    internal class MigrationHookRunner : MigrationHookRunnerBase, IMigrationHookRunner
    {
        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        /// <param name="plan">Migration plan used to run the hooks.</param>
        /// <param name="services">Service provider context to resolve the hooks used by the runner.</param>
        public MigrationHookRunner(IMigrationPlan plan, IServiceProvider services) : base(plan, services)
        { }

        /// <summary>
        /// Abstract method to get the factory collection from the appropriate plan builder property.
        /// </summary>
        /// <typeparam name="THook">The hook type.</typeparam>
        /// <typeparam name="TContext">The hook context type.</typeparam>
        /// <returns></returns>
        protected sealed override ImmutableArray<IMigrationHookFactory> GetFactoryCollection<THook, TContext>()
        {
            return Plan.Hooks.GetHooks<THook>();
        }
    }
}
