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

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client authentication configuration operations.
    /// </summary>
    public interface IAuthenticationConfigurationsApiClient : IPagedListApiClient<IAuthenticationConfiguration>
    {
        /// <summary>
        /// Gets all the authentication configurations on the site.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The list of authentication configurations.</returns>
        Task<IResult<IImmutableList<IAuthenticationConfiguration>>> GetAuthenticationConfigurationsAsync(CancellationToken cancel);
    }
}
