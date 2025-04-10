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

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Default <see cref="IDestinationSiteInfo"/> implementation.
    /// </summary>
    public class DestinationSiteInfo : IDestinationSiteInfo
    {
        /// <summary>
        /// Creates a new <see cref="DestinationSiteInfo"/> object.
        /// </summary>
        /// <param name="destinationSiteContentUrl">The Content URL of the destination Tableau site.</param>
        /// <param name="destinationSiteId">The ID of the destination Tableau site.</param>
        /// <param name="destinationSiteUrl">The url of the the destination Tableau instance.</param>
        public DestinationSiteInfo(
            string destinationSiteContentUrl,
            Guid destinationSiteId,
            string destinationSiteUrl)
        {
            ContentUrl = destinationSiteContentUrl;
            SiteId = destinationSiteId;
            SiteUrl = destinationSiteUrl;
        }

        /// <inheritdoc/>
        public string ContentUrl { get; }

        /// <inheritdoc/>
        public Guid SiteId { get; }

        /// <inheritdoc/>
        public string SiteUrl { get; }
    }
}
