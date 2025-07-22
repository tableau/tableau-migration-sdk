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
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Mappings.Default
{
    /// <summary>
    /// Mapping that automatically maps favorites based on their respective
    /// user and content references.
    /// </summary>
    public sealed class FavoriteMapping : ContentMappingBase<IFavorite>
    {
        private readonly IMigrationManifestEditor _manifest;

        /// <summary>
        /// Creates a new <see cref="FavoriteMapping"/> object.
        /// </summary>
        /// <param name="manifest">The current migration manifest.</param>
        /// <param name="localizer">The shared resource localizer.</param>
        /// <param name="logger">The logger.</param>
        public FavoriteMapping(IMigrationManifestEditor manifest,
            ISharedResourcesLocalizer localizer, ILogger<IContentMapping<IFavorite>> logger)
            : base(localizer, logger)
        {
            _manifest = manifest;
        }

        /// <inheritdoc />
        public override Task<ContentMappingContext<IFavorite>?> MapAsync(ContentMappingContext<IFavorite> ctx, CancellationToken cancel)
        {
            var favorite = ctx.ContentItem;

            var userLocation = favorite.User.Location;
            var userManifestEntries = _manifest.Entries.GetOrCreatePartition<IUser>();
            if (userManifestEntries.BySourceLocation.TryGetValue(favorite.User.Location, out var userManifestEntry))
            {
                if(userManifestEntry.Destination is not null)
                {
                    userLocation = userManifestEntry.MappedLocation;
                }
            }

            var contentLocation = favorite.Content.Location;
            var contentPartitionType = favorite.ContentType.ToMigrationContentType();
            var contentManifestEntries = _manifest.Entries.GetOrCreatePartition(contentPartitionType);
            if (contentManifestEntries.BySourceLocation.TryGetValue(favorite.Content.Location, out var contentItemManifestEntry))
            {
                if (contentItemManifestEntry.Destination is not null)
                {
                    contentLocation = contentItemManifestEntry.MappedLocation;
                }
            }

            var mappedLocation = Favorite.BuildLocation(userLocation, favorite.ContentType, contentLocation);
            return ctx.MapTo(mappedLocation).ToTask();
        }
    }
}
