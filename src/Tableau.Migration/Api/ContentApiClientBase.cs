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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal abstract class ContentApiClientBase : ApiClientBase, IContentApiClient
    {
        protected readonly IContentReferenceFinderFactory ContentFinderFactory;

        public string UrlPrefix { get; }

        public ContentApiClientBase(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            string? urlPrefix = null)
            : base(restRequestBuilderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            UrlPrefix = urlPrefix ?? RestUrlPrefixes.GetUrlPrefix(GetType());

            ContentFinderFactory = finderFactory;
        }

        #region - Find Project Methods -

        protected async Task<IContentReference?> FindProjectAsync<T>(
            T response,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            where T : IWithProjectReferenceType, IRestIdentifiable
            => await ContentFinderFactory
                .FindProjectAsync(
                    response,
                    Logger,
                    SharedResourcesLocalizer,
                    throwIfNotFound,
                    cancel)
                .ConfigureAwait(false);

        protected async Task<IContentReference?> FindProjectByIdAsync(
            Guid id,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            => await ContentFinderFactory
                .FindProjectByIdAsync(
                    id,
                    Logger,
                    SharedResourcesLocalizer,
                    throwIfNotFound,
                    cancel)
                .ConfigureAwait(false);

        #endregion - Find Project Methods -

        #region - Find User Methods -

        protected async Task<IContentReference?> FindUserAsync<T>(
            T response,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            where T : IRestIdentifiable
            => await ContentFinderFactory
                .FindUserAsync(
                    response,
                    Logger,
                    SharedResourcesLocalizer,
                    throwIfNotFound,
                    cancel)
                .ConfigureAwait(false);

        #endregion

        #region - Find Owner Methods -

        protected async Task<IContentReference?> FindOwnerAsync<T>(
            T response,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            where T : IWithOwnerType, IRestIdentifiable
            => await ContentFinderFactory
                .FindOwnerAsync(
                    response,
                    Logger,
                    SharedResourcesLocalizer,
                    throwIfNotFound,
                    cancel)
                .ConfigureAwait(false);

        #endregion - Find Owner Methods -

        #region - Find Workbook Methods -

        protected async Task<IContentReference?> FindWorkbookAsync<T>(
            T response,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            where T : IWithWorkbookReferenceType, IRestIdentifiable
            => await ContentFinderFactory
                .FindWorkbookAsync(
                    response,
                    Logger,
                    SharedResourcesLocalizer,
                    throwIfNotFound,
                    cancel)
                .ConfigureAwait(false);

        protected async Task<IContentReference?> FindWorkbookByIdAsync(
            Guid id,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            => await ContentFinderFactory
                .FindWorkbookByIdAsync(
                    id,
                    Logger,
                    SharedResourcesLocalizer,
                    throwIfNotFound,
                    cancel)
                .ConfigureAwait(false);

        #endregion - Find Workbook Methods -
    }
}
