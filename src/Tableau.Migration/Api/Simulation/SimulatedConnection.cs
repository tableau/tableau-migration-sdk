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
using Tableau.Migration.Api.Rest.Models.Requests;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// The connections type in the API request body.
    /// </summary>
    public class SimulatedConnection
    {
        /// <summary>
        /// Default parameterless constructor
        /// </summary>
        public SimulatedConnection()
        { }

        /// <summary>
        /// Method to update the simulated connection from the REST API update request.
        /// </summary>
        /// <param name="update"></param>
        public void Update(UpdateConnectionRequest.ConnectionType update)
        {
            if (update.ServerAddress != null && !string.Equals(ServerAddress, update.ServerAddress))
            {
                ServerAddress = update.ServerAddress;
            }

            if (update.ServerPort != null && !string.Equals(ServerPort, update.ServerPort))
            {
                ServerPort = update.ServerPort;
            }

            if (update.QueryTaggingEnabled != null && !string.Equals(QueryTaggingEnabled, update.QueryTaggingEnabled))
            {
                if (!bool.TryParse(update.QueryTaggingEnabled, out bool queryTaggingEnabled))
                {
                    throw new ArgumentException(
                        $"{nameof(update.QueryTaggingEnabled)} should be boolean. Current value is {update.QueryTaggingEnabled}.",
                        nameof(update));
                }
                QueryTaggingEnabled = queryTaggingEnabled;
            }

            if (Credentials == null)
            {
                Credentials = new SimulatedConnectionCredentials();
            }

            if (update.ConnectionUsername != null && !string.Equals(Credentials.Name, update.ConnectionUsername))
            {
                Credentials.Name = update.ConnectionUsername;
            }

            if (update.Password != null && !string.Equals(Credentials.Password, update.Password))
            {
                Credentials.Password = update.Password;
            }

            if (update.EmbedPassword != null && !string.Equals(Credentials.Embed, update.EmbedPassword))
            {
                Credentials.Embed = update.EmbedPassword;
            }
        }


        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the server address.
        /// </summary>        
        public string? ServerAddress { get; set; }

        /// <summary>
        /// Gets or sets the server port.
        /// </summary>        
        public string? ServerPort { get; set; }


        /// <summary>
        /// Gets or sets the connection type.
        /// </summary> 
        public string? ConnectionType { get; set; }

        /// <summary>
        /// Gets or sets the query tagging enabled flag.
        /// </summary>
        public bool? QueryTaggingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>        
        public SimulatedConnectionCredentials? Credentials { get; set; }
    }
}
