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

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a content item's embedded connection.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the connection type for the response.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the server address for the response.
        /// </summary>
        string? ServerAddress { get; }

        /// <summary>
        /// Gets the server port for the response.
        /// </summary>
        string? ServerPort { get; }

        /// <summary>
        /// Gets the connection username for the response.
        /// </summary>
        string? ConnectionUsername { get; }

        /// <summary>
        /// Gets the query tagging enabled flag for the response. 
        /// This is returned only for administrator users.
        /// </summary>
        bool? QueryTaggingEnabled { get; }

        /// <summary>
        /// Gets the authentication type for the response.
        /// </summary>
        string? AuthenticationType { get; }

        /// <summary>
        /// Gets whether to use OAuth managed keychain.
        /// </summary>
        bool? UseOAuthManagedKeychain { get; }

        /// <summary>
        /// Gets whether to embed the password.
        /// </summary>
        bool? EmbedPassword { get; }
    }
}
