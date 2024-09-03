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
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using Tableau.Migration.Net.Simulation;
using Tableau.Migration.Net.Simulation.Requests;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Requests
{
    internal record RestApiRequestMatcher(Uri BaseUrl, HttpMethod HttpMethod, Regex UrlPattern, IEnumerable<KeyValuePair<string, Regex>> QueryStringPatterns) : IRequestMatcher
    {
        protected virtual bool SupportsVersion(decimal restApiVersion) => true;

        public virtual bool Matches(HttpRequestMessage request)
        {
            if (request.Method != HttpMethod)
            {
                return false;
            }

            if (request.RequestUri is null)
            {
                return false;
            }

            if (!BaseUrlComparer.Instance.Equals(request.RequestUri, BaseUrl))
            {
                return false;
            }

            var restApiPathMatch = UrlPattern.Match(request.RequestUri.TrimmedPath());
            if (!restApiPathMatch.Success)
            {
                return false;
            }

            if (QueryStringPatterns.Any())
            {
                var requestQuery = HttpUtility.ParseQueryString(request.RequestUri.Query);
                foreach (var queryStringPattern in QueryStringPatterns)
                {
                    var requestQueryValue = requestQuery[queryStringPattern.Key];
                    var queryMatch = queryStringPattern.Value.Match(requestQueryValue ?? string.Empty);
                    if (!queryMatch.Success)
                    {
                        return false;
                    }
                }
            }

            var restApiVersionGroup = restApiPathMatch.Groups[RestUrlPatterns.VersionGroupName];
            if (restApiVersionGroup is null
                || (!decimal.TryParse(restApiVersionGroup.Value, out var restApiVersion) 
                    && restApiVersionGroup.Value != ApiClient.EXPERIMENTAL_API_VERSION))
            {
                return false;
            }

            if (!SupportsVersion(restApiVersion))
            {
                return false;
            }

            return true;
        }

        public bool Equals(IRequestMatcher? other)
        {
            if (other is null || other is not RestApiRequestMatcher restMatcher)
            {
                return false;
            }

            return Equals(restMatcher);
        }

        public override string ToString() => $"{nameof(UrlPattern)}: {UrlPattern}, {nameof(HttpMethod)}: {HttpMethod}, {nameof(BaseUrl)}: {BaseUrl}";
    }
}
