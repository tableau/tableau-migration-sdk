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

namespace Tableau.Migration.Engine.Hooks.Mappings
{
    internal class ContentMappingRunner
        : MigrationHookRunnerBase, IContentMappingRunner
    {
        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        /// <param name="plan">Migration plan used to run the mappings.</param>
        /// <param name="services">Service provider context to resolve the mappings used by the runner.</param>
        public ContentMappingRunner(IMigrationPlan plan, IServiceProvider services)
            : base(plan, services)
        { }

        /// <inheritdoc />
        public async Task<ContentMappingContext<TContent>> ExecuteAsync<TContent>(ContentMappingContext<TContent> location, CancellationToken cancel)
            where TContent : IContentReference
            => await ExecuteAsync<IContentMapping<TContent>, ContentMappingContext<TContent>>(location, cancel).ConfigureAwait(false);

        protected sealed override ImmutableArray<IMigrationHookFactory> GetFactoryCollection<THook, TContext>()
            => Plan.Mappings.GetHooks<THook>();
    }
}
