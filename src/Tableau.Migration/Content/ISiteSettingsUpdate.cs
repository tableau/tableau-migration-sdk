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

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a site settings update operation.
    /// </summary>
    public interface ISiteSettingsUpdate
    {
        /// <summary>
        /// Gets the ID of the site to update settings for.
        /// </summary>
        Guid SiteId { get; }

        /// <summary>
        /// Gets the new extract encryption mode, or null to not update the setting.
        /// </summary>
        string? ExtractEncryptionMode { get; set; }

        /// <summary>
        /// Finds whether any settings require updates.
        /// </summary>
        /// <returns>True if any setting has changes, otherwise false.</returns>
        bool NeedsUpdate();
    }
}
