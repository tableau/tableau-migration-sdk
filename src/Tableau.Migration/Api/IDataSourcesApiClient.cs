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
using Tableau.Migration.Api.Labels;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Content;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client data source operations.
    /// </summary>
    public interface IDataSourcesApiClient
        : IPagedListApiClient<IDataSource>,
        IPublishApiClient<IPublishableDataSource, IDataSourceDetails>,
        IPullApiClient<IDataSource, IPublishableDataSource>,
        IOwnershipApiClient,
        ITagsContentApiClient,
        IApiPageAccessor<IDataSource>,
        IPermissionsContentApiClient,
        IConnectionsApiClient,
        ILabelsContentApiClient<IDataSource>
    {
        /// <summary>
        /// Gets all published data sources in the current site.
        /// </summary>
        /// <param name="pageNumber">The 1-indexed page number.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>A list of a page of data sources in the current site.</returns>
        Task<IPagedResult<IDataSource>> GetAllPublishedDataSourcesAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancel);

        /// <summary>
        /// Gets a data source by the given ID.
        /// </summary>
        /// <param name="dataSourceId">The ID to get the data source for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The data source result.</returns>
        Task<IResult<IDataSourceDetails>> GetDataSourceAsync(
             Guid dataSourceId,
             CancellationToken cancel);

        /// <summary>
        /// Downloads the data source file for the given ID.
        /// </summary>
        /// <param name="dataSourceId">The ID to download the data source file for.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The file download result.</returns>
        Task<IAsyncDisposableResult<FileDownload>> DownloadDataSourceAsync(
            Guid dataSourceId,
            CancellationToken cancel);

        /// <summary>
        /// Uploads the input data source file.
        /// </summary>
        /// <param name="options">The new data source's details.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The published data source.</returns>
        Task<IResult<IDataSourceDetails>> PublishDataSourceAsync(
            IPublishDataSourceOptions options,
            CancellationToken cancel);

        /// <summary>
        /// Updates the data source after publishing.
        /// </summary>
        /// <param name="dataSourceId">The ID for the data source to update.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <param name="newName">The new name of a the data source, or null to not update the name.</param>
        /// <param name="newProjectId">The LUID of a project to move the data source to, or null to not update the project.</param>
        /// <param name="newOwnerId">The LUID of a user to assign the data source to as owner, or null to not update the owner.</param>
        /// <param name="newIsCertified">Whether or not the data source is certified, or null to not update the flag.</param>
        /// <param name="newCertificationNote">The certification note, or null to not update the note.</param>
        /// <param name="newEncryptExtracts">Whether or not to encrypt extracts, or null to not update the option.</param>
        /// <returns>The update result.</returns>
        Task<IResult<IUpdateDataSourceResult>> UpdateDataSourceAsync(
            Guid dataSourceId,
            CancellationToken cancel,
            string? newName = null,
            Guid? newProjectId = null,
            Guid? newOwnerId = null,
            bool? newIsCertified = null,
            string? newCertificationNote = null,
            bool? newEncryptExtracts = null);
    }
}
