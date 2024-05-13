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
using Moq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Xunit;

using RestProject = Tableau.Migration.Api.Rest.Models.Responses.ProjectsResponse.ProjectType;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class RestProjectBuilderTests
    {
        #region - Test Class -

        public class RestProjectBuilderTest : AutoFixtureTestBase
        {
            protected List<RestProject> Projects { get; }

            public RestProjectBuilderTest()
            {
                Projects = CreateMany<RestProject>().ToList();
            }

            internal RestProjectBuilder CreateBuilder()
                => new(Projects.ToImmutableDictionary(p => p.Id, p => p));
        }

        #endregion

        #region - RestProjects -

        public class RestProjects : RestProjectBuilderTest
        {
            [Fact]
            public void ReturnsValues()
            {
                var builder = CreateBuilder();

                Assert.True(Projects.SequenceEqual(builder.RestProjects, r => r.Id));
            }
        }

        #endregion

        #region - Count -

        public class Count : RestProjectBuilderTest
        {
            [Fact]
            public void ReturnsCount()
            {
                var builder = CreateBuilder();
                Assert.Equal(Projects.Count, builder.Count);
            }
        }

        #endregion

        #region - BuildProjectAsync -

        public class BuildProjectAsync : RestProjectBuilderTest
        {
            [Fact]
            public async Task BuildsProjectAsync()
            {
                var builder = CreateBuilder();

                var mockUserFinder = Create<Mock<IContentReferenceFinder<IUser>>>();

                var restProj = Projects.First();

                var owner = Create<IContentReference>();
                mockUserFinder.Setup(x => x.FindByIdAsync(restProj.Owner!.Id, Cancel))
                    .ReturnsAsync(owner);

                var proj = await builder.BuildProjectAsync(restProj, mockUserFinder.Object, Cancel);

                Assert.Equal(restProj.Id, proj.Id);
                Assert.Same(owner, proj.Owner);
            }

            [Fact]
            public async Task OwnerNotFoundAsync()
            {
                var builder = CreateBuilder();

                var mockUserFinder = Create<Mock<IContentReferenceFinder<IUser>>>();
                mockUserFinder.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), Cancel))
                    .ReturnsAsync((IContentReference?)null);

                var restProj = Projects.First();

                await Assert.ThrowsAsync<ArgumentNullException>(() => builder.BuildProjectAsync(restProj, mockUserFinder.Object, Cancel));
            }
        }

        #endregion

        #region - BuildLocation -

        public class BuildLocation : RestProjectBuilderTest
        {
            [Fact]
            public void TopLevelProject()
            {
                var proj = Create<RestProject>();
                proj.ParentProjectId = null;

                var builder = CreateBuilder();

                var loc = builder.BuildLocation(proj);
                Assert.Equal(proj.Name, loc.Path);
            }

            [Fact]
            public void SingleParentProject()
            {
                var parent = Projects.First();

                var proj = Create<RestProject>();
                proj.ParentProjectId = parent.Id.ToString();

                var builder = CreateBuilder();

                var loc = builder.BuildLocation(proj);
                Assert.Equal($"{parent.Name}/{proj.Name}", loc.Path);
            }

            [Fact]
            public void GetsAllParentProjects()
            {
                var ancestor = Projects.First();

                var parent = Create<RestProject>();
                parent.ParentProjectId = ancestor.Id.ToString();
                Projects.Add(parent);

                var proj = Create<RestProject>();
                proj.ParentProjectId = parent.Id.ToString();

                var builder = CreateBuilder();

                var loc = builder.BuildLocation(proj);
                Assert.Equal($"{ancestor.Name}/{parent.Name}/{proj.Name}", loc.Path);
            }
        }

        #endregion

        #region - LoadFromPagerAsync -

        public class LoadFromPagerAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task LoadsAllPagesAsync()
            {
                var projects = CreateMany<RestProject>(12).ToImmutableArray();

                var pager = new MemoryPager<RestProject>(projects, 5);
                var cancel = new CancellationToken();

                var builderResult = await RestProjectBuilder.LoadFromPagerAsync(pager, cancel);

                builderResult.AssertSuccess();

                var builder = builderResult.Value!;

                Assert.True(projects.SequenceEqual(builder.RestProjects, r => r.Id));
            }
        }

        #endregion
    }
}
