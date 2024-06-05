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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.TestApplication.Config;

namespace Tableau.Migration.TestApplication.Hooks
{
    class SkipByParentLocationFilter<TContent> : IContentFilter<TContent>
        where TContent : IContentReference
    {
        private readonly ContentLocation _skippedParentProject;
        private readonly ISourceContentReferenceFinder<IProject> _sourceProjectContentReferenceFinder;
        private readonly ILogger<SkipByParentLocationFilter<TContent>> _logger;

        public SkipByParentLocationFilter(
            ILogger<SkipByParentLocationFilter<TContent>> logger,
            ISourceContentReferenceFinderFactory sourceContentReferenceFinderFactory,
            IOptions<TestApplicationOptions> options)
        {
            _sourceProjectContentReferenceFinder = sourceContentReferenceFinderFactory.ForSourceContentType<IProject>();
            _logger = logger;
            _skippedParentProject = ContentLocation.FromPath(options.Value.SkippedProject);
        }

        public async Task<IEnumerable<ContentMigrationItem<TContent>>?> ExecuteAsync(
            IEnumerable<ContentMigrationItem<TContent>> ctx, 
            CancellationToken cancel)
        {
            if (_skippedParentProject.IsEmpty)
            {
                return ctx;
            }

            var filteredList = new List<ContentMigrationItem<TContent>>();

            foreach (var item in ctx)
            {
                var parent = item.SourceItem.Location.Parent();

                if (_skippedParentProject != parent)
                {
                    filteredList.Add(item);
                    continue;
                }

                var contentReference = await _sourceProjectContentReferenceFinder
                    .FindBySourceLocationAsync(parent, cancel)
                    .ConfigureAwait(false);

                _logger.LogInformation($"Skipping {typeof(TContent).Name} that belongs to \"{nameof(TestApplicationOptions.SkippedProject)}\"  (Project Id: {contentReference?.Id})");
            }

            return filteredList;
        }
    }
}
