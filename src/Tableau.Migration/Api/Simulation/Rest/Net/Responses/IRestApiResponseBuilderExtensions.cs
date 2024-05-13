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

using System.Linq;
using System.Net.Http;
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal static class IRestApiResponseBuilderExtensions
    {
        public static bool IsUnauthorizedRequest(this IRestApiResponseBuilder builder, HttpRequestMessage request, TableauData data)
        {
            if (!builder.RequiresAuthentication)
                return false;

            if (data.SignIn?.Token is null)
                return false;

            request.Headers.TryGetValues(RestHeaders.AuthenticationToken, out var tokenValues);

            if (tokenValues.IsNullOrEmpty() || tokenValues.First() != data.SignIn.Token)
                return true;

            return false;
        }
    }
}
