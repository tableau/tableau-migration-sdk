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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.ContentClients;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters.Default
{
    /// <summary>
    /// Migration filter that skips favorites for users that were also skipped or for content that hasn't been migrated,
    /// since favorites 'belong' to the user and reference content that must exist.
    /// </summary>
    public sealed class FavoriteFilter : AsyncContentFilterBase<IFavorite>
    {
        private readonly IMigrationManifestEditor _manifest;
        private readonly IViewsContentClient _sourceViewClient;

        /// <summary>
        /// Creates a new <see cref="FavoriteFilter"/> object.
        /// </summary>
        /// <param name="manifest">The current migration manifest.</param>
        /// <param name="sourceEndpoint">The source endpoint.</param>
        /// <param name="localizer">The shared resource localizer.</param>
        /// <param name="logger">The logger.</param>
        public FavoriteFilter(IMigrationManifestEditor manifest,
            ISourceEndpoint sourceEndpoint,
            ISharedResourcesLocalizer localizer, ILogger<IContentFilter<IFavorite>> logger)
            : base(localizer, logger)
        {
            _manifest = manifest;
            _sourceViewClient = sourceEndpoint.GetViewsContentClient();
        }

        /// <inheritdoc />
        public override async Task<bool> ShouldMigrateAsync(ContentMigrationItem<IFavorite> item, CancellationToken cancel)
        {
            var favorite = item.SourceItem;

            // Skip flow favorites until flows are a supported content type to migrate.
            if (!favorite.ContentType.IsMigrationSupported())
            {
                return false;
            }

            // Skip favorites for users that did not migrate.
            var userManifestEntries = _manifest.Entries.GetOrCreatePartition<IUser>();
            if (userManifestEntries.BySourceLocation.TryGetValue(favorite.User.Location, out var userManifestEntry))
            {
                if (userManifestEntry.Status is MigrationManifestEntryStatus.Skipped)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            // Skip favorites for content items that did not migrate.
            if (favorite.ContentType is FavoriteContentType.View)
            {
                var sourceViewResult = await _sourceViewClient.GetByIdAsync(favorite.Content.Id, cancel).ConfigureAwait(false);
                if (!sourceViewResult.Success)
                {
                    return false;
                }

                var sourceView = sourceViewResult.Value;
                var workbookManifestEntries = _manifest.Entries.GetOrCreatePartition<IWorkbook>();
                return workbookManifestEntries.BySourceLocation.TryGetValue(sourceView.ParentWorkbook.Location, out var parentWorkbookManifestEntry)
                    ? parentWorkbookManifestEntry.Status is not MigrationManifestEntryStatus.Skipped
                    : false;
            }

            var contentType = favorite.ContentType.ToMigrationContentType();
            var contentManifestEntries = _manifest.Entries.GetOrCreatePartition(contentType);

            return contentManifestEntries.BySourceLocation.TryGetValue(favorite.Content.Location, out var contentItemManifestEntry)
                ? contentItemManifestEntry.Status is not MigrationManifestEntryStatus.Skipped
                : false;

        }
    }
}