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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api;
using Tableau.Migration.Content;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Endpoints.ContentClients
{
    /// <summary>
    /// Content client to interact with workbooks.
    /// </summary>
    public class ViewsContentClient : ContentClientBase<IView>, IViewsContentClient
    {
        private readonly IViewsApiClient _viewsApiClient;

        /// <inheritdoc/>
        public ViewsContentClient(
            IViewsApiClient viewsApiClient,
            ILogger<IViewsContentClient> logger,
            ISharedResourcesLocalizer localizer) : base(logger, localizer)
        {
            _viewsApiClient = viewsApiClient;
        }

        /// <inheritdoc/>
        public Task<IResult<IView>> GetByIdAsync(Guid id, CancellationToken cancel)
        {
            return _viewsApiClient.GetByIdAsync(id, cancel);
        }
    }
}
