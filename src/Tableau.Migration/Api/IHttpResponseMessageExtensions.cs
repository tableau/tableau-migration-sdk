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
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal static class IHttpResponseMessageExtensions
    {
        public static async Task<IResult> ToResultAsync(this Task<IHttpResponseMessage> getResponseAsync, IHttpContentSerializer serializer, ISharedResourcesLocalizer sharedResourcesLocalizer, CancellationToken cancel)
        {
            try
            {
                var response = await getResponseAsync.ConfigureAwait(false);

                return await response.ToResultAsync(serializer, sharedResourcesLocalizer, cancel).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Result.Failed(ex);
            }
        }

        public static async Task<IResult> ToResultAsync(this IHttpResponseMessage response, IHttpContentSerializer serializer, ISharedResourcesLocalizer sharedResourcesLocalizer, CancellationToken cancel)
        {
            try
            {
                if (response.Content is not null && response.Content.Headers.ContentLength.GetValueOrDefault(0) > 0)
                {
                    // Since we're using an IHttpResponseMessage and not an IHttpResponseMessage<T> here, 
                    // we don't expect any content. But the REST API will return error content if there is an error on the server.
                    // Deserializing it here if it exists so we can include it in the result.
                    var tsError = await serializer.TryDeserializeErrorAsync(response.Content, cancel).ConfigureAwait(false);

                    if (tsError is not null)
                    {
                        throw new RestException(
                            response.RequestMessage?.Method,
                            response.RequestMessage?.RequestUri,
                            tsError,
                            sharedResourcesLocalizer);
                    }
                }

                response.EnsureSuccessStatusCode();

                return Result.Succeeded();
            }
            catch (Exception ex)
            {
                return Result.Failed(ex);
            }
        }

        public static async Task<IResult<TModel>> ToResultAsync<TResponse, TModel>(this Task<IHttpResponseMessage<TResponse>> getResponseAsync, Func<TResponse, TModel> createModel, ISharedResourcesLocalizer sharedResourcesLocalizer)
            where TResponse : TableauServerResponse
            where TModel : class
        {
            try
            {
                var response = await getResponseAsync.ConfigureAwait(false);

                return response.ToResult(createModel, sharedResourcesLocalizer);
            }
            catch (Exception ex)
            {
                return Result<TModel>.Failed(ex);
            }
        }

        public static async Task<IResult<TModel>> ToResultAsync<TResponse, TModel>(this Task<IHttpResponseMessage<TResponse>> getResponseAsync, Func<TResponse, CancellationToken, Task<TModel>> createModelAsync, ISharedResourcesLocalizer sharedResourcesLocalizer, CancellationToken cancel)
            where TResponse : TableauServerResponse
            where TModel : class
        {
            try
            {
                var response = await getResponseAsync.ConfigureAwait(false);

                return await response.ToResultAsync(createModelAsync, sharedResourcesLocalizer, cancel).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Result<TModel>.Failed(ex);
            }
        }

        public static IResult<TModel> ToResult<TResponse, TModel>(this IHttpResponseMessage<TResponse> response, Func<TResponse, TModel> createModel, ISharedResourcesLocalizer sharedResourcesLocalizer)
            where TResponse : TableauServerResponse
            where TModel : class
        {
            try
            {
                var restError = response.DeserializedContent?.Error;

                if (restError is not null)
                {
                    throw new RestException(
                        response.RequestMessage?.Method,
                        response.RequestMessage?.RequestUri,
                        restError,
                        sharedResourcesLocalizer);
                }

                response.EnsureSuccessStatusCode();

                var content = Guard.AgainstNull(response.DeserializedContent, () => response.DeserializedContent);

                var model = createModel(content);

                return Result<TModel>.Succeeded(model);
            }
            catch (Exception ex)
            {
                return Result<TModel>.Failed(ex);
            }
        }

        public static async Task<IResult<TModel>> ToResultAsync<TResponse, TModel>(this IHttpResponseMessage<TResponse> response, Func<TResponse, CancellationToken, Task<TModel>> createModelAsync, ISharedResourcesLocalizer sharedResourcesLocalizer, CancellationToken cancel)
            where TResponse : TableauServerResponse
            where TModel : class
        {
            try
            {
                var restError = response.DeserializedContent?.Error;

                if (restError is not null)
                {
                    throw new RestException(
                        response.RequestMessage?.Method,
                        response.RequestMessage?.RequestUri,
                        restError,
                        sharedResourcesLocalizer);
                }

                response.EnsureSuccessStatusCode();

                var content = Guard.AgainstNull(response.DeserializedContent, () => response.DeserializedContent);

                var model = await createModelAsync(content, cancel).ConfigureAwait(false);

                return Result<TModel>.Succeeded(model);
            }
            catch (Exception ex)
            {
                return Result<TModel>.Failed(ex);
            }
        }

        public static async Task<IPagedResult<TModel>> ToPagedResultAsync<TResponse, TModel>(this Task<IHttpResponseMessage<TResponse>> getResponseAsync, Func<TResponse, IImmutableList<TModel>> createModel, ISharedResourcesLocalizer sharedResourcesLocalizer)
            where TResponse : TableauServerResponse, IPageInfo
            where TModel : class
        {
            try
            {
                var response = await getResponseAsync.ConfigureAwait(false);

                return response.ToPagedResult(createModel, sharedResourcesLocalizer);
            }
            catch (Exception ex)
            {
                return PagedResult<TModel>.Failed(ex);
            }
        }

        public static IPagedResult<TModel> ToPagedResult<TResponse, TModel>(this IHttpResponseMessage<TResponse> response, Func<TResponse, IImmutableList<TModel>> createModel, ISharedResourcesLocalizer sharedResourcesLocalizer)
            where TResponse : TableauServerResponse, IPageInfo
            where TModel : class
        {
            try
            {
                var restError = response.DeserializedContent?.Error;

                if (restError is not null)
                {
                    throw new RestException(
                        response.RequestMessage?.Method,
                        response.RequestMessage?.RequestUri,
                        restError,
                        sharedResourcesLocalizer);
                }

                response.EnsureSuccessStatusCode();

                var content = Guard.AgainstNull(response.DeserializedContent, () => response.DeserializedContent);

                var model = createModel(content);

                return PagedResult<TModel>.Succeeded(model, content.PageNumber, content.PageSize, content.TotalCount, content.FetchedAllPages);
            }
            catch (Exception ex)
            {
                return PagedResult<TModel>.Failed(ex);
            }
        }

        public static async Task<IPagedResult<TModel>> ToPagedResultAsync<TResponse, TModel>(this Task<IHttpResponseMessage<TResponse>> getResponseAsync, Func<TResponse, CancellationToken, Task<IImmutableList<TModel>>> createModelAsync, ISharedResourcesLocalizer sharedResourcesLocalizer, CancellationToken cancel)
            where TResponse : TableauServerResponse, IPageInfo
            where TModel : class
        {
            try
            {
                var response = await getResponseAsync.ConfigureAwait(false);

                return await response.ToPagedResultAsync(createModelAsync, sharedResourcesLocalizer, cancel).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return PagedResult<TModel>.Failed(ex);
            }
        }

        public static async Task<IPagedResult<TModel>> ToPagedResultAsync<TResponse, TModel>(this IHttpResponseMessage<TResponse> response,
             Func<TResponse, CancellationToken, Task<IImmutableList<TModel>>> createModelAsync,
             ISharedResourcesLocalizer sharedResourcesLocalizer, CancellationToken cancel)
             where TResponse : TableauServerResponse, IPageInfo
             where TModel : class
        {
            try
            {
                var restError = response.DeserializedContent?.Error;

                if (restError is not null)
                {
                    throw new RestException(
                        response.RequestMessage?.Method,
                        response.RequestMessage?.RequestUri,
                        restError,
                        sharedResourcesLocalizer);
                }

                response.EnsureSuccessStatusCode();

                var content = Guard.AgainstNull(response.DeserializedContent, () => response.DeserializedContent);

                var model = await createModelAsync(content, cancel).ConfigureAwait(false);

                return PagedResult<TModel>.Succeeded(model, content.PageNumber, content.PageSize, content.TotalCount, content.FetchedAllPages);
            }
            catch (Exception ex)
            {
                return PagedResult<TModel>.Failed(ex);
            }
        }

        #region - File Downloads -

        internal static string? GetContentDispositionFilename(this IHttpResponseMessage response)
        {
            if (!response.Content.Headers.TryGetValues(RestHeaders.ContentDisposition, out var contentDispositions))
            {
                return null;
            }

            foreach (var contentDisposition in contentDispositions)
            {
                if (string.IsNullOrEmpty(contentDisposition))
                    continue;

                var parts = contentDisposition.Split(';', StringSplitOptions.RemoveEmptyEntries);
                if (parts.IsNullOrEmpty())
                    continue;

                foreach (var part in parts)
                {
                    if (string.IsNullOrEmpty(part))
                        continue;

                    var kvp = part.Trim().Split("=", StringSplitOptions.RemoveEmptyEntries);
                    if (kvp.IsNullOrEmpty() || kvp.Length != 2)
                        continue;

                    if (kvp[0].Equals("filename", StringComparison.OrdinalIgnoreCase))
                        return kvp[1].Trim('"');
                }
            }

            return null;
        }

        public static async Task<IAsyncDisposableResult<FileDownload>> DownloadResultAsync(this Task<IHttpResponseMessage> getResponseAsync, CancellationToken cancel)
        {
            try
            {
                var response = await getResponseAsync.ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var filename = response.GetContentDispositionFilename();

                var content = await response.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);

                var download = new FileDownload(filename, content);
                return AsyncDisposableResult<FileDownload>.Succeeded(download);
            }
            catch (Exception ex)
            {
                return AsyncDisposableResult<FileDownload>.Failed(ex);
            }
        }

        #endregion
    }
}
