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
using Tableau.Migration.Api;

namespace Tableau.Migration.Net.Rest
{
    internal sealed class RestRequestBuilderFactory : RequestBuilderFactory<IRestRequestBuilder>, IRestRequestBuilderFactory
    {
        private readonly IServerSessionProvider _sessionProvider;
        private readonly IHttpRequestBuilderFactory _requestBuilderFactory;

        public RestRequestBuilderFactory(
            IRequestBuilderFactoryInput input,
            IServerSessionProvider sessionProvider,
            IHttpRequestBuilderFactory requestBuilderFactory)
            : base(input)
        {
            _sessionProvider = sessionProvider;
            _requestBuilderFactory = requestBuilderFactory;
        }

        private string? GetApiVersion()
            => _sessionProvider.Version?.RestApiVersion;

        private string? GetSiteId()
            => _sessionProvider.SiteId?.ToUrlSegment();

        public override IRestRequestBuilder CreateUri(string path, bool useExperimental = false)
        {
            var builder = new RestRequestBuilder(BaseUri, path, _requestBuilderFactory);

            if (useExperimental)
            {
                builder.WithApiVersion(ApiClient.EXPERIMENTAL_API_VERSION);
            }
            else
            {
                var apiVersion = GetApiVersion();
                if(apiVersion is not null)
                {
                    builder.WithApiVersion(apiVersion);
                }
            }

            var siteId = GetSiteId();
            if (siteId is not null)
                builder.WithSiteId(siteId);

            return builder;
        }
    }
}
