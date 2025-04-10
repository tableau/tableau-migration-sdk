//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    internal class ContentFilterRunner : MigrationHookRunnerBase, IContentFilterRunner
    {
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILogger<ContentFilterRunner> _logger;

        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        /// <param name="plan">Migration plan used to run the filters.</param>
        /// <param name="services">Service provider context to resolve the filters used by the runner.</param>
        /// <param name="localizer">String localizer.</param>
        /// <param name="logger">Default logger.</param>
        public ContentFilterRunner(
            IMigrationPlan plan,
            IServiceProvider services,
            ISharedResourcesLocalizer localizer,
            ILogger<ContentFilterRunner> logger) : base(plan, services)
        {
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IEnumerable<ContentMigrationItem<TContent>>> ExecuteAsync<TContent>(IEnumerable<ContentMigrationItem<TContent>> context, CancellationToken cancel)
            where TContent : IContentReference
            => await ExecuteAsync<IContentFilter<TContent>, IEnumerable<ContentMigrationItem<TContent>>>(context, AfterHookAction, cancel).ConfigureAwait(false);

        protected sealed override ImmutableArray<IMigrationHookFactory> GetFactoryCollection<THook, TContext>()
            => Plan.Filters.GetHooks<THook>();

        protected void AfterHookAction<TContent>(string hookName, IEnumerable<ContentMigrationItem<TContent>> inContext, IEnumerable<ContentMigrationItem<TContent>> outContext)
            where TContent : IContentReference
        {
            var filteredItems = inContext.Except(outContext).ToList();
            foreach (var filteredItem in filteredItems)
            {
                filteredItem.ManifestEntry.SetSkipped(hookName);

                _logger.LogDebug(
                    _localizer[SharedResourceKeys.ContentFilterBaseDebugMessage],
                    hookName,
                    filteredItem.SourceItem.ToStringForLog());
            }
        }
    }
}
