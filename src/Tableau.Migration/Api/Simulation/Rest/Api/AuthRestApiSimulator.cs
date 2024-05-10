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

using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API authentication methods.
    /// </summary>
    public sealed class AuthRestApiSimulator
    {
        /// <summary>
        /// Gets the simulated sign in API method.
        /// </summary>
        public MethodSimulator SignIn { get; }

        /// <summary>
        /// Gets the simulated sign out API method.
        /// </summary>
        public MethodSimulator SignOut { get; }

        /// <summary>
        /// Creates a new <see cref="AuthRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public AuthRestApiSimulator(TableauApiResponseSimulator simulator)
        {
            SignIn = simulator.SetupRestPost<SignInResponse, SignInResponse.CredentialsType>(RestApiUrl("auth/signin"), d => d.SignIn, requiresAuthentication: false);
            SignOut = simulator.SetupRestPost(RestApiUrl("auth/signout"), new EmptyRestResponseBuilder(simulator.Data, simulator.Serializer, false));
        }
    }
}
