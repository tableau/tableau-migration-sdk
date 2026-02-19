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
    /// <see cref="IContentReferenceCache"/> implementation that loads content items
    /// in bulk from an API list client.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class BulkApiContentReferenceCache<TContent> : ApiContentReferenceCacheBase<TContent>
        where TContent : class, IContentReference
    {
        /// <inheritdoc />
        protected override string Name => $"Bulk API fallback {typeof(TContent)}";

        /// <summary>
        /// Creates a new <see cref="BulkApiContentReferenceCache{TContent}"/> object.
        /// </summary>
        /// <param name="apiClient">An API client.</param>
        /// <param name="configReader">A config reader.</param>
        /// <param name="logger"><inheritdoc /></param>
        public BulkApiContentReferenceCache(ISitesApiClient? apiClient, IConfigReader configReader,
            ILogger<BulkApiContentReferenceCache<TContent>> logger) 
            : base(new BulkContentReferenceCacheLoadStrategy<TContent>(), apiClient, configReader, logger)
        { }
    }
}
