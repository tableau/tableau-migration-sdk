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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class UsersApiClient : ContentApiClientBase, IUsersApiClient
    {
        private readonly IJobsApiClient _jobs;
        private readonly IHttpContentSerializer _serializer;

        public UsersApiClient(
            IJobsApiClient jobs,
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            ILoggerFactory loggerFactory,
            IHttpContentSerializer serializer,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            _jobs = jobs;
            _serializer = serializer;
        }

        /// <inheritdoc />
        public async Task<IPagedResult<IGroup>> GetUserGroupsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancel)
        {
            var getUserGroupsResult = await RestRequestBuilderFactory
                .CreateUri($"/users/{userId.ToUrlSegment()}/groups")
                .WithPage(pageNumber, pageSize)
                .ForGetRequest()
                .SendAsync<GroupsResponse>(cancel)
                .ToPagedResultAsync(r => r.GetGroupsFromResponse(), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return getUserGroupsResult;
        }

        /// <inheritdoc />
        public async Task<IPagedResult<IUser>> GetAllUsersAsync(int pageNumber, int pageSize, CancellationToken cancel)
        {
            var getAllUsersResult = await RestRequestBuilderFactory
                .CreateUri("/users")
                .WithPage(pageNumber, pageSize)
                .ForGetRequest()
                .SendAsync<UsersResponse>(cancel)
                .ToPagedResultAsync(r => r.GetUsersFromResponse(), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return getAllUsersResult;
        }

        /// <inheritdoc />
        public async Task<IResult<IImportJob>> ImportUsersAsync(IEnumerable<IUser> users, Stream csvStream, CancellationToken cancel)
        {
            try
            {
                // Create the XML request, using the most common authentication type as the request-default.
                ImportUsersFromCsvRequest xmlRequest;

                var authTypes = users
                    .Select(u => u.AuthenticationType)
                    .Distinct()
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .ToImmutableArray();

                if (authTypes.Length < 2)
                {
                    var authType = authTypes.FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(authType))
                    {
                        xmlRequest = new ImportUsersFromCsvRequest();
                    }
                    else
                    {
                        xmlRequest = new ImportUsersFromCsvRequest(authType);
                    }
                }
                else
                {
                    var requestUsers = users
                        .Where(u => !string.IsNullOrWhiteSpace(u.AuthenticationType))
                        .Select(u =>
                        new ImportUsersFromCsvRequest.UserType
                        {
                            Name = u.Name,
                            AuthSetting = u.AuthenticationType
                        });
                    xmlRequest = new ImportUsersFromCsvRequest(requestUsers);
                }

                var payloadContent = new StringContent(xmlRequest.ToXml(), Constants.DefaultEncoding, MediaTypes.Xml.MediaType!);

                // Create the multipart content.
                var csvDataStreamContent = new StreamContent(csvStream);
                var content = new MultipartFormDataContent
                    {
                        { csvDataStreamContent, "tableau_user_import", $"{Guid.NewGuid()}.csv" },
                        { payloadContent, "request_payload" }
                    };

                var jobResult = await RestRequestBuilderFactory
                    .CreateUri("/users/import")
                    .ForPostRequest()
                    .WithContent(content)
                    .AcceptXml(false)
                    .SendAsync<ImportJobResponse>(cancel)
                    .ToResultAsync<ImportJobResponse, IImportJob>(r => new ImportJob(r), SharedResourcesLocalizer)
                    .ConfigureAwait(false);

                return jobResult;

            }
            catch (Exception ex)
            {
                return Result<IImportJob>.Failed(ex);
            }
        }

        /// <inheritdoc />
        public async Task<IResult<IAddUserResult>> AddUserAsync(string userName, string siteRole, string? authenticationType, CancellationToken cancel)
        {
            var userResult = await RestRequestBuilderFactory
                .CreateUri("/users")
                .ForPostRequest()
                .WithXmlContent(new AddUserToSiteRequest(userName, siteRole, authenticationType))
                .SendAsync<AddUserResponse>(cancel)
                .ToResultAsync<AddUserResponse, IAddUserResult>(r => new AddUserResult(r), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return userResult;
        }

        /// <inheritdoc />
        public async Task<IResult<IUpdateUserResult>> UpdateUserAsync(Guid id,
                                                          string newSiteRole,
                                                          CancellationToken cancel,
                                                          string? newfullName = null,
                                                          string? newEmail = null,
                                                          string? newPassword = null,
                                                          string? newAuthSetting = null)
        {
            var userResult = await RestRequestBuilderFactory
                .CreateUri($"/users/{id}")
                .ForPutRequest()
                .WithXmlContent(new UpdateUserRequest(newSiteRole, newfullName, newEmail, newPassword, newAuthSetting))
                .SendAsync<UpdateUserResponse>(cancel)
                .ToResultAsync<UpdateUserResponse, IUpdateUserResult>(r => new UpdateUserResult(r), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return userResult;
        }

        /// <inheritdoc/>
        public async Task<IResult> DeleteUserAsync(Guid userId, CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"/users/{userId.ToUrlSegment()}")
                .ForDeleteRequest()
                .SendAsync(cancel)
                .ToResultAsync(_serializer, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        #region - IPagedListApiClient<IUser> Implementation -

        /// <inheritdoc />
        public IPager<IUser> GetPager(int pageSize) => new ApiListPager<IUser>(this, pageSize);

        #endregion

        #region - IBatchPublishApiClient<IUser> Implementation -

        /// <inheritdoc />
        public async Task<IResult> PublishBatchAsync(IEnumerable<IUser> items, CancellationToken cancel)
        {
            // Create user CSV
            using var csvStream = UsersApiClient.GenerateUserCsvStream(items);

            cancel.ThrowIfCancellationRequested();

            // Start asynchronous import job.
            var jobCreateResult = await ImportUsersAsync(items, csvStream, cancel).ConfigureAwait(false);
            if (!jobCreateResult.Success)
            {
                return jobCreateResult;
            }

            cancel.ThrowIfCancellationRequested();

            // Wait for import job to finish.
            return await _jobs.WaitForJobAsync(jobCreateResult.Value.Id, cancel).ConfigureAwait(false);
        }

        internal static Stream GenerateUserCsvStream(IEnumerable<IUser> items)
        {
            var csv = new StringBuilder();
            foreach (var item in items)
            {
                item.AppendCsvLine(csv);
            }

            var csvStream = new MemoryStream(Constants.DefaultEncoding.GetBytes(csv.ToString()));

            csvStream.Seek(0, SeekOrigin.Begin);

            return csvStream;
        }

        #endregion

        #region - IApiPageAccessor<IUser> Implementation -

        /// <inheritdoc />
        public async Task<IPagedResult<IUser>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await GetAllUsersAsync(pageNumber, pageSize, cancel).ConfigureAwait(false);

        #endregion

        #region - IReadApiClient<IUser> Implementation -

        /// <inheritdoc />
        public async Task<IResult<IUser>> GetByIdAsync(Guid contentId, CancellationToken cancel)
        {
            var getUserResult = await RestRequestBuilderFactory
               .CreateUri($"/users/{contentId.ToUrlSegment()}")
               .ForGetRequest()
               .SendAsync<UserResponse>(cancel)
               .ToResultAsync<UserResponse, IUser>(r => new User(r.Item!), SharedResourcesLocalizer)
               .ConfigureAwait(false);

            return getUserResult;
        }

        #endregion

        #region - IPublishApiClient<IUser> Implementation -

        public async Task<IResult<IUser>> PublishAsync(IUser item, CancellationToken cancel)
        {
            var result = await AddUserAsync(
                item.Name,
                item.SiteRole,
                item.AuthenticationType,
                cancel)
                .ConfigureAwait(false);

            if (!result.Success)
            {
                return Result<IUser>.Failed(result.Errors);
            }

            return Result<IUser>.Succeeded(
                new User(
                    result.Value.Id,
                    null,
                    null,
                    result.Value.Name,
                    null,
                    result.Value.SiteRole,
                    result.Value.AuthSetting));
        }

        #endregion
    }
}
