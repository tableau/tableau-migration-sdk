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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Tests.Unit.Api.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class ViewsApiClientTests
    {
        public class ViewsApiClientTest : PermissionsApiClientTestBase<IViewsApiClient>
        {
            #region - Test Helpers -

            public void SetupGetPermissionsAsync(bool success, IPermissions? permissions = null)
            {
                var setup = MockPermissionsClient
                    .Setup(c => c.GetPermissionsAsync(
                        It.IsAny<Guid>(),
                        Cancel));

                if (success)
                {
                    Assert.NotNull(permissions);

                    setup.Returns(
                        Task.FromResult<IResult<IPermissions>>(
                            Result<IPermissions>.Create(Result.Succeeded(), permissions)));
                    return;
                }

                setup.Returns(Task.FromResult<IResult<IPermissions>>(Result<IPermissions>.Failed(new Exception())));
            }

            public void VerifyGetPermissionsAsync(Times times)
            {
                MockPermissionsClient
                    .Verify(c => c.GetPermissionsAsync(
                            It.IsAny<Guid>(),
                            Cancel),
                        times);
            }

            #endregion

            #region - Get -

            public class GetViewAsync : ViewsApiClientTest
            {
                [Fact]
                public async Task ErrorAsync()
                {
                    var exception = new Exception();

                    var mockResponse = new MockHttpResponseMessage<ViewResponse>(HttpStatusCode.InternalServerError, null);
                    mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                    MockHttpClient.SetupResponse(mockResponse);

                    var contentId = Guid.NewGuid();

                    var result = await ApiClient.GetByIdAsync(contentId, Cancel);

                    result.AssertFailure();

                    var resultError = Assert.Single(result.Errors);
                    Assert.Same(exception, resultError);

                    MockHttpClient.AssertSingleRequest(r =>
                    {
                        r.AssertHttpMethod(HttpMethod.Get);
                    });
                }

                [Fact]
                public async Task FailureResponseAsync()
                {
                    var mockResponse = new MockHttpResponseMessage<ViewResponse>(HttpStatusCode.NotFound, null);
                    MockHttpClient.SetupResponse(mockResponse);

                    var contentId = Guid.NewGuid();

                    var result = await ApiClient.GetByIdAsync(contentId, Cancel);

                    result.AssertFailure();

                    Assert.Null(result.Value);
                    Assert.Single(result.Errors);

                    MockHttpClient.AssertSingleRequest(r =>
                    {
                        r.AssertHttpMethod(HttpMethod.Get);
                    });
                }

                [Fact]
                public async Task SuccessAsync()
                {
                    // Arrange
                    // View Response
                    var viewResponse = AutoFixture.CreateResponse<ViewResponse>();
                    viewResponse.Item!.Project = Create<ViewResponse.ViewType.ProjectReferenceType>();
                    viewResponse.Item!.Workbook = Create<ViewResponse.ViewType.WorkbookReferenceType>();

                    // The mock workbook of the view
                    var workbook = Create<IContentReference>();
                    MockWorkbookFinder.Setup(x => x.FindByIdAsync(viewResponse.Item.Workbook.Id, Cancel))
                        .ReturnsAsync(workbook);

                    // The mock project of the view
                    var project = Create<IContentReference>();
                    MockProjectFinder.Setup(x => x.FindByIdAsync(viewResponse.Item.Project.Id, Cancel))
                        .ReturnsAsync(project);

                    // The mock response message
                    var mockResponse = new MockHttpResponseMessage<ViewResponse>(viewResponse);
                    MockHttpClient.SetupResponse(mockResponse);

                    var contentId = Guid.NewGuid();

                    // Act
                    var result = await ApiClient.GetByIdAsync(
                        contentId,
                        Cancel);

                    // Assert
                    result.AssertSuccess();
                    Assert.NotNull(result.Value);

                    MockHttpClient.AssertSingleRequest(r =>
                    {
                        r.AssertHttpMethod(HttpMethod.Get);
                    });

                    Assert.Same(workbook, result.Value.ParentWorkbook);
                }
            }

            #endregion - Get -

            #region - Permissions -

            public class GetPermissionsAsync : ViewsApiClientTest
            {
                [Fact]
                public async Task Success()
                {
                    var sourcePermissions = Create<IPermissions>();
                    var destinationPermissions = Create<IPermissions>();

                    SetupGetPermissionsAsync(true, destinationPermissions);

                    var result = await MockPermissionsClient.Object.GetPermissionsAsync(
                        Guid.NewGuid(),
                        Cancel);

                    Assert.True(result.Success);

                    // Get permissions is called once.
                    VerifyGetPermissionsAsync(Times.Once());
                }
            }

            #endregion
        }
    }
}