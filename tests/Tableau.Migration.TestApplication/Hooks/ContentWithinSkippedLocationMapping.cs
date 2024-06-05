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
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.TestApplication.Config;

namespace Tableau.Migration.TestApplication.Hooks
{
    class ContentWithinSkippedLocationMapping<TContent> : IContentMapping<TContent>
        where TContent : IContentReference
    {
        private readonly ContentLocation _skippedParentProject;
        private readonly ContentLocation _missingParentLocation;
        private readonly IDestinationContentReferenceFinder<IProject> _destinationProjectContentReferenceFinder;
        private readonly ILogger<ContentWithinSkippedLocationMapping<TContent>> _logger;

        public ContentWithinSkippedLocationMapping(
            ILogger<ContentWithinSkippedLocationMapping<TContent>> logger,
            IDestinationContentReferenceFinderFactory destinationContentReferenceFinderFactory,
            IOptions<TestApplicationOptions> options)
        {
            _destinationProjectContentReferenceFinder = destinationContentReferenceFinderFactory.ForDestinationContentType<IProject>();
            _logger = logger;
            _skippedParentProject = ContentLocation.FromPath(options.Value.SkippedProject);
            _missingParentLocation = ContentLocation.FromPath(options.Value.SkippedMissingParentDestination);
        }

        public async Task<ContentMappingContext<TContent>?> ExecuteAsync(
            ContentMappingContext<TContent> ctx, 
            CancellationToken cancel)
        {
            if (_skippedParentProject.IsEmpty)
            {
                return ctx;
            }

            var pathReplaced = ctx.ContentItem.Location.Path.Replace(_skippedParentProject.Path, "");

            if (!ctx.ContentItem.Location.Path.StartsWith(_skippedParentProject.Path) ||
                string.IsNullOrWhiteSpace(pathReplaced) ||
                pathReplaced.Split(Constants.PathSeparator, StringSplitOptions.RemoveEmptyEntries).Length <= 1)
            {
                return ctx;
            }

            var contentReference = await _destinationProjectContentReferenceFinder
                .FindByMappedLocationAsync(_missingParentLocation, cancel)
                .ConfigureAwait(false);

            if (contentReference is null)
            {
                _logger.LogError($"Cannot map the {typeof(TContent).Name} \"{ctx.ContentItem.Name}\" that belongs to \"{nameof(TestApplicationOptions.SkippedProject)}\" to the Project \"{_missingParentLocation}\". You must create the destination location first.");

                return ctx;
            }
            var pathList = new List<string>(_missingParentLocation.PathSegments);

            for (int i = _skippedParentProject.PathSegments.Length + 1; i < ctx.ContentItem.Location.PathSegments.Length; i++)
            {
                pathList.Add(ctx.ContentItem.Location.PathSegments[i]);
            }

            _logger.LogInformation($"Mapping the {typeof(TContent).Name} \"{ctx.ContentItem.Name}\" that belongs to \"{nameof(TestApplicationOptions.SkippedProject)}\" to the Project \"{_missingParentLocation}\" ( Id: {contentReference.Id})");

            ctx = ctx.MapTo(new ContentLocation(pathList));

            return ctx;
        }
    }
}
