//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.PostPublish.Default
{
    /// <summary>
    /// Workbook post-publish hook that cleans up views in destination that don't exist in source.
    /// </summary>
    public class CleanupViewsPostPublishHook : ContentItemPostPublishHookBase<IPublishableWorkbook, IWorkbookDetails>
    {
        private readonly ISourceApiEndpoint? _sourceApiEndpoint;
        private readonly IDestinationApiEndpoint? _destinationApiEndpoint;
        private readonly ILogger<CleanupViewsPostPublishHook> _logger;
        private readonly ISharedResourcesLocalizer _localizer;

        /// <summary>
        /// Gets whether the hook is enabled.
        /// </summary>
        [MemberNotNullWhen(true, nameof(_sourceApiEndpoint), nameof(_destinationApiEndpoint))]
        internal bool IsEnabled { get; }

        /// <summary>
        /// Creates a new <see cref="CleanupViewsPostPublishHook"/> object.
        /// </summary>
        /// <param name="migration">The current migration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="localizer">The shared resources localizer.</param>
        public CleanupViewsPostPublishHook(
            IMigration migration,
            ILogger<CleanupViewsPostPublishHook> logger,
            ISharedResourcesLocalizer localizer)
        {
            _logger = logger;
            _localizer = localizer;
            IsEnabled = migration.TryGetSourceApiEndpoint(out _sourceApiEndpoint) &&
                       migration.TryGetDestinationApiEndpoint(out _destinationApiEndpoint);
        }

        /// <inheritdoc/>
        public override async Task<ContentItemPostPublishContext<IPublishableWorkbook, IWorkbookDetails>?> ExecuteAsync(
            ContentItemPostPublishContext<IPublishableWorkbook, IWorkbookDetails> ctx,
            CancellationToken cancel)
        {
            if (!IsEnabled)
            {
                return ctx;
            }

            try
            {

                var sourceViewsResult = await _sourceApiEndpoint.SiteApi.Workbooks
                    .GetWorkbookViewsAsync(ctx.PublishedItem.Id, cancel)
                    .ConfigureAwait(false);

                if (!sourceViewsResult.Success)
                {
                    _logger.LogWarning(_localizer[SharedResourceKeys.FailedToGetSourceViewsForWorkbookError],
                        ctx.PublishedItem.Id, string.Join(", ", sourceViewsResult.Errors.Select(e => e.Message)));
                    ctx.ManifestEntry.SetFailed(sourceViewsResult.Errors);
                    return ctx;
                }

                // Get destination views
                var destinationViewsResult = await _destinationApiEndpoint.SiteApi.Workbooks
                    .GetWorkbookViewsAsync(ctx.DestinationItem.Id, cancel)
                    .ConfigureAwait(false);

                if (!destinationViewsResult.Success)
                {
                    _logger.LogWarning(_localizer[SharedResourceKeys.FailedToGetDestinationViewsForWorkbookError],
                        ctx.DestinationItem.Id, string.Join(", ", destinationViewsResult.Errors.Select(e => e.Message)));
                    ctx.ManifestEntry.SetFailed(destinationViewsResult.Errors);
                    return ctx;
                }

                // Compare views and find ones to delete
                var sourceViewNames = new HashSet<string>(
                    sourceViewsResult.Value.Select(v => v.Name),
                    StringComparer.OrdinalIgnoreCase);

                var viewsToDelete = destinationViewsResult.Value
                    .Where(v => !sourceViewNames.Contains(v.Name))
                    .ToList();

                if (viewsToDelete.Count == 0)
                {
                    return ctx;
                }

                // Delete views that don't exist in source
                var deleteResults = new List<Exception>();
                foreach (var viewToDelete in viewsToDelete)
                {
                    var deleteResult = await _destinationApiEndpoint.SiteApi.Views
                        .DeleteAsync(viewToDelete.Id, cancel)
                        .ConfigureAwait(false);

                    if (!deleteResult.Success)
                    {
                        _logger.LogError(_localizer[SharedResourceKeys.FailedToDeleteViewFromWorkbookError],
                            viewToDelete.Id, viewToDelete.Name, ctx.DestinationItem.Id,
                            string.Join(", ", deleteResult.Errors.Select(e => e.Message)));

                        deleteResults.AddRange(deleteResult.Errors);
                    }
                }

                // If there were any delete failures, mark the manifest entry as failed
                if (deleteResults.Count > 0)
                {
                    ctx.ManifestEntry.SetFailed(deleteResults);
                }

                return ctx;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _localizer[SharedResourceKeys.UnexpectedErrorDuringViewCleanupError],
                    ctx.DestinationItem.Id, ctx.DestinationItem.Name);
                ctx.ManifestEntry.SetFailed(ex);
                return ctx;
            }
        }
    }
}