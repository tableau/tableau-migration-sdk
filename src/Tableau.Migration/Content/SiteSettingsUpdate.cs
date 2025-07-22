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
using System.Linq;

namespace Tableau.Migration.Content
{
    internal sealed class SiteSettingsUpdate : ISiteSettingsUpdate
    {
        /// <summary>
        /// Creates a new <see cref="SiteSettingsUpdate"/> object.
        /// </summary>
        /// <param name="siteId">The ID of the site to update.</param>
        public SiteSettingsUpdate(Guid siteId)
        {
            SiteId = siteId;
        }

        /// <inheritdoc />
        public Guid SiteId { get; }

        /// <inheritdoc />
        public string? ExtractEncryptionMode { get; set; }

        /// <inheritdoc />
        public bool? DisableSubscriptions { get; set; }

        /// <inheritdoc />
        public bool? GroupSetsEnabled { get; set; }

        /// <inheritdoc />
        public bool NeedsUpdate()
        {
            object?[] values = [ExtractEncryptionMode, DisableSubscriptions, GroupSetsEnabled];
            return values.ExceptNulls().Any();
        }
    }
}
