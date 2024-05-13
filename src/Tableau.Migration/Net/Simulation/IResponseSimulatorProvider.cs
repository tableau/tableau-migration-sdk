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

using System.Net.Http;

namespace Tableau.Migration.Net.Simulation
{
    /// <summary>
    /// Interface for an object that can provide response simulators for a given request.
    /// </summary>
    public interface IResponseSimulatorProvider
    {
        /// <summary>
        /// Gets a response simulator for the given request, or null.
        /// </summary>
        /// <param name="request">The request to get the response simulator for.</param>
        /// <returns>The response simulator, or null if there is no response simulator for the given request.</returns>
        IResponseSimulator? ForRequest(HttpRequestMessage request);
    }
}
