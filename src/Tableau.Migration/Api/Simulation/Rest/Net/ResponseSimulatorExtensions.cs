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
using System.Net.Http;
using System.Text.RegularExpressions;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net.Simulation;
using Tableau.Migration.Net.Simulation.Responses;

namespace Tableau.Migration.Api.Simulation.Rest.Net
{
    internal static class ResponseSimulatorExtensions
    {
        #region - General -

        public static MethodSimulator SetupRestMethod(
            this TableauApiResponseSimulator simulator,
            HttpMethod httpMethod,
            Regex urlPattern,
            IResponseBuilder responseBuilder,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null)
        {
            var requestMatcher = new RestApiRequestMatcher(simulator.BaseUrl, httpMethod, urlPattern,
                queryStringPatterns?.ToImmutableDictionary(i => i.Key, i => i.ValuePattern) ?? ImmutableDictionary<string, Regex>.Empty);

            var method = new MethodSimulator(requestMatcher, responseBuilder);
            simulator.SetupMethod(method);
            return method;
        }

        public static MethodSimulator SetupRestDelete(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            IResponseBuilder responseBuilder,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null)
                => simulator.SetupRestMethod(HttpMethod.Delete, urlPattern, responseBuilder, queryStringPatterns);

        public static MethodSimulator SetupRestGet(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            IResponseBuilder responseBuilder,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null)
                => simulator.SetupRestMethod(HttpMethod.Get, urlPattern, responseBuilder, queryStringPatterns);

        public static MethodSimulator SetupRestPost(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            IResponseBuilder responseBuilder,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null)
                => simulator.SetupRestMethod(HttpMethod.Post, urlPattern, responseBuilder, queryStringPatterns);

        public static MethodSimulator SetupRestPut(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            IResponseBuilder responseBuilder,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null)
                => simulator.SetupRestMethod(HttpMethod.Put, urlPattern, responseBuilder, queryStringPatterns);

        #endregion

        #region - CRUD -

        public static MethodSimulator SetupRestGet<TResponse, TResponseItem>(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            Func<TableauData, TResponseItem?> getEntity,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null,
            bool requiresAuthentication = true)
                where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
                    => simulator.SetupRestGet<TResponse, TResponseItem>(
                        urlPattern,
                        (d, _) => getEntity(d),
                        queryStringPatterns,
                        requiresAuthentication);

        public static MethodSimulator SetupRestGet<TResponse, TResponseItem>(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            Func<TableauData, HttpRequestMessage, TResponseItem?> getEntity,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null,
            bool requiresAuthentication = true)
                where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
                    => simulator.SetupRestGet(
                        urlPattern,
                        new RestSingleEntityResponseBuilder<TResponse, TResponseItem>(simulator.Data, simulator.Serializer, getEntity, requiresAuthentication),
                        queryStringPatterns
            );

        public static MethodSimulator SetupRestPost<TResponse, TResponseItem>(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            Func<TableauData, TResponseItem?> getEntity,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null,
            bool requiresAuthentication = true)
                where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
                    => simulator.SetupRestPost<TResponse, TResponseItem>(
                        urlPattern,
                        (d, _) => getEntity(d),
                        queryStringPatterns,
                        requiresAuthentication);

        public static MethodSimulator SetupRestPost<TResponse, TResponseItem>(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            Func<TableauData, HttpRequestMessage, TResponseItem?> getEntity,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null,
            bool requiresAuthentication = true)
                where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
                    => simulator.SetupRestPost(
                        urlPattern,
                        new RestSingleEntityResponseBuilder<TResponse, TResponseItem>(simulator.Data, simulator.Serializer, getEntity, requiresAuthentication),
                        queryStringPatterns
            );

        public static MethodSimulator SetupRestPost<TResponse, TResponseItem>(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            IResponseBuilder responseBuilder,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null)
                where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
                    => simulator.SetupRestPost(urlPattern, responseBuilder, queryStringPatterns);

        public static MethodSimulator SetupRestPut<TResponse, TResponseItem>(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            Func<TableauData, HttpRequestMessage, TResponseItem?> getEntity,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null,
            bool requiresAuthentication = true)
                where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
                    => simulator.SetupRestPut(
                        urlPattern,
                        new RestSingleEntityResponseBuilder<TResponse, TResponseItem>(simulator.Data, simulator.Serializer, getEntity, requiresAuthentication),
                        queryStringPatterns
            );

