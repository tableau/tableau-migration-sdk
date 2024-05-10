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

using Tableau.Migration.Api.Search;
using Tableau.Migration.Config;

namespace Tableau.Migration.Engine.Endpoints.Search
{
    /// <summary>
    /// <see cref="BulkApiContentReferenceCache{TContent}"/> implementation
    /// that is built from ISourceEndpoint.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class BulkSourceCache<TContent> : BulkApiContentReferenceCache<TContent>
        where TContent : class, IContentReference
    {
        /// <summary>
        /// Creates a new <see cref="BulkSourceCache{TContent}"/>
        /// </summary>
        /// <param name="endpoint">The source endpoint.</param>
        /// <param name="configReader">A config reader.</param>
        public BulkSourceCache(
            ISourceEndpoint endpoint,
            IConfigReader configReader)
            : base((endpoint as ISourceApiEndpoint)?.SiteApi, configReader)
        {
        }
    }
}
