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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for a class representing the current session details for a signed-in user.
    /// </summary>
    public interface IServerSessionProvider
    {
        /// <summary>
        /// Gets the server's version information.
        /// </summary>
        TableauServerVersion? Version { get; }

        /// <summary>
        /// Gets the current site's content URL.
        /// </summary>
        string? SiteContentUrl { get; }

        /// <summary>
        /// Gets the current site's ID.
        /// </summary>
        Guid? SiteId { get; }

        /// <summary>
        /// Gets the current user's ID.
        /// </summary>
        Guid? UserId { get; }

        /// <summary>
        /// The type of Tableau instance connected to. One of the values in <see cref="InstanceType"/>.
        /// </summary>
        TableauInstanceType InstanceType { get; }

        /// <summary>
        /// Sets the current session information.
        /// </summary>
        /// <param name="signInResult">The sign-in result containing the current user and site information.</param>
        /// <param name="instanceType">The instance type connected to. It can be one of <see cref="TableauInstanceType"/>.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The task to await.</returns>
        Task SetCurrentSessionAsync(ISignInResult signInResult, TableauInstanceType instanceType, CancellationToken cancel);

        /// <summary>
        /// Sets the current session information.
        /// </summary>
        /// <param name="userId">The current user's ID.</param>
        /// <param name="siteId">The current site's ID.</param>
        /// <param name="siteContentUrl">The current site's content URL.</param>
        /// <param name="authenticationToken">The current user's authentication token.</param>
        /// <param name="instanceType">The instance type connected to. It can be one of <see cref="TableauInstanceType"/>.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The task to await.</returns>
        Task SetCurrentSessionAsync(Guid userId, Guid siteId, string siteContentUrl, string authenticationToken, TableauInstanceType instanceType, CancellationToken cancel);

        /// <summary>
        /// Clears the current session information.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The task to await.</returns>
        Task ClearCurrentSessionAsync(CancellationToken cancel);

        /// <summary>
        /// Sets the current version information.
        /// </summary>
        /// <param name="version">The server's version information.</param>
        void SetVersion(TableauServerVersion version);
    }
}