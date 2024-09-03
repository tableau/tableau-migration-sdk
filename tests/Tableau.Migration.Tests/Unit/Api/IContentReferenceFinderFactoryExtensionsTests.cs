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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class IContentReferenceFinderFactoryExtensionsTests
    {
        private class WithProjectType : IWithProjectType, INamedContent, IRestIdentifiable
        {
            public virtual Guid Id { get; set; }
            public virtual string? Name { get; set; }
            public virtual IProjectReferenceType? Project { get; set; }
        }

        private class WithUserType : INamedContent, IRestIdentifiable
        {
            public virtual Guid Id { get; set; }
            public virtual string? Name { get; set; }
        }

        private class WithOwnerType : IWithOwnerType, INamedContent, IRestIdentifiable
        {
            public virtual Guid Id { get; set; }
            public virtual string? Name { get; set; }
            public virtual IRestIdentifiable? Owner { get; set; }
        }

        private class WithWorkbookReferenceType : IWithWorkbookReferenceType, INamedContent, IRestIdentifiable
        {
            public virtual Guid Id { get; set; }
            public virtual string? Name { get; set; }
            public virtual IRestIdentifiable? Workbook { get; set; }
        }

        public abstract class IContentReferenceFinderFactoryExtensionsTest : AutoFixtureTestBase
        {
            protected readonly Mock<IContentReferenceFinderFactory> MockFinderFactory = new();
            protected readonly Mock<IContentReferenceFinder<IProject>> MockProjectFinder = new();
            protected readonly Mock<IContentReferenceFinder<IUser>> MockUserFinder = new();
            protected readonly Mock<IContentReferenceFinder<IWorkbook>> MockWorkbookFinder = new();
            protected readonly Mock<IContentReferenceFinder<IView>> MockViewFinder = new();
            protected readonly Mock<ILogger> MockLogger = new();
            protected readonly ISharedResourcesLocalizer SharedResourcesLocalizer = new TestSharedResourcesLocalizer();

            public IContentReferenceFinderFactoryExtensionsTest()
            {
                MockFinderFactory.Setup(f => f.ForContentType<IProject>()).Returns(MockProjectFinder.Object);
                MockFinderFactory.Setup(f => f.ForContentType<IUser>()).Returns(MockUserFinder.Object);
                MockFinderFactory.Setup(f => f.ForContentType<IWorkbook>()).Returns(MockWorkbookFinder.Object);
                MockFinderFactory.Setup(f => f.ForContentType<IView>()).Returns(MockViewFinder.Object);
            }
        }

        public class FindProjectAsync : IContentReferenceFinderFactoryExtensionsTest
        {
            [Fact]
            public async Task Throws_when_response_is_null()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    MockFinderFactory.Object.FindProjectAsync<WithProjectType>(
                        null,
                        MockLogger.Object,
                        SharedResourcesLocalizer,
                        true,
                        Cancel));
            }

            [Fact]
            public async Task Throws_when_response_project_is_null()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    MockFinderFactory.Object.FindProjectAsync(
                        new WithProjectType(),
                        MockLogger.Object,
                        SharedResourcesLocalizer,
                        true,
                        Cancel));
            }

            [Fact]
            public async Task Throws_when_response_project_id_is_default()
            {
                var mockProjectReference = new Mock<IProjectReferenceType>();
                mockProjectReference.SetupGet(p => p.Id).Returns(Guid.Empty);

                var response = new WithProjectType { Project =  mockProjectReference.Object };

                await Assert.ThrowsAsync<ArgumentException>(() =>
                    MockFinderFactory.Object.FindProjectAsync(
                        response,
                        MockLogger.Object,
                        SharedResourcesLocalizer,
                        true,
                        Cancel));
            }

            [Fact]
            public async Task Returns_project_when_found()
            {
                var mockContentReference = new Mock<IContentReference>();
                mockContentReference.SetupGet(p => p.Id).Returns(Guid.NewGuid());

                var mockProjectReference = new Mock<IProjectReferenceType>();
                mockProjectReference.SetupGet(p => p.Id).Returns(mockContentReference.Object.Id);

                MockProjectFinder.Setup(f => f.FindByIdAsync(mockContentReference.Object.Id, Cancel)).ReturnsAsync(mockContentReference.Object);

                var response = new WithProjectType { Project = mockProjectReference.Object };

                var result = await MockFinderFactory.Object.FindProjectAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    true,
                    Cancel);

                Assert.Same(mockContentReference.Object, result);
            }

            [Fact]
            public async Task Returns_null_when_not_found_and_throw_is_false()
            {
                var response = new WithProjectType { Project = Create<Mock<IProjectReferenceType>>().Object };

                var result = await MockFinderFactory.Object.FindProjectAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    false,
                    Cancel);

                Assert.Null(result);
            }

            [Fact]
            public async Task Throws_when_not_found_and_throw_is_true()
            {
                var response = new WithProjectType { Project = Create<Mock<IProjectReferenceType>>().Object };

                await Assert.ThrowsAsync<InvalidOperationException>(() => MockFinderFactory.Object.FindProjectAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    true,
                    Cancel));
            }
        }

        public class FindOwnerAsync : IContentReferenceFinderFactoryExtensionsTest
        {
            [Fact]
            public async Task Throws_when_response_is_null()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    MockFinderFactory.Object.FindOwnerAsync<WithOwnerType>(
                        null,
                        MockLogger.Object,
                        SharedResourcesLocalizer,
                        true,
                        Cancel));
            }

            [Fact]
            public async Task Throws_when_response_owner_is_null()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    MockFinderFactory.Object.FindOwnerAsync(
                        new WithOwnerType(),
                        MockLogger.Object,
                        SharedResourcesLocalizer,
                        true,
                        Cancel));
            }

            [Fact]
            public async Task Throws_when_response_owner_id_is_default()
            {
                var mockOwnerReference = new Mock<IRestIdentifiable>();
                mockOwnerReference.SetupGet(o => o.Id).Returns(Guid.Empty);

                var response = new WithOwnerType { Owner = mockOwnerReference.Object };

                await Assert.ThrowsAsync<ArgumentException>(() =>
                    MockFinderFactory.Object.FindOwnerAsync(
                        response,
                        MockLogger.Object,
                        SharedResourcesLocalizer,
                        true,
                        Cancel));
            }

            [Fact]
            public async Task Returns_owner_when_found()
            {
                var mockContentReference = new Mock<IContentReference>();
                mockContentReference.SetupGet(o => o.Id).Returns(Guid.NewGuid());

                var mockOwnerReference = new Mock<IRestIdentifiable>();
                mockOwnerReference.SetupGet(o => o.Id).Returns(mockContentReference.Object.Id);

                MockUserFinder.Setup(f => f.FindByIdAsync(mockContentReference.Object.Id, Cancel)).ReturnsAsync(mockContentReference.Object);

                var response = new WithOwnerType { Owner = mockOwnerReference.Object };

                var result = await MockFinderFactory.Object.FindOwnerAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    true,
                    Cancel);

                Assert.Same(mockContentReference.Object, result);
            }

            [Fact]
            public async Task Returns_null_when_not_found_and_throw_is_false()
            {
                var response = new WithOwnerType { Owner = Create<Mock<IRestIdentifiable>>().Object };

                var result = await MockFinderFactory.Object.FindOwnerAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    false,
                    Cancel);

                Assert.Null(result);
            }

            [Fact]
            public async Task Throws_when_not_found_and_throw_is_true()
            {
                var response = new WithOwnerType { Owner = Create<Mock<IRestIdentifiable>>().Object };

                await Assert.ThrowsAsync<InvalidOperationException>(() => MockFinderFactory.Object.FindOwnerAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    true,
                    Cancel));
            }
        }

        public class FindUserAsync : IContentReferenceFinderFactoryExtensionsTest
        {
            [Fact]
            public async Task Throws_when_response_is_null()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    MockFinderFactory.Object.FindUserAsync<WithUserType>(
                        null,
                        MockLogger.Object,
                        SharedResourcesLocalizer,
                        true,
                        Cancel));
            }

            [Fact]
            public async Task Throws_when_response_user_is_default()
            {
                await Assert.ThrowsAsync<ArgumentException>(() =>
                    MockFinderFactory.Object.FindUserAsync(
                        new WithUserType(),
                        MockLogger.Object,
                        SharedResourcesLocalizer,
                        true,
                        Cancel));
            }

            [Fact]
            public async Task Returns_user_when_found()
            {
                var mockContentReference = new Mock<IContentReference>();
                mockContentReference.SetupGet(o => o.Id).Returns(Guid.NewGuid());

                MockUserFinder.Setup(f => f.FindByIdAsync(mockContentReference.Object.Id, Cancel)).ReturnsAsync(mockContentReference.Object);

                var response = new WithUserType { Id = mockContentReference.Object.Id };

                var result = await MockFinderFactory.Object.FindUserAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    true,
                    Cancel);

                Assert.Same(mockContentReference.Object, result);
            }

            [Fact]
            public async Task Returns_null_when_not_found_and_throw_is_false()
            {
                var response = new WithUserType { Id = Guid.NewGuid() };

                var result = await MockFinderFactory.Object.FindUserAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    false,
                    Cancel);

                Assert.Null(result);
            }

            [Fact]
            public async Task Throws_when_not_found_and_throw_is_true()
            {
                var response = new WithUserType { Id = Guid.NewGuid() };

                await Assert.ThrowsAsync<InvalidOperationException>(() => MockFinderFactory.Object.FindUserAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    true,
                    Cancel));
            }
        }

        public class FindWorkbookAsync : IContentReferenceFinderFactoryExtensionsTest
        {
            [Fact]
            public async Task Throws_when_response_is_null()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    MockFinderFactory.Object.FindWorkbookAsync<WithWorkbookReferenceType>(
                        null,
                        MockLogger.Object,
                        SharedResourcesLocalizer,
                        true,
                        Cancel));
            }

            [Fact]
            public async Task Throws_when_response_workbook_is_null()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    MockFinderFactory.Object.FindWorkbookAsync(
                        new WithWorkbookReferenceType(),
                        MockLogger.Object,
                        SharedResourcesLocalizer,
                        true,
                        Cancel));
            }

            [Fact]
            public async Task Throws_when_response_workbook_id_is_default()
            {
                var mockWorkbookReference = new Mock<IRestIdentifiable>();
                mockWorkbookReference.SetupGet(o => o.Id).Returns(Guid.Empty);

                var response = new WithWorkbookReferenceType { Workbook = mockWorkbookReference.Object };

                await Assert.ThrowsAsync<ArgumentException>(() =>
                    MockFinderFactory.Object.FindWorkbookAsync(
                        response,
                        MockLogger.Object,
                        SharedResourcesLocalizer,
                        true,
                        Cancel));
            }

            [Fact]
            public async Task Returns_workbook_when_found()
            {
                var mockContentReference = new Mock<IContentReference>();
                mockContentReference.SetupGet(o => o.Id).Returns(Guid.NewGuid());

                var mockWorkbookReference = new Mock<IRestIdentifiable>();
                mockWorkbookReference.SetupGet(o => o.Id).Returns(mockContentReference.Object.Id);

                MockWorkbookFinder.Setup(f => f.FindByIdAsync(mockContentReference.Object.Id, Cancel)).ReturnsAsync(mockContentReference.Object);

                var response = new WithWorkbookReferenceType { Workbook = mockWorkbookReference.Object };

                var result = await MockFinderFactory.Object.FindWorkbookAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    true,
                    Cancel);

                Assert.Same(mockContentReference.Object, result);
            }

            [Fact]
            public async Task Returns_null_when_not_found_and_throw_is_false()
            {
                var response = new WithWorkbookReferenceType { Workbook = Create<Mock<IRestIdentifiable>>().Object };

                var result = await MockFinderFactory.Object.FindWorkbookAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    false,
                    Cancel);

                Assert.Null(result);
            }

            [Fact]
            public async Task Throws_when_not_found_and_throw_is_true()
            {
                var response = new WithWorkbookReferenceType { Workbook = Create<Mock<IRestIdentifiable>>().Object };

                await Assert.ThrowsAsync<InvalidOperationException>(() => MockFinderFactory.Object.FindWorkbookAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    true,
                    Cancel));
            }
        }
    }
}
