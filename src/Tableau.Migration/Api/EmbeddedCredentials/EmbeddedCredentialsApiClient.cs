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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api.EmbeddedCredentials
{
    internal class EmbeddedCredentialsApiClient : IEmbeddedCredentialsApiClient
    {
        private readonly IRestRequestBuilderFactory _restRequestBuilderFactory;
        private readonly IHttpContentSerializer _serializer;
        private readonly ISharedResourcesLocalizer _sharedResourcesLocalizer;
        private readonly string _urlPrefix;
        private readonly ILoggerFactory _loggerFactory;

        public EmbeddedCredentialsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            string urlPrefix,
            IHttpContentSerializer serializer)
        {
            _restRequestBuilderFactory = restRequestBuilderFactory;
            _loggerFactory = loggerFactory;
            _sharedResourcesLocalizer = sharedResourcesLocalizer;
            _urlPrefix = urlPrefix;
            _serializer = serializer;
        }

        #region - IEmbeddedCredentialsApiClient Implementation -

        /// <inheritdoc />
        public async Task<IResult<IEmbeddedCredentialKeychainResult>> RetrieveKeychainAsync(
            Guid contentItemId,
            IDestinationSiteInfo destinationSiteInfo,
            CancellationToken cancel)
        {
            return await _restRequestBuilderFactory
                .CreateUri($"{_urlPrefix}/{contentItemId.ToUrlSegment()}/retrievekeychain")
                .ForPostRequest()
                .WithXmlContent(new RetrieveKeychainRequest(destinationSiteInfo))
                .SendAsync<RetrieveKeychainResponse>(cancel)
                .ToResultAsync<RetrieveKeychainResponse, IEmbeddedCredentialKeychainResult>(
                    (response) =>
                    new EmbeddedCredentialKeychainResult(response),
                    _sharedResourcesLocalizer)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult> ApplyKeychainAsync(Guid contentItemId, IApplyKeychainOptions options, CancellationToken cancel)
        {
            return await _restRequestBuilderFactory
                .CreateUri($"{_urlPrefix}/{contentItemId.ToUrlSegment()}/applykeychain")
                .ForPutRequest()
                .WithXmlContent(new ApplyKeychainRequest(options))
                .SendAsync(cancel)
                .ToResultAsync(_serializer, _sharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);
        }

        #endregion
    }
}