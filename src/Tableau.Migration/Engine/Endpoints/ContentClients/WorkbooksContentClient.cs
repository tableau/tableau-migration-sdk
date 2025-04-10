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
using System.Collections.Immutable;
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
    public class WorkbooksContentClient : ContentClientBase<IWorkbook>, IWorkbooksContentClient
    {
        private readonly IWorkbooksApiClient _workbooksApiClient;

        /// <inheritdoc/>
        public WorkbooksContentClient(
            IWorkbooksApiClient workbooksApiClient,
            ILogger<IWorkbooksContentClient> logger,
            ISharedResourcesLocalizer localizer) : base(logger, localizer)
        {
            _workbooksApiClient = workbooksApiClient;
        }

        /// <inheritdoc/>
        public async Task<IResult<IImmutableList<IView>>> GetViewsForWorkbookIdAsync(Guid workbookId, CancellationToken cancel)
        {
            var workbook = await _workbooksApiClient.GetWorkbookAsync(workbookId, cancel).ConfigureAwait(false);

            if (!workbook.Success)
            {
                return workbook.CastFailure<IImmutableList<IView>>();
            }

            return Result<IImmutableList<IView>>.Succeeded(workbook.Value.Views);
        }
    }
}
