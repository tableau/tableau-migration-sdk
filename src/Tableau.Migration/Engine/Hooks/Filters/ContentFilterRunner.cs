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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    internal class ContentFilterRunner : MigrationHookRunnerBase, IContentFilterRunner
    {
        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        /// <param name="plan">Migration plan used to run the filters.</param>
        /// <param name="services">Service provider context to resolve the filters used by the runner.</param>
        public ContentFilterRunner(IMigrationPlan plan, IServiceProvider services) : base(plan, services)
        { }

        public async Task<IEnumerable<ContentMigrationItem<TContent>>> ExecuteAsync<TContent>(IEnumerable<ContentMigrationItem<TContent>> context, CancellationToken cancel)
            where TContent : IContentReference
            => await ExecuteAsync<IContentFilter<TContent>, IEnumerable<ContentMigrationItem<TContent>>>(context, cancel).ConfigureAwait(false);

        protected sealed override ImmutableArray<IMigrationHookFactory> GetFactoryCollection<THook, TContext>()
            => Plan.Filters.GetHooks<THook>();
    }
}