        public static MethodSimulator SetupRestPutList<TResponse, TResponseItem>(
           this TableauApiResponseSimulator simulator,
           Regex urlPattern,
           Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> getEntity,
           IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null,
           bool requiresAuthentication = true)
               where TResponse : TableauServerListResponse<TResponseItem>, new()
                    => simulator.SetupRestPut(
                        urlPattern,
                        new RestListResponseBuilder<TResponse, TResponseItem>(simulator.Data, simulator.Serializer, getEntity, requiresAuthentication),
                        queryStringPatterns
            );

        public static MethodSimulator SetupRestGetById<TResponse, TResponseItem>(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            Func<TableauData, ICollection<TResponseItem>> getData,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null,
            bool requiresAuthentication = true)
                where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
                where TResponseItem : IRestIdentifiable
                    => simulator.SetupRestGetById<TResponse, TResponseItem>(
                        urlPattern,
                        (d, _) => getData(d),
                        queryStringPatterns,
                        requiresAuthentication);

        public static MethodSimulator SetupRestGetById<TResponse, TResponseItem>(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> getData,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null,
            bool requiresAuthentication = true)
                where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
                where TResponseItem : IRestIdentifiable
                    => simulator.SetupRestGet(
                        urlPattern,
                        new RestGetByIdResponseBuilder<TResponse, TResponseItem>(simulator.Data, simulator.Serializer, getData, requiresAuthentication),
                        queryStringPatterns
            );

        public static MethodSimulator SetupRestGetByContentUrl<TResponse, TResponseItem>(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            Func<TableauData, ICollection<TResponseItem>> getData,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null,
            bool requiresAuthentication = true)
                where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
                where TResponseItem : IApiContentUrl
                    => simulator.SetupRestGetByContentUrl<TResponse, TResponseItem>(
                        urlPattern,
                        (d, _) => getData(d),
                        queryStringPatterns,
                        requiresAuthentication);

        public static MethodSimulator SetupRestGetByContentUrl<TResponse, TResponseItem>(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> getData,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null,
            bool requiresAuthentication = true)
                where TResponse : TableauServerResponse, ITableauServerResponse<TResponseItem>, new()
                where TResponseItem : IApiContentUrl
                    => simulator.SetupRestGet(
                        urlPattern,
                        new RestGetByContentUrlResponseBuilder<TResponse, TResponseItem>(simulator.Data, simulator.Serializer, getData, requiresAuthentication),
                        queryStringPatterns);

        public static MethodSimulator SetupRestPagedList<TResponse, TResponseItem>(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            Func<TableauData, ICollection<TResponseItem>> getData,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null,
            bool requiresAuthentication = true)
                where TResponse : PagedTableauServerResponse<TResponseItem>, new()
                    => simulator.SetupRestPagedList<TResponse, TResponseItem>(
                        urlPattern,
                        (d, _) => getData(d),
                        queryStringPatterns,
                        requiresAuthentication);

        public static MethodSimulator SetupRestPagedList<TResponse, TResponseItem>(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> getData,
            IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null,
            bool requiresAuthentication = true)
                where TResponse : PagedTableauServerResponse<TResponseItem>, new()
                    => simulator.SetupRestGet(
                        urlPattern,
                        new RestPagedListResponseBuilder<TResponse, TResponseItem>(simulator.Data, simulator.Serializer, getData, requiresAuthentication),
                        queryStringPatterns
            );

        public static MethodSimulator SetupRestGetList<TResponse, TResponseItem>(
           this TableauApiResponseSimulator simulator,
           Regex urlPattern,
           Func<TableauData, HttpRequestMessage, ICollection<TResponseItem>> getData,
           IEnumerable<(string Key, Regex ValuePattern)>? queryStringPatterns = null,
           bool requiresAuthentication = true)
               where TResponse : TableauServerListResponse<TResponseItem>, new()
                   => simulator.SetupRestGet(
                       urlPattern,
                       new RestListResponseBuilder<TResponse, TResponseItem>(simulator.Data, simulator.Serializer, getData, requiresAuthentication),
                       queryStringPatterns
           );

        #endregion

        #region - Download -

        public static MethodSimulator SetupRestDownloadById(
            this TableauApiResponseSimulator simulator,
            Regex urlPattern,
            Func<TableauData, IReadOnlyDictionary<Guid, byte[]>> getFilesById,
            int idNotFoundSubCode,
            bool requiresAuthentication = true)
                => simulator.SetupRestGet(
                    urlPattern,
                    new RestDownloadFileByIdResponseBuilder(simulator.Data, simulator.Serializer, getFilesById, idNotFoundSubCode, requiresAuthentication)
            );

        #endregion
    }
}