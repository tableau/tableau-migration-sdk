﻿// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Api.Search
{
    /// <summary>
    /// <see cref="IContentReferenceCache"/> implementation that loads content items
    /// in bulk from an API list client.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class BulkApiContentReferenceCache<TContent> : ContentReferenceCacheBase
        where TContent : IContentReference
    {
        private readonly IPagedListApiClient<TContent> _apiListClient;
        private readonly IConfigReader _configReader;

        /// <summary>
        /// Creates a new <see cref="BulkApiContentReferenceCache{TContent}"/> object.
        /// </summary>
        /// <param name="apiClient">An API client.</param>
        /// <param name="configReader">A config reader.</param>
        public BulkApiContentReferenceCache(ISitesApiClient apiClient, IConfigReader configReader)
        {
            _apiListClient = apiClient.GetListApiClient<TContent>();
            _configReader = configReader;
        }

        /// <summary>
        /// Gets the configured batch size.
        /// </summary>
        protected int BatchSize => _configReader.Get().BatchSize;

        /// <summary>
        /// Loads all content items from the API client.
        /// </summary>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>All content items.</returns>
        protected async ValueTask<IEnumerable<ContentReferenceStub>> LoadAllAsync(CancellationToken cancel)
        {
            var listResult = await _apiListClient.GetAllAsync(BatchSize, cancel).ConfigureAwait(false);

            if (!listResult.Success)
            {
                return Enumerable.Empty<ContentReferenceStub>();
            }

            return listResult.Value.Select(i => new ContentReferenceStub(i));
        }

        /// <inheritdoc />
        protected override async ValueTask<IEnumerable<ContentReferenceStub>> SearchAsync(ContentLocation searchLocation, CancellationToken cancel)
            => await LoadAllAsync(cancel).ConfigureAwait(false);

        /// <inheritdoc />
        protected override async ValueTask<IEnumerable<ContentReferenceStub>> SearchAsync(Guid searchId, CancellationToken cancel)
            => await LoadAllAsync(cancel).ConfigureAwait(false);
    }
}
