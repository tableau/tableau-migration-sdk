//
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
    /// Structure representing the authentication type of a user.
    /// </summary>
    public readonly record struct UserAuthenticationType
    {
        /// <summary>
        /// Gets the authentication type, 
        /// or null if the site uses <see cref="IdpConfigurationId"/>s.
        /// </summary>
        public string? AuthenticationType { get; }

        /// <summary>
        /// Gets the IdP configuration ID,
        /// or null if the site uses <see cref="AuthenticationType"/>s.
        /// </summary>
        public Guid? IdpConfigurationId { get; }

        /// <summary>
        /// Gets a value representing the site default authentication type.
        /// </summary>
        public static readonly UserAuthenticationType Default = new(null, null);

        /// <summary>
        /// Creates a new <see cref="UserAuthenticationType"/> value.
        /// </summary>
        /// <param name="authenticationType">The authentication type, or null if <paramref name="idpConfigurationId"/> is non-null.</param>
        /// <param name="idpConfigurationId">The IdP configuration ID, or null if <paramref name="authenticationType"/> is non-null.</param>
        internal UserAuthenticationType(string? authenticationType, Guid? idpConfigurationId)
        {
            AuthenticationType = authenticationType;
            IdpConfigurationId = idpConfigurationId;
        }

        /// <summary>
        /// Creates a new <see cref="UserAuthenticationType"/> value.
        /// </summary>
        /// <param name="authenticationType">The authentication type.</param>
        /// <returns>The created <see cref="UserAuthenticationType"/> value.</returns>
        public static UserAuthenticationType ForAuthenticationType(string authenticationType)
            => new(authenticationType, null);

        /// <summary>
        /// Creates a new <see cref="UserAuthenticationType"/> value.
        /// </summary>
        /// <param name="idpConfigurationId">The IdP configuration ID.</param>
        /// <returns>The created <see cref="UserAuthenticationType"/> value.</returns>
        public static UserAuthenticationType ForConfigurationId(Guid idpConfigurationId)
            => new(null, idpConfigurationId);
    }
}
