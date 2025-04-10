﻿//
//  Copyright (c) 2025, Salesforce, Inc.
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
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    /// <inheritdoc/>
    internal class Connection : IConnection
    {
        public Connection(IConnectionType response)
        {
            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);
            Type = Guard.AgainstNullOrEmpty(response.Type, () => response.Type);

            ServerAddress = response.ServerAddress;
            ServerPort = response.ServerPort;
            ConnectionUsername = response.ConnectionUsername;
            QueryTaggingEnabled = response.QueryTaggingEnabled.ToBoolOrNull();
            AuthenticationType = response.AuthenticationType;
            EmbedPassword = response.EmbedPassword.ToBoolOrNull();
            UseOAuthManagedKeychain = response.UseOAuthManagedKeychain.ToBoolOrNull();
        }

        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <inheritdoc/>
        public string Type { get; set; }

        /// <inheritdoc/>
        public string? ServerAddress { get; set; }

        /// <inheritdoc/>
        public string? ServerPort { get; set; }

        /// <inheritdoc/>
        public string? ConnectionUsername { get; set; }

        /// <inheritdoc/>
        public bool? QueryTaggingEnabled { get; set; }

        /// <inheritdoc/>
        public string? AuthenticationType { get; set; }

        /// <inheritdoc/>
        public bool? UseOAuthManagedKeychain { get; set; }

        /// <inheritdoc/>
        public bool? EmbedPassword { get; set; }
    }
}
