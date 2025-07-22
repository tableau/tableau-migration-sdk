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
    public sealed class IContentReferenceFinderFactoryExtensionsTests
    {
        #region - Test Classes -

        private class WithProjectNamedReferenceType : IWithProjectNamedReferenceType, INamedContent, IRestIdentifiable
        {
            public virtual Guid Id { get; set; }
            public virtual string? Name { get; set; }
            public virtual IProjectNamedReferenceType? Project { get; set; }
            IProjectReferenceType? IWithProjectReferenceType.Project => Project;
        }

        private class WithGroupType : INamedContent, IRestIdentifiable
        {
            public virtual Guid Id { get; set; }
            public virtual string? Name { get; set; }
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

        private class WithWorkbookNamedReferenceType : IWithWorkbookReferenceType, INamedContent, IRestIdentifiable
        {
            public virtual Guid Id { get; set; }
            public virtual string? Name { get; set; }
            public virtual IWorkbookReferenceType? Workbook { get; set; }
        }

        public abstract class IContentReferenceFinderFactoryExtensionsTest : AutoFixtureTestBase
        {
            protected readonly Mock<IContentReferenceFinderFactory> MockFinderFactory = new();
            protected readonly Mock<IContentReferenceFinder<IGroup>> MockGroupFinder = new();
            protected readonly Mock<IContentReferenceFinder<IProject>> MockProjectFinder = new();
            protected readonly Mock<IContentReferenceFinder<IUser>> MockUserFinder = new();
            protected readonly Mock<IContentReferenceFinder<IWorkbook>> MockWorkbookFinder = new();
            protected readonly Mock<IContentReferenceFinder<IView>> MockViewFinder = new();
            protected readonly Mock<ILogger> MockLogger = new();
            protected readonly ISharedResourcesLocalizer SharedResourcesLocalizer = new TestSharedResourcesLocalizer();

            public IContentReferenceFinderFactoryExtensionsTest()
            {
                MockFinderFactory.Setup(f => f.ForContentType<IGroup>()).Returns(MockGroupFinder.Object);
                MockFinderFactory.Setup(f => f.ForContentType<IProject>()).Returns(MockProjectFinder.Object);
                MockFinderFactory.Setup(f => f.ForContentType<IUser>()).Returns(MockUserFinder.Object);
                MockFinderFactory.Setup(f => f.ForContentType<IWorkbook>()).Returns(MockWorkbookFinder.Object);
                MockFinderFactory.Setup(f => f.ForContentType<IView>()).Returns(MockViewFinder.Object);
            }
        }

        #endregion

        #region - FindUserAsync -

        public sealed class FindGroupAsync : IContentReferenceFinderFactoryExtensionsTest
        {
            [Fact]
            public async Task NullThrowsAsync()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    MockFinderFactory.Object.FindGroupAsync<WithGroupType>(null, MockLogger.Object, SharedResourcesLocalizer, true, Cancel)
                );
            }

            [Fact]
            public async Task ThrowsDefaultGroupAsync()
            {
                await Assert.ThrowsAsync<ArgumentException>(() =>
                    MockFinderFactory.Object.FindGroupAsync(new WithGroupType(), MockLogger.Object, SharedResourcesLocalizer, true, Cancel)
                );
            }

            [Fact]
            public async Task GroupFoundAsync()
            {
                var mockContentReference = new Mock<IContentReference>();
                mockContentReference.SetupGet(o => o.Id).Returns(Guid.NewGuid());

                MockGroupFinder.Setup(f => f.FindByIdAsync(mockContentReference.Object.Id, Cancel)).ReturnsAsync(mockContentReference.Object);

                var response = new WithGroupType { Id = mockContentReference.Object.Id };

                var result = await MockFinderFactory.Object.FindGroupAsync(response, MockLogger.Object, SharedResourcesLocalizer, true, Cancel);

                Assert.Same(mockContentReference.Object, result);
            }

            [Fact]
            public async Task ReturnsNullAsync()
            {
                var response = new WithGroupType { Id = Guid.NewGuid() };

                var result = await MockFinderFactory.Object.FindGroupAsync(response, MockLogger.Object, SharedResourcesLocalizer, false, Cancel);

                Assert.Null(result);
            }

            [Fact]
            public async Task NotFoundThrowsAsync()
            {
                var response = new WithGroupType { Id = Guid.NewGuid() };

                await Assert.ThrowsAsync<InvalidOperationException>(() => 
                    MockFinderFactory.Object.FindGroupAsync(response, MockLogger.Object, SharedResourcesLocalizer, true, Cancel)
                );
            }
        }

        #endregion

        #region - FindProjectAsync -

        public sealed class FindProjectAsync : IContentReferenceFinderFactoryExtensionsTest
        {
            [Fact]
            public async Task Throws_when_response_is_null()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    MockFinderFactory.Object.FindProjectAsync<WithProjectNamedReferenceType>(
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
                        new WithProjectNamedReferenceType(),
                        MockLogger.Object,
                        SharedResourcesLocalizer,
                        true,
                        Cancel));
            }

            [Fact]
            public async Task Throws_when_response_project_id_is_default()
            {
                var mockProjectReference = new Mock<IProjectNamedReferenceType>();
                mockProjectReference.SetupGet(p => p.Id).Returns(Guid.Empty);

                var response = new WithProjectNamedReferenceType { Project = mockProjectReference.Object };

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

                var mockProjectReference = new Mock<IProjectNamedReferenceType>();
                mockProjectReference.SetupGet(p => p.Id).Returns(mockContentReference.Object.Id);

                MockProjectFinder.Setup(f => f.FindByIdAsync(mockContentReference.Object.Id, Cancel)).ReturnsAsync(mockContentReference.Object);

                var response = new WithProjectNamedReferenceType { Project = mockProjectReference.Object };

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
                var response = new WithProjectNamedReferenceType { Project = Create<Mock<IProjectNamedReferenceType>>().Object };

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
                var response = new WithProjectNamedReferenceType { Project = Create<Mock<IProjectNamedReferenceType>>().Object };

                await Assert.ThrowsAsync<InvalidOperationException>(() => MockFinderFactory.Object.FindProjectAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    true,
                    Cancel));
            }
        }

        #endregion

        #region - FindOwnerAsync -

        public sealed class FindOwnerAsync : IContentReferenceFinderFactoryExtensionsTest
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

        #endregion

        #region - FindUserAsync -

        public sealed class FindUserAsync : IContentReferenceFinderFactoryExtensionsTest
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

        #endregion

        #region - FindWorkbookAsync -

        public sealed class FindWorkbookAsync : IContentReferenceFinderFactoryExtensionsTest
        {
            [Fact]
            public async Task Throws_when_response_is_null()
            {
                await Assert.ThrowsAsync<ArgumentNullException>(() =>
                    MockFinderFactory.Object.FindWorkbookAsync<WithWorkbookNamedReferenceType>(
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
                        new WithWorkbookNamedReferenceType(),
                        MockLogger.Object,
                        SharedResourcesLocalizer,
                        true,
                        Cancel));
            }

            [Fact]
            public async Task Throws_when_response_workbook_id_is_default()
            {
                var mockWorkbookReference = new Mock<IWorkbookReferenceType>();
                mockWorkbookReference.SetupGet(o => o.Id).Returns(Guid.Empty);

                var response = new WithWorkbookNamedReferenceType { Workbook = mockWorkbookReference.Object };

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

                var mockWorkbookReference = new Mock<IWorkbookReferenceType>();
                mockWorkbookReference.SetupGet(o => o.Id).Returns(mockContentReference.Object.Id);

                MockWorkbookFinder.Setup(f => f.FindByIdAsync(mockWorkbookReference.Object.Id, Cancel)).ReturnsAsync(mockContentReference.Object);

                var response = new WithWorkbookNamedReferenceType { Workbook = mockWorkbookReference.Object };

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
                var response = new WithWorkbookNamedReferenceType { Workbook = Create<Mock<IWorkbookReferenceType>>().Object };

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
                var response = new WithWorkbookNamedReferenceType { Workbook = Create<Mock<IWorkbookReferenceType>>().Object };

                await Assert.ThrowsAsync<InvalidOperationException>(() => MockFinderFactory.Object.FindWorkbookAsync(
                    response,
                    MockLogger.Object,
                    SharedResourcesLocalizer,
                    true,
                    Cancel));
            }
        }

        #endregion
    }
}
