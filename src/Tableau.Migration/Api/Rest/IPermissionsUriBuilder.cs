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
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Api.Rest
{
    /// <summary>
    /// An interface for building REST permission-related URIs.
    /// </summary>
    public interface IPermissionsUriBuilder
    {
        /// <summary>
        /// Gets the prefix of the URI (i.e. "projects" in /api/{api-version}/sites/{site-id}/projects/{project-id}/permissions).
        /// </summary>
        string Prefix { get; }

        /// <summary>
        /// Gets the suffix of the URI (i.e. "permissions" in /api/{api-version}/sites/{site-id}/projects/{project-id}/permissions).
        /// </summary>
        string Suffix { get; }

        /// <summary>
        /// Builds the URI for a permission operation. Use <see cref="BuildDeleteUri(Guid, ICapability, GranteeType, Guid)"/> for delete operations.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <returns>The URI string for the permissions URI.</returns>
        string BuildUri(Guid contentItemId);

        /// <summary>
        /// Builds the URI for a permission delete operation. Use <see cref="BuildUri(Guid)"/> for non-delete operations.
        /// </summary>
        /// <param name="contentItemId">The ID of the content item.</param>
        /// <param name="capability">The capability to delete.</param>
        /// <param name="granteeType">The type of grantee for the capability to delete.</param>
        /// <param name="granteeId">The ID of the grantee for the capability to delete.</param>
        /// <returns>The URI string for the permissions URI.</returns>
        string BuildDeleteUri(
            Guid contentItemId,
            ICapability capability,
            GranteeType granteeType,
            Guid granteeId);
    }
}