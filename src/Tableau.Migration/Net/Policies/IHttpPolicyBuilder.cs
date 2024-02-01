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

using System.Net.Http;
using Polly;

namespace Tableau.Migration.Net.Policies
{
    /// <summary>
    /// Abstraction build a policy that apply for a given http request.
    /// <see href="https://github.com/App-vNext/Polly"/>
    /// </summary>
    public interface IHttpPolicyBuilder
    {
        /// <summary>
        /// Build and return the policy that apply for the http request.
        /// </summary>
        /// <param name="httpRequest">The http request that we will request the policies</param>
        /// <returns>A async policy that apply to a given http response of a http request.</returns>
        IAsyncPolicy<HttpResponseMessage>? Build(HttpRequestMessage httpRequest);
    }
}