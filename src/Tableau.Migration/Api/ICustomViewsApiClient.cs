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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client Custom View operations.
    /// </summary>
    public interface ICustomViewsApiClient :
        IContentApiClient,
        IPagedListApiClient<ICustomView>,
        IApiPageAccessor<ICustomView>,
        IReadApiClient<ICustomView>,
        IPullApiClient<ICustomView, IPublishableCustomView>,
        IPublishApiClient<IPublishableCustomView, ICustomView>
    {
        /// <summary>
        /// Gets all custom views in the current site.
        /// </summary>
        /// <param name="pageNumber">The 1-indexed page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A list of a page of custom views in the current site.</returns>
        Task<IPagedResult<ICustomView>> GetAllCustomViewsAsync(int pageNumber, int pageSize, CancellationToken cancel);

        /// <summary>
        /// Changes the owner of an existing custom view.
        /// </summary>
        /// <param name="id">The ID for the custom view.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <param name="newOwnerId">The new owner ID.</param>
        /// <param name="newViewName">The new view name.</param>
        /// <returns></returns>
        Task<IResult<ICustomView>> UpdateCustomViewAsync(
            Guid id,
            CancellationToken cancel,
            Guid? newOwnerId = null,
            string? newViewName = null);

        /// <summary>
        /// Deletes the specified custom view.
        /// </summary>
        /// <param name="id">The ID for the custom view.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult> DeleteCustomViewAsync(
            Guid id,
            CancellationToken cancel);

        /// <summary>
        /// Gets the list of user content references whose default view is the specified custom view.
        /// </summary>
        /// <param name="id">The ID for the custom view.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A list of all users whose default view is the specified custom view.</returns>
        Task<IResult<IImmutableList<IContentReference>>> GetAllCustomViewDefaultUsersAsync(
            Guid id,
            CancellationToken cancel);

        /// <summary>
        /// Gets the paged list of user content references whose default view is the specified custom view.
        /// </summary>
        /// <param name="id">The ID for the custom view.</param>
        /// <param name="pageNumber">The 1-indexed page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A list of a page of users whose default view is the specified custom view.</returns>
        Task<IPagedResult<IContentReference>> GetCustomViewDefaultUsersAsync(
            Guid id,
            int pageNumber,
            int pageSize,
            CancellationToken cancel);

        /// <summary>
        /// Sets the specified custom for as the default view for up to 100 specified users. Success or failure for each user is reported in the response body.
        /// </summary>
        /// <param name="id">The ID for the custom view.</param>
        /// <param name="users">The list of users.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult<IImmutableList<ICustomViewAsUserDefaultViewResult>>> SetCustomViewDefaultUsersAsync(
            Guid id,
            IEnumerable<IContentReference> users,
            CancellationToken cancel);

        /// <summary>
        /// Downloads the custom view.
        /// </summary>
        /// <param name="customViewId">The ID for the custom view.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IAsyncDisposableResult<FileDownload>> DownloadCustomViewAsync(
            Guid customViewId,
            CancellationToken cancel);
    }
}