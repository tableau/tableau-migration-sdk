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
    /// Interface for an object that matches HTTP requests to simulate a response for.
    /// </summary>
    public interface IRequestMatcher : IEquatable<IRequestMatcher>
    {
        /// <summary>
        /// Determines whether the request matches this matcher's criteria.
        /// </summary>
        /// <param name="request">The request to attempt to match.</param>
        /// <returns>True if the request matches, otherwise false.</returns>
        bool Matches(HttpRequestMessage request);
    }
}
