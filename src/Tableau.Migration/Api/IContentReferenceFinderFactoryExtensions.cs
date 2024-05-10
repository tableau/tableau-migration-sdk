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
        public static async Task<IContentReference?> FindProjectAsync<T>(
            this IContentReferenceFinderFactory finderFactory,
            [NotNull] T? response,
            ILogger logger,
            ISharedResourcesLocalizer localizer,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            where T : IWithProjectType, INamedContent
        {
            Guard.AgainstNull(response, nameof(response));

            var project = Guard.AgainstNull(response.Project, () => nameof(response.Project));
            var projectId = Guard.AgainstDefaultValue(project.Id, () => nameof(response.Project.Id));

            var projectFinder = finderFactory.ForContentType<IProject>();

            var foundProject = await projectFinder.FindByIdAsync(projectId, cancel).ConfigureAwait(false);

            if (foundProject is not null)
                return foundProject;

            logger.LogWarning(localizer[SharedResourceKeys.ProjectReferenceNotFoundMessage], response.Project.Name, response.GetType().Name, response.Name);

            return throwIfNotFound 
                ? throw new InvalidOperationException($"The project with ID {projectId} was not found.") 
                : null;
        }

        public static async Task<IContentReference?> FindOwnerAsync<T>(
            this IContentReferenceFinderFactory finderFactory,
            [NotNull] T? response,
            ILogger logger,
            ISharedResourcesLocalizer localizer,
            [DoesNotReturnIf(true)] bool throwIfNotFound,
            CancellationToken cancel)
            where T : IWithOwnerType, INamedContent
        {
            Guard.AgainstNull(response, nameof(response));

            var owner = Guard.AgainstNull(response.Owner, () => nameof(response.Owner));
            var ownerId = Guard.AgainstDefaultValue(owner.Id, () => nameof(response.Owner.Id));

            var userFinder = finderFactory.ForContentType<IUser>();

            var foundOwner = await userFinder.FindByIdAsync(ownerId, cancel).ConfigureAwait(false);

            if (foundOwner is not null)
                return foundOwner;

            logger.LogWarning(localizer[SharedResourceKeys.OwnerNotFoundMessage], ownerId, response.GetType().Name, response.Name);

            return throwIfNotFound
                ? throw new InvalidOperationException($"The owner with ID {ownerId} was not found.")
                : null;
        }
    }
}
