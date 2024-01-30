// Copyright (c) 2023, Salesforce, Inc.
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
using Tableau.Migration.Api;

namespace Tableau.Migration.Net.Rest
{
    internal sealed class RestRequestBuilderFactory : RequestBuilderFactory<IRestRequestBuilder>, IRestRequestBuilderFactory
    {
        private readonly IServerSessionProvider _sessionProvider;
        private readonly IHttpRequestBuilderFactory _requestBuilderFactory;

        private string? _apiVersion;
        private string? _siteId;

        public RestRequestBuilderFactory(
            IRequestBuilderFactoryInput input,
            IServerSessionProvider sessionProvider,
            IHttpRequestBuilderFactory requestBuilderFactory)
            : base(input)
        {
            _sessionProvider = sessionProvider;
            _requestBuilderFactory = requestBuilderFactory;
        }

        public void SetDefaultApiVersion(string? version) => _apiVersion = version;

        public void SetDefaultSiteId(Guid? siteId) => SetDefaultSiteId(siteId?.ToUrlSegment());

        public void SetDefaultSiteId(string? siteId) => _siteId = siteId;

        public override IRestRequestBuilder CreateUri(string path)
        {
            var builder = new RestRequestBuilder(BaseUri, path, _requestBuilderFactory);

            if (_apiVersion is null && _sessionProvider.Version?.RestApiVersion is not null)
                _apiVersion = _sessionProvider.Version?.RestApiVersion;

            if (_siteId is null && _sessionProvider.SiteId is not null)
                _siteId = _sessionProvider.SiteId.Value.ToUrlSegment();

            if (_apiVersion is not null)
                builder.WithApiVersion(_apiVersion);

            if (_siteId is not null)
                builder.WithSiteId(_siteId);

            return builder;
        }
    }
}
