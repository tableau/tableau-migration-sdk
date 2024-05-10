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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal abstract class RestCommitUploadResponseBuilder<TResponse, TItem, TCommitRequest> : RestApiResponseBuilderBase<TResponse>
        where TResponse : TableauServerResponse<TItem>, new()
        where TItem : class, IRestIdentifiable
        where TCommitRequest : TableauServerRequest
    {
        private readonly Func<TableauData, ICollection<TItem>> _getContent;
        private readonly Func<TableauData, ConcurrentDictionary<Guid, byte[]>> _getFiles;
        private readonly Action<TableauData, TItem, byte[]> _addItem;

        public RestCommitUploadResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, ICollection<TItem>> getContent,
            Func<TableauData, ConcurrentDictionary<Guid, byte[]>> getFiles,
            Action<TableauData, TItem, byte[]> addItem)
            : base(data, serializer, true)
        {
            _getContent = getContent;
            _getFiles = getFiles;
            _addItem = addItem;
        }

        protected override async ValueTask<(TResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(
            HttpRequestMessage request, CancellationToken cancel)
        {
            var sessionId = request.GetQueryStringValue("uploadSessionId");

            var file = Data.Files.First(ds => ds.Key == sessionId).Value.ToArray();

            if (file is null)
            {
                return BuildEmptyErrorResponse(HttpStatusCode.BadRequest, 0, "No file found to commit.", "");
            }

            var commitRequest = await GetCommitRequestAsync(request).ConfigureAwait(false);
            if (commitRequest is null)
            {
                return BuildEmptyErrorResponse(HttpStatusCode.BadRequest, 0, "Invalid commit request.", "");
            }

            var overwrite = false;

            var overwriteQueryVal = request.GetQueryStringValue("overwrite");
            if (!string.IsNullOrEmpty(overwriteQueryVal) && !bool.TryParse(overwriteQueryVal, out overwrite))
            {
                return BuildEmptyErrorResponse(HttpStatusCode.BadRequest, 8, "Bad Request", $"Invalid boolean value '{overwriteQueryVal}' for parameter 'overwrite'.");
            }

            var currentUser = EnsureSignedInUser();

            var existingContent = GetExistingContentItem(commitRequest);

            // Check if a workbook already exists, but override is not set
            if (!overwrite && existingContent is not null)
            {
                return BuildEmptyErrorResponse(HttpStatusCode.Forbidden, 7, "Overwrite forbidden", "An matching item already exists and the overwrite parameter was not set to true.");
            }

            TItem? responseContent;
            try
            {
                responseContent = BuildContent(commitRequest, ref file, existingContent, currentUser, overwrite);
            }
            catch (BuildResponseException ex)
            {
                return BuildEmptyErrorResponse(ex.StatusCode, ex.SubCode, ex.Summary, ex.Detail);
            }

            // Override existing content
            if (overwrite && existingContent is not null)
            {
                _getContent(Data).Remove(existingContent);
                _getFiles(Data).TryRemove(existingContent.Id, out _);
            }

            _addItem(Data, responseContent, file);

            Data.Files.TryRemove(sessionId, out _);

            return (new TResponse
            {
                Item = BuildResponse(responseContent)
            },
            HttpStatusCode.Created);
        }

        protected abstract TItem? GetExistingContentItem(TCommitRequest commitRequest);

        protected static async Task<TCommitRequest?> GetCommitRequestAsync(HttpRequestMessage request)
        {
            var multiContent = request.Content as MultipartContent;

            var data = multiContent?.FirstOrDefault() ?? throw new Exception("Unable to access payload content.");

            var result = await data.ReadAsStringAsync().ConfigureAwait(false);
            var commitRequest = result.FromXml<TCommitRequest>();

            return commitRequest;
        }

        /// <summary>
        /// Builds the content that will be used to build the response
        /// </summary>
        /// <param name="commitRequest">The Commit Request.</param>
        /// <param name="commitFileData">The Commit Request File. This is a ref in case the file data needs to be updated by the commit request.</param>
        /// <param name="existingContent">Existing content that was found earlier.</param>
        /// <param name="currentUser">The currently logged in user.</param>
        /// <param name="overwrite">Flat to determine if existing content should be overridden</param>
        protected abstract TItem BuildContent(TCommitRequest commitRequest, ref byte[] commitFileData, TItem? existingContent, UsersResponse.UserType currentUser, bool overwrite);

        protected abstract TItem BuildResponse(TItem item);
    }
}
