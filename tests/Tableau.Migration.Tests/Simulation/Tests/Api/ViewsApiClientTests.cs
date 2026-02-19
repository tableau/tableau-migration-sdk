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

using System;
using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public class ViewsApiClientTests
    {
        public class ViewsApiClientTest : ApiClientTestBase<IViewsApiClient, ViewResponse.ViewType>
        { }

        #region - GetByIdAsync -

        public class GetByIdAsync : ViewsApiClientTest
        {
            [Fact]
            public async Task SuccessAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var workbook = Api.Data.CreateWorkbook(AutoFixture);
                var workbookView = workbook.Views.First();

                // Create ViewResponse.ViewType and add to simulation data
                var view = new ViewResponse.ViewType()
                {
                    Id = workbookView.Id,
                    ContentUrl = workbookView.ContentUrl,
                    Name = workbookView.Name,
                    Workbook = new ViewResponse.ViewType.WorkbookReferenceType { Id = workbook.Id },
                    Project = new ViewResponse.ViewType.ProjectReferenceType { Id = workbook.Project!.Id }
                };

                Api.Data.AddView(view);

                // Act
                var result = await sitesClient.Views.GetByIdAsync(view.Id, Cancel);

                // Assert
                result.AssertSuccess();
                Assert.NotNull(result.Value);
                Assert.Equal(view.Id, result.Value.Id);
                Assert.Equal(view.Name, result.Value.Name);
                Assert.Equal(workbook.Id, result.Value.ParentWorkbook.Id);
            }

            [Fact]
            public async Task NotFoundAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var nonExistentViewId = Guid.NewGuid();

                // Act
                var result = await sitesClient.Views.GetByIdAsync(nonExistentViewId, Cancel);

                // Assert
                result.AssertFailure();
            }
        }

        #endregion

        #region - DeleteAsync -

        public class DeleteAsync : ViewsApiClientTest
        {
            [Fact]
            public async Task SuccessAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var workbook = Api.Data.CreateWorkbook(AutoFixture);
                var workbookView = workbook.Views.First();

                // Verify view exists before deletion
                Assert.Contains(Api.Data.Views, v => v.Id == workbookView.Id);

                // Act
                var result = await sitesClient.Views.DeleteAsync(workbookView.Id, Cancel);

                // Assert
                result.AssertSuccess();

                // Verify view was removed from data
                Assert.DoesNotContain(Api.Data.Views, v => v.Id == workbookView.Id);
            }

            [Fact]
            public async Task NotFoundAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var nonExistentViewId = Guid.NewGuid();

                // Act
                var result = await sitesClient.Views.DeleteAsync(nonExistentViewId, Cancel);

                // Assert
                result.AssertFailure();
            }

            [Fact]
            public async Task RemovesTheCorrectViewAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var workbook = Api.Data.CreateWorkbook(AutoFixture);
                var workbookViews = workbook.Views.Take(2).ToArray();

                var view1 = workbookViews[0];
                var view2 = workbookViews[1];

                var initialViewCount = Api.Data.Views.Count(v => v.Workbook?.Id == workbook.Id);
                Assert.Equal(workbook.Views.Length, initialViewCount);

                // Act
                var result = await sitesClient.Views.DeleteAsync(view1.Id, Cancel);

                // Assert
                result.AssertSuccess();

                // Verify only the specific view was removed
                Assert.DoesNotContain(Api.Data.Views, v => v.Id == view1.Id);
                Assert.Contains(Api.Data.Views, v => v.Id == view2.Id);

                var remainingViewCount = Api.Data.Views.Count(v => v.Workbook?.Id == workbook.Id);
                Assert.Equal(initialViewCount - 1, remainingViewCount);
            }

            
        }

        #endregion
    }
} 