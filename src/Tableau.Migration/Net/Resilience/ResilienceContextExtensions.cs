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
using Polly;

namespace Tableau.Migration.Net.Resilience
{
    internal static class ResilienceContextExtensions
    {
        /* Key string value taken from 
         * https://github.com/dotnet/extensions/blob/v8.1.0/src/Libraries/Microsoft.Extensions.Http.Resilience/Internal/ResilienceKeys.cs
         * due to the value being internal.
         */
        internal static readonly ResiliencePropertyKey<HttpRequestMessage> REQUEST_CONTEXT_KEY 
            = new("Resilience.Http.RequestMessage");

        internal static HttpRequestMessage GetRequest(this ResilienceContext ctx)
        {
            if (ctx.Properties.TryGetValue(REQUEST_CONTEXT_KEY, out var request))
            {
                return request;
            }

            throw new InvalidOperationException("No request was provided for resilience strategy context.");
        }
    }
}
