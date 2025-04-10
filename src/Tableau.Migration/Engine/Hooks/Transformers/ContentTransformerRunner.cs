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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Transformers
{
    internal class ContentTransformerRunner : MigrationHookRunnerBase, IContentTransformerRunner
    {
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILogger<ContentTransformerRunner> _logger;

        public ContentTransformerRunner(
            IMigrationPlan plan,
            IServiceProvider services,
            ISharedResourcesLocalizer localizer,
            ILogger<ContentTransformerRunner> logger)
                : base(plan, services)
        {
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<TPublish> ExecuteAsync<TPublish>(TPublish itemToTransform, CancellationToken cancel)
            => await ExecuteAsync<IContentTransformer<TPublish>, TPublish>(itemToTransform, LogTransformationAction, cancel).ConfigureAwait(false);

        protected sealed override ImmutableArray<IMigrationHookFactory> GetFactoryCollection<THook, TContext>()
            => Plan.Transformers.GetHooks<THook>();

        protected void LogTransformationAction<TPublish>(string hookName, TPublish _, TPublish outItemTransformed)
        {
            if (outItemTransformed is null)
                return;

            IContentReference? item = outItemTransformed as IContentReference;

            if (item is not null)
            {
                _logger.LogDebug(
                    _localizer[SharedResourceKeys.ContentTransformerBaseDebugMessage],
                    hookName,
                    item.ToStringForLog());
            }

        }
    }
}
