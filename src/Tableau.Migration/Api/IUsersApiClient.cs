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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client user operations.
    /// </summary>
    public interface IUsersApiClient : IContentApiClient, IPagedListApiClient<IUser>, IBatchPublishApiClient<IUser>, IApiPageAccessor<IUser>, IReadApiClient<IUser>, IPublishApiClient<IUser>
    {
        /// <summary>
        /// Gets the groups belonging to a user.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="pageNumber">The 1-indexed page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A list of groups for the given user ID</returns>
        Task<IPagedResult<IGroup>> GetUserGroupsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancel);

        /// <summary>
        /// Gets all users in the current site.
        /// </summary>
        /// <param name="pageNumber">The 1-indexed page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A list of a page of users in the current site.</returns>
        Task<IPagedResult<IUser>> GetAllUsersAsync(int pageNumber, int pageSize, CancellationToken cancel);

        /// <summary>
        /// Imports users into the current site from a CSV file.
        /// </summary>
        /// <param name="users">The users to include in the request payload.</param>
        /// <param name="csvStream">The in-memory stream containing user data in a csv format.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The newly created import job.</returns>
        Task<IResult<IImportJob>> ImportUsersAsync(IEnumerable<IUser> users, Stream csvStream, CancellationToken cancel);

        /// <summary>
        /// Adds a user.
        /// </summary>
        /// <param name="userName">The username. In case of Tableau Cloud, the user name is the email address of the user.</param>
        /// <param name="siteRole">The site role for the user.</param>
        /// <param name="authenticationType">The optional authentication type of the user.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult<IAddUserResult>> AddUserAsync(string userName, string siteRole, string? authenticationType, CancellationToken cancel);

        /// <summary>
        /// Updates the user already present at the destination.
        /// </summary>
        /// <param name="id">The Identifier for the user.</param>
        /// <param name="newSiteRole">The new Site Role for the user.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <param name="newfullName">(Optional) The new Full Name for the user.</param>
        /// <param name="newEmail">(Optional) The new email address for the user.</param>
        /// <param name="newPassword">(Optional) The new password for the user.</param>
        /// <param name="newAuthSetting">(Optional) The new email Auth Setting for the user.</param>

        /// <returns></returns>
        Task<IResult<IUpdateUserResult>> UpdateUserAsync(Guid id,
                                             string newSiteRole,
                                             CancellationToken cancel,
                                             string? newfullName = null,
                                             string? newEmail = null,
                                             string? newPassword = null,
                                             string? newAuthSetting = null);

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns></returns>
        Task<IResult> DeleteUserAsync(Guid userId, CancellationToken cancel);
    }
}
