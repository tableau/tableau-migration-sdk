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
    /// Interface for a site.
    /// </summary>
    public interface ISite //We don't implement IContentReference because sites aren't a true 'content type.'
        : ISiteSettings
    {
        /// <summary>
        /// Gets the unique identifier of the site, 
        /// corresponding to the LUID in the Tableau REST API.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the site's friendly name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the site's "content URL".
        /// </summary>
        string ContentUrl { get; }
    }
}