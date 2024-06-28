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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for listing or updating embedded connections of content items.
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>
        /// List the connection on the content item.
        /// </summary>
        /// <param name="urlPrefix">The URL prefix of the content item to get connections for.</param>
        /// <param name="contentItemId">The ID of the content item to get connections for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult<ImmutableList<IConnection>>> ListConnectionsAsync(
            string urlPrefix,
            Guid contentItemId,
            CancellationToken cancel);

        /// <summary>
        /// Update the connection on the content item.
        /// </summary>
        /// <param name="urlPrefix">The URL prefix of the content item to update connections for.</param>            
        /// <param name="contentItemId">The ID of the content item to update connections for.</param>
        /// <param name="connectionId">The ID of the connection to be updated.</param>
        /// <param name="options">The update connetion options.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult<IConnection>> UpdateConnectionAsync(
            string urlPrefix,
            Guid contentItemId,
            Guid connectionId,
            IUpdateConnectionOptions options,
            CancellationToken cancel);
    }
}