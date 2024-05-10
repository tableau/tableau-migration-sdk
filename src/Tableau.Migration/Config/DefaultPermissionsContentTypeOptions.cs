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
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Config
{
    /// <summary>
    /// Options for <see cref="DefaultPermissionsContentTypeOptions"/>.
    /// </summary>
    public class DefaultPermissionsContentTypeOptions
    {
        /// <summary>
        /// <para>
        /// Gets or sets the corresponding URL segments for default permissions content types.
        /// </para>
        /// <para>
        /// For example, for the URL "/api/api-version/sites/site-luid/projects/project-luid/default-permissions/workbooks" the URL segment would be "workbooks".
        /// </para>
        /// </summary>
        public HashSet<string> UrlSegments { get; } = new(DefaultPermissionsContentTypeUrlSegments.GetAll(), StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Creates a new <see cref="DefaultPermissionsContentTypeOptions"/> instance.
        /// </summary>
        /// <param name="urlSegments">The optional custom default permissions content types.</param>
        public DefaultPermissionsContentTypeOptions(IEnumerable<string>? urlSegments = null)
        {
            if (!urlSegments.IsNullOrEmpty())
            {
                foreach (var urlSegment in urlSegments)
                {
                    if (!String.IsNullOrWhiteSpace(urlSegment))
                        UrlSegments.Add(urlSegment);
                }
            }
        }
    }
}
