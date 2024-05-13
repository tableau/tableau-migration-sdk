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
using System.Net.Http;

namespace Tableau.Migration.Net.Simulation.Requests
{
    /// <summary>
    /// Default <see cref="IPathRequestMatcher"/> implementation.
    /// </summary>
    /// <param name="Method"><inheritdoc /></param>
    /// <param name="RequestUrl"><inheritdoc /></param>
    public record PathRequestMatcher(HttpMethod Method, Uri RequestUrl) : IPathRequestMatcher
    {
        /// <inheritdoc />
        public bool Matches(HttpRequestMessage request)
        {
            if (request.Method != Method)
            {
                return false;
            }

            if (request.RequestUri is null)
            {
                return false;
            }

            if (!BaseUrlComparer.Instance.Equals(request.RequestUri, RequestUrl))
            {
                return false;
            }

            if (!string.Equals(request.RequestUri.TrimmedPath(), RequestUrl.TrimmedPath()))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public bool Equals(IRequestMatcher? other)
        {
            if (other is null || other is not IPathRequestMatcher pathMatcher)
            {
                return false;
            }

            return Equals(new(pathMatcher.Method, pathMatcher.RequestUrl));
        }

        /// <summary>
        /// Returns a string which represents the object instance.
        /// </summary>
        public override string ToString() => $"{nameof(RequestUrl)}: {RequestUrl}, {nameof(Method)}: {Method}";
    }
}
