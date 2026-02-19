//
//  Copyright (c) 2026, Salesforce, Inc.
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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net.Rest.Filtering;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api.Paging
{
    internal class ProjectFilteredListPager : IndexedPagerBase<IProject>
    {
        private readonly IProjectsResponseApiClient _apiClient;
        private readonly IEnumerable<Filter> _filters;

        private IContentReferenceFinder<IProject> _projectFinder;
        private IContentReferenceFinder<IUser> _userFinder;

        public ProjectFilteredListPager(IProjectsResponseApiClient apiClient, IContentReferenceFinderFactory finderFactory,
            IEnumerable<Filter> filters, int pageSize)
            : base(pageSize)
        {
            _apiClient = apiClient;
            _filters = filters;

            _projectFinder = finderFactory.ForContentType<IProject>();
            _userFinder = finderFactory.ForContentType<IUser>();
        }

        protected override async Task<IPagedResult<IProject>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
        {
            var responseResult = await _apiClient.GetAllProjectsAsync(pageNumber, pageSize, _filters, cancel).ConfigureAwait(false);
            if(!responseResult.Success)
            {
                return responseResult.CastPagedFailure<IProject>();
            }

            var projects = ImmutableArray.CreateBuilder<IProject>(responseResult.Value.Count);
            foreach(var response in responseResult.Value)
            {
                IContentReference? parentProject = null;

                var parentProjectId = response.GetParentProjectId();
                if (parentProjectId is not null)
                {
                    parentProject = await _projectFinder.FindByIdAsync(parentProjectId.Value, cancel).ConfigureAwait(false);
                }

                var project = await RestProjectBuilder.BuildProjectAsync(response, parentProject, _userFinder, cancel).ConfigureAwait(false);
                projects.Add(project);
            }

            return PagedResult<IProject>.Succeeded(projects.ToImmutable(), pageNumber, pageSize, responseResult.TotalCount, responseResult.FetchedAllPages);
        }
    }
}
