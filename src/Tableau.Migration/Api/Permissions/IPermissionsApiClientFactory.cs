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

using Tableau.Migration.Api.Rest;

namespace Tableau.Migration.Api.Permissions
{
    /// <summary>
    /// Interface for permissions API client factories.
    /// </summary>
    public interface IPermissionsApiClientFactory
    {
        /// <summary>
        /// Creates an <see cref="IPermissionsApiClient"/> instance.
        /// </summary>
        /// <param name="uriBuilder">The permission URI builder.</param>
        /// <returns>The created <see cref="IPermissionsApiClient"/>.</returns>
        IPermissionsApiClient Create(IPermissionsUriBuilder uriBuilder);

        /// <summary>
        /// Creates an <see cref="IPermissionsApiClient"/> instance.
        /// </summary>
        /// <param name="contentApiClient">The content API client to use to determine the URL prefix.</param>
        /// <returns>The created <see cref="IPermissionsApiClient"/>.</returns>
        IPermissionsApiClient Create(IContentApiClient contentApiClient);

        /// <summary>
        /// Creates an <see cref="IDefaultPermissionsApiClient"/> instance.
        /// </summary>
        IDefaultPermissionsApiClient CreateDefaultPermissionsClient();
    }
}