//
//  Copyright (c) 2026, Salesforce, Inc.
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
using Tableau.Migration.Config;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Api.Search
{
    /// <summary>
    /// Abstract base class for <see cref="IContentReferenceCache"/> implementations that
    /// load items from API clients.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public abstract class ApiContentReferenceCacheBase<TContent> : ContentReferenceCacheBase<TContent>
        where TContent : class, IContentReference
    {
        /// <summary>
        /// Creates a new <see cref="ApiContentReferenceCacheBase{TContent}"/> object.
        /// </summary>
        /// <param name="loadStrategy"><inheritdoc /></param>
        /// <param name="apiClient">An API client.</param>
        /// <param name="configReader">A config reader.</param>
        /// <param name="logger"><inheritdoc /></param>
        public ApiContentReferenceCacheBase(IContentReferenceCacheLoadStrategy<TContent> loadStrategy, 
            ISitesApiClient? apiClient, IConfigReader configReader,
            ILogger<ApiContentReferenceCacheBase<TContent>> logger)
            : base(loadStrategy, new ApiContentReferenceStore<TContent>(apiClient, configReader), logger)
        { }
    }
}
