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
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Object that can simulate API responses like Tableau Server/Cloud.
    /// Used in 'simulation' integration tests so real SDK components can be tested together without full Tableau Server/Cloud infrastructure.
    /// Unless overridden all responses are successful and well-formed.
    /// </summary>
    public sealed class TableauApiSimulator
    {
        /// <summary>
        /// Gets the URL for the simulated Tableau Server.
        /// </summary>
        public Uri ServerUrl { get; }

        /// <summary>
        /// Gets the simulator for mocking server responses.
        /// </summary>
        public TableauApiResponseSimulator ResponseSimulator { get; }

        /// <summary>
        /// Gets the data store for the API.
        /// </summary>
        public TableauData Data { get; }

        /// <summary>
        /// Gets the simulator for REST API requests.
        /// </summary>
        public RestApiSimulator RestApi { get; }

        /// <summary>
        /// Creates a new <see cref="TableauApiSimulator"/> object.
        /// </summary>
        /// <param name="serverUrl">The base server URL.</param>
        /// <param name="serializer">A serializer to use to produce responses.</param>
        /// <param name="isTableauServer">Indicates whether the current Tableau Data is for Tableau Server (true) or Tableau Cloud (false).</param>
        /// <param name="defaultSignedInUser">Default signed in user to user. If none is provided, the simulated server will have no users.</param>
        /// <param name="defaultDomain">The default domain of the site.</param>
        public TableauApiSimulator(
            Uri serverUrl, 
            IHttpContentSerializer serializer,
            bool isTableauServer,
            UsersResponse.UserType? defaultSignedInUser = null, 
            string defaultDomain = Constants.LocalDomain)
        {
            ServerUrl = serverUrl;
            Data = new(defaultSignedInUser, defaultDomain);
            Data.IsTableauServer = isTableauServer;
            ResponseSimulator = new TableauApiResponseSimulator(ServerUrl, Data, serializer);
            RestApi = new(ResponseSimulator);
        }
    }
}
