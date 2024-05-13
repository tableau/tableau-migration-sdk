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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

using RestProject = Tableau.Migration.Api.Rest.Models.Responses.ProjectsResponse.ProjectType;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Helper object that can load a project hierarchy from raw response objects,
    /// and use that hierarchy to acts as a location path builder 
    /// to build <see cref="IProject"/> objects.
    /// </summary>
    internal sealed class RestProjectBuilder
    {
        private readonly ImmutableDictionary<Guid, RestProject> _restProjectsById;

        private static readonly ImmutableHashSet<string> _systemProjectNames =
            DefaultExternalAssetsProjectTranslations.GetAll()
            .Append(Constants.DefaultProjectName)
            // The admin insight project is usually named "Admin Insights"
            // However, if that name is already taken when the real admin insights project is created
            // one of the alternate names is used
            .Append(Constants.AdminInsightsProjectName)
            .Append(Constants.AdminInsightsTableauProjectName)
            .Append(Constants.AdminInsightsTableauOnlineProjectName)
            .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<RestProject> RestProjects => _restProjectsById.Values;

        public int Count => _restProjectsById.Count;

        internal RestProjectBuilder(ImmutableDictionary<Guid, RestProject> restProjectsById)
        {
            _restProjectsById = restProjectsById;
        }

        public static async Task<IContentReference> FindProjectOwnerAsync(
            IProjectType restProject,
            IContentReferenceFinder<IUser> userFinder,
            CancellationToken cancel)
        {
            var owner = Guard.AgainstNull(restProject.Owner, () => nameof(restProject.Owner));
            var ownerId = Guard.AgainstDefaultValue(owner.Id, () => nameof(restProject.Owner.Id));

            var foundOwner = await userFinder.FindByIdAsync(ownerId, cancel).ConfigureAwait(false);

            if (foundOwner is null)
            {
                if (restProject.Name is not null && _systemProjectNames.Contains(restProject.Name))
                {
                    return new ContentReferenceStub(ownerId, string.Empty, Constants.SystemUserLocation);
                }

                throw new ArgumentNullException(
                    nameof(restProject),
                    $"The project's owner ID {ownerId} is not valid.");
            }

            return foundOwner;
        }

        public static async Task<IProject> BuildProjectAsync(IProjectType restProject, IContentReference? parent,
            IContentReferenceFinder<IUser> userFinder, CancellationToken cancel)
        {
            var owner = await FindProjectOwnerAsync(restProject, userFinder, cancel).ConfigureAwait(false);

            return new Project(restProject, parent, owner);
        }

        public async Task<IProject> BuildProjectAsync(RestProject restProject, IContentReferenceFinder<IUser> userFinder, CancellationToken cancel)
        {
            IContentReference? parentProject = null;

            var parentProjectId = restProject.GetParentProjectId();
            if (parentProjectId is not null)
            {
                if (_restProjectsById.TryGetValue(parentProjectId.Value, out var parentRestProject))
                {
                    var parentLoc = BuildLocation(parentRestProject);
                    parentProject = new ContentReferenceStub(parentProjectId.Value, string.Empty, parentLoc);
                }
            }

            return await BuildProjectAsync(restProject, parentProject, userFinder, cancel).ConfigureAwait(false);
        }

        internal ContentLocation BuildLocation(IProjectType project)
        {
            var parentProjectId = project.GetParentProjectId();
            var name = project.Name ?? string.Empty;

            if (parentProjectId is null)
            {
                return new(name);
            }

            var pathStack = new Stack<string>();
            pathStack.Push(name);
            while (parentProjectId is not null)
            {
                if (!_restProjectsById.TryGetValue(parentProjectId.Value, out var parentProject))
                {
                    break;
                }

                pathStack.Push(parentProject.Name ?? string.Empty);
                parentProjectId = parentProject.GetParentProjectId();
            }

            return new(pathStack);
        }

        public static async Task<IResult<RestProjectBuilder>> LoadFromPagerAsync(IPager<RestProject> pager, CancellationToken cancel)
        {
            var restProjects = ImmutableDictionary.CreateBuilder<Guid, RestProject>();

            var loadResult = await pager.GetAllPagesAsync(
                capacity => { }, //Immutable dictionary doesn't support setting capacity.
                page => restProjects.AddRange(page.Select(i => new KeyValuePair<Guid, RestProject>(i.Id, i))),
            cancel).ConfigureAwait(false);

            var projectBuilder = new RestProjectBuilder(restProjects.ToImmutable());

            return Result<RestProjectBuilder>.Create(loadResult, projectBuilder);
        }
    }
}
