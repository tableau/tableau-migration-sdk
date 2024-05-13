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
using Tableau.Migration.Content;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client prep flow operations.
    /// </summary>
    public interface IFlowsApiClient :
        IApiPageAccessor<IFlow>,
        IPagedListApiClient<IFlow>,
        IPullApiClient<IFlow, IPublishableFlow>,
        IPublishApiClient<IPublishableFlow, IFlow>
        /*IOwnershipApiClient,
        ITagsContentApiClient,
        IPermissionsContentApiClient,
        */
    {
        /// <summary>
        /// Gets all prep flows in the current site.
        /// </summary>
        /// <param name="pageNumber">The 1-indexed page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>A list of a page of prep flows in the current site.</returns>
        Task<IPagedResult<IFlow>> GetAllFlowsAsync(int pageNumber, int pageSize, CancellationToken cancel);

        /// <summary>
        /// Downloads the prep flow file for the given ID.
        /// </summary>
        /// <param name="flowId">The ID to download the flow file for.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The file download result.</returns>
        Task<IAsyncDisposableResult<FileDownload>> DownloadFlowAsync(Guid flowId, CancellationToken cancel);

        /// <summary>
        /// Uploads the input prep flow file.
        /// </summary>
        /// <param name="options">The new prep flows's details.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The published prep flow.</returns>
        Task<IResult<IFlow>> PublishFlowAsync(IPublishFlowOptions options, CancellationToken cancel);
    }
}
