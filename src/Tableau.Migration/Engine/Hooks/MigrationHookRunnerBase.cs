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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Base implementation for <see cref="IMigrationHookRunner"/>
    /// </summary>
    internal abstract class MigrationHookRunnerBase
    {
        protected readonly IServiceProvider Services;
        protected readonly IMigrationPlan Plan;

        /// <summary>
        /// Default constructor for this base class.
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="services"></param>
        protected MigrationHookRunnerBase(IMigrationPlan plan, IServiceProvider services)
        {
            Plan = plan;
            Services = services;
        }

        /// <inheritdoc/>
        public async Task<TContext> ExecuteAsync<THook, TContext>(TContext context, CancellationToken cancel) where THook : IMigrationHook<TContext>
        {
            var currentContext = context;

            var hookFactories = GetFactoryCollection<THook, TContext>();
            foreach (var hookFactory in hookFactories)
            {
                var hook = hookFactory.Create<IMigrationHook<TContext>>(Services);

                var inputContext = currentContext;
                currentContext = (await hook.ExecuteAsync(inputContext, cancel).ConfigureAwait(false)) ?? inputContext;
            }

            return currentContext;
        }

        /// <summary>
        /// Abstract method to get the factory collection from the appropriate plan builder property.
        /// </summary>
        /// <typeparam name="THook">The hook type.</typeparam>
        /// <typeparam name="TContext">The hook context type.</typeparam>
        /// <returns></returns>
        protected abstract ImmutableArray<IMigrationHookFactory> GetFactoryCollection<THook, TContext>() where THook : IMigrationHook<TContext>;
    }
}