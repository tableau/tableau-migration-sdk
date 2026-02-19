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

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Search;

namespace Tableau.Migration.Api.Search
{
    internal class ApiContentReferenceStore<TContent> : IContentReferenceStore<TContent>
        where TContent : class, IContentReference
    {
        private readonly IPagedListApiClient<TContent> _listClient;
        private readonly IReadApiClient<TContent>? _readClient;
        private readonly INameSearchApiClient<TContent>? _nameSearchClient;
        private readonly IContentUrlSearchApiClient<TContent>? _contentUrlSearchClient;
        private readonly IConfigReader _configReader;

        /// <summary>
        /// Gets the configured batch size.
        /// </summary>
        protected int BatchSize => _configReader.Get<TContent>().BatchSize;

        /// <summary>
        /// Creates a new <see cref="ApiContentReferenceStore{TContent}"/> object.
        /// </summary>
        /// <param name="apiClient">An API client.</param>
        /// <param name="configReader">A config reader.</param>
        public ApiContentReferenceStore(ISitesApiClient? apiClient, IConfigReader configReader)
        {
            Guard.AgainstNull(apiClient, () => apiClient);

            _listClient = apiClient.GetListApiClient<TContent>();
            _readClient = apiClient.GetReadApiClient<TContent>();
            _nameSearchClient = apiClient.GetNameSearchApiClient<TContent>();
            _contentUrlSearchClient = apiClient.GetContentUrlSearchApiClient<TContent>();
            _configReader = configReader;
        }

        #region - IContentReferenceStore<TContent> Implementation -

        /// <inheritdoc />
        public async ValueTask<IImmutableList<TContent>> LoadAllAsync(CancellationToken cancel)
        {
            if(_listClient is not null)
            {
                var listResult = await _listClient.GetAllAsync(BatchSize, cancel).ConfigureAwait(false);
                if (listResult.Success)
                {
                    return listResult.Value;
                }
            }

            return [];
        }

        /// <inheritdoc />
        public async ValueTask<ContentReferenceLoadResult<TContent>> LoadAsync(Guid searchId, CancellationToken cancel)
        {
            if (_readClient is not null)
            {
                var searchResult = await _readClient.GetByIdAsync(searchId, cancel).ConfigureAwait(false);
                if (searchResult.Success)
                {
                    return new([searchResult.Value]);
                }

                return ContentReferenceLoadResult<TContent>.Empty;
            }

            return ContentReferenceLoadResult<TContent>.Unsupported;
        }

        /// <inheritdoc />
        public async ValueTask<ContentReferenceLoadResult<TContent>> LoadAsync(string searchContentUrl, CancellationToken cancel)
        {
            if (_contentUrlSearchClient is not null)
            {
                var searchResult = await _contentUrlSearchClient.SearchByContentUrlAsync(searchContentUrl, cancel).ConfigureAwait(false);
                if (searchResult.Success)
                {
                    return new(searchResult.Value);
                }

                return ContentReferenceLoadResult<TContent>.Empty;
            }

            return ContentReferenceLoadResult<TContent>.Unsupported;
        }

        /// <inheritdoc />
        public async ValueTask<ContentReferenceLoadResult<TContent>> LoadAsync(ContentLocation searchLocation, CancellationToken cancel)
        {
            if (_nameSearchClient is not null)
            {
                var searchResult = await _nameSearchClient.SearchByNameAsync(searchLocation.Name, BatchSize, cancel).ConfigureAwait(false);
                if (searchResult.Success)
                {
                    return new(searchResult.Value);
                }

                return ContentReferenceLoadResult<TContent>.Empty;
            }

            return ContentReferenceLoadResult<TContent>.Unsupported;
        }

        #endregion
    }
}
