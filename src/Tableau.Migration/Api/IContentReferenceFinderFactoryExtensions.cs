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
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal static class IContentReferenceFinderFactoryExtensions
    {
        #region - Project Find Methods -

        public static async Task<IContentReference?> FindProjectAsync<T>(
            this IContentReferenceFinderFactory finderFactory,
            T response,
            ILogger logger,
            ISharedResourcesLocalizer localizer,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            where T : IWithProjectReferenceType, IRestIdentifiable
            => await finderFactory
                .FindAsync<T, IProject>(
                    response,
                    r =>
                    {
                        var project = Guard.AgainstNull(r.Project, () => nameof(response.Project));
                        return Guard.AgainstDefaultValue(project.Id, () => nameof(response.Project.Id));
                    },
                    logger,
                    localizer,
                    throwIfNotFound,
                    SharedResourceKeys.ProjectReferenceNotFoundMessage,
                    SharedResourceKeys.ProjectReferenceNotFoundException,
                    cancel)
                .ConfigureAwait(false);

        public static async Task<IContentReference?> FindProjectByIdAsync(
            this IContentReferenceFinderFactory finderFactory,
            Guid id,
            ILogger logger,
            ISharedResourcesLocalizer localizer,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            => await finderFactory
                .FindByIdAsync<IProject>(
                    id,
                    logger,
                    localizer,
                    throwIfNotFound,
                    SharedResourceKeys.ProjectReferenceNotFoundException,
                    SharedResourceKeys.ProjectReferenceNotFoundException,
                    cancel)
                .ConfigureAwait(false);

        #endregion - Project Find Methods -

        #region - User Find Methods -

        public static async Task<IContentReference?> FindUserAsync<T>(
            this IContentReferenceFinderFactory finderFactory,
            T response,
            ILogger logger,
            ISharedResourcesLocalizer localizer,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            where T : IRestIdentifiable
            => await finderFactory
                .FindAsync<T, IUser>(
                    response,
                    r =>
                    {
                        return Guard.AgainstDefaultValue(r.Id, () => nameof(response.Id));
                    },
                    logger,
                    localizer,
                    throwIfNotFound,
                    SharedResourceKeys.UserReferenceNotFoundMessage,
                    SharedResourceKeys.UserReferenceNotFoundException,
                    cancel)
                .ConfigureAwait(false);

        #endregion - User Find Methods -

        #region - Owner Find Methods -

        public static async Task<IContentReference?> FindOwnerAsync<T>(
            this IContentReferenceFinderFactory finderFactory,
            T response,
            ILogger logger,
            ISharedResourcesLocalizer localizer,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            where T : IWithOwnerType, IRestIdentifiable
            => await finderFactory
                .FindAsync<T, IUser>(
                    response,
                    r =>
                    {
                        var owner = Guard.AgainstNull(r.Owner, () => nameof(response.Owner));
                        return Guard.AgainstDefaultValue(owner.Id, () => nameof(response.Owner.Id));
                    },
                    logger,
                    localizer,
                    throwIfNotFound,
                    SharedResourceKeys.OwnerNotFoundMessage,
                    SharedResourceKeys.OwnerNotFoundException,
                    cancel)
                .ConfigureAwait(false);

        #endregion - Owner Find Methods -

        #region - Workbook Find Methods -

        public static async Task<IContentReference?> FindWorkbookAsync<T>(
            this IContentReferenceFinderFactory finderFactory,
            T response,
            ILogger logger,
            ISharedResourcesLocalizer localizer,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            where T : IWithWorkbookReferenceType, IRestIdentifiable
            => await finderFactory
                .FindAsync<T, IWorkbook>(
                    response,
                    r =>
                    {
                        var workbook = Guard.AgainstNull(r.Workbook, () => nameof(response.Workbook));
                        return Guard.AgainstDefaultValue(workbook.Id, () => nameof(response.Workbook.Id));
                    },
                    logger,
                    localizer,
                    throwIfNotFound,
                    SharedResourceKeys.WorkbookReferenceNotFoundMessage,
                    SharedResourceKeys.WorkbookReferenceNotFoundException,
                    cancel)
                .ConfigureAwait(false);

        public static async Task<IContentReference?> FindWorkbookByIdAsync(
            this IContentReferenceFinderFactory finderFactory,
            Guid Id,
            ILogger logger,
            ISharedResourcesLocalizer localizer,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            => await finderFactory
                .FindByIdAsync<IWorkbook>(
                    Id,
                    logger,
                    localizer,
                    throwIfNotFound,
                    SharedResourceKeys.WorkbookReferenceNotFoundException,
                    SharedResourceKeys.WorkbookReferenceNotFoundException,
                    cancel)
                .ConfigureAwait(false);

        #endregion - Workbook Find Methods -

        #region - Base Find Methods -

        private static async Task<IContentReference?> FindAsync<TResponse, TContent>(
            this IContentReferenceFinderFactory finderFactory,
            TResponse response,
            Func<TResponse, Guid> getResponseId,
            ILogger logger,
            ISharedResourcesLocalizer localizer,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            string warningMessageResource,
            string exceptionMessageResource,
            CancellationToken cancel)
            where TResponse : IRestIdentifiable
            where TContent : class, IContentReference
        {
            Guard.AgainstNull(response, nameof(response));

            var responseId = getResponseId(response);

            var finder = finderFactory.ForContentType<TContent>();

            var foundContent = await finder.FindByIdAsync(responseId, cancel).ConfigureAwait(false);

            if (foundContent is not null)
                return foundContent;

            var namedResponse = response as INamedContent;

            logger.LogWarning(
                localizer[warningMessageResource],
                responseId,
                response.GetType().Name,
                namedResponse?.Name ?? string.Empty);

            return throwIfNotFound
                ? throw new InvalidOperationException(
                    string.Format(
                        localizer[exceptionMessageResource],
                        responseId))
                : null;
        }

        private static async Task<IContentReference?> FindByIdAsync<TContent>(
            this IContentReferenceFinderFactory finderFactory,
            Guid Id,
            ILogger logger,
            ISharedResourcesLocalizer localizer,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            string warningMessageResource,
            string exceptionMessageResource,
            CancellationToken cancel)
            where TContent : class, IContentReference
        {
            var finder = finderFactory.ForContentType<TContent>();

            var foundContent = await finder.FindByIdAsync(Id, cancel).ConfigureAwait(false);

            if (foundContent is not null)
                return foundContent;

            logger.LogWarning(
                localizer[warningMessageResource],
                Id);

            return throwIfNotFound
                ? throw new InvalidOperationException(
                    string.Format(
                        localizer[exceptionMessageResource],
                        Id))
                : null;
        }

        #endregion - Base Find Methods -
    }
}
