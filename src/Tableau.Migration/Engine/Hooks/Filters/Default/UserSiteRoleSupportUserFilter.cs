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

using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Hooks.Filters.Default
{
    /// <summary>
    /// Excludes users with <see cref="SiteRoles.SupportUser"/> siterole from migration.
    /// </summary>
    public class UserSiteRoleSupportUserFilter : ContentFilterBase<IUser>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="localizer">A string localizer.</param>
        /// <param name="logger">Default logger.</param>
        public UserSiteRoleSupportUserFilter(ISharedResourcesLocalizer localizer, ILogger<IContentFilter<IUser>> logger) : base(localizer, logger) { }

        /// <inheritdoc/>
        public override bool ShouldMigrate(ContentMigrationItem<IUser> item)
            => !SiteRoles.IsAMatch(item.SourceItem.SiteRole, SiteRoles.SupportUser);
    }
}
