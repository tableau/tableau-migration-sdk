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
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Content
{
    internal sealed class Site : ISite
    {
        /// <summary>
        /// Gets the site content URL that is used by Tableau to represent the Default site on all installations.
        /// </summary>
        public const string DefaultContentUrl = "";

        /// <summary>
        /// Gets a <see cref="StringComparer"/> suitable for comparing content URLs for sites.
        /// </summary>
        public static readonly StringComparer ContentUrlComparer = StringComparer.OrdinalIgnoreCase;

        /// <inheritdoc />
        public Guid Id { get; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string ContentUrl { get; }

        /// <inheritdoc />
        public string ExtractEncryptionMode { get; set; }

        public Site(SiteResponse response)
        {
            var site = Guard.AgainstNull(response.Item, () => response.Item);

            Id = Guard.AgainstDefaultValue(site.Id, () => response.Item.Id);
            Name = Guard.AgainstNullEmptyOrWhiteSpace(site.Name, () => response.Item.Name);
            ContentUrl = Guard.AgainstNull(site.ContentUrl, () => response.Item.ContentUrl);
            ExtractEncryptionMode = Guard.AgainstNullEmptyOrWhiteSpace(site.ExtractEncryptionMode, () => response.Item.ExtractEncryptionMode);
        }
    }
}
