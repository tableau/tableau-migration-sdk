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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class CustomViewsApiClientTests
    {
        public abstract class CustomViewsApiClientTest : ApiClientTestBase<ICustomViewsApiClient>
        {
            internal CustomViewsApiClient CustomViewsApiClient => GetApiClient<CustomViewsApiClient>();
            protected readonly string BaseApiUri;

            protected CustomViewsApiClientTest()
            {
                MockConfigReader
                    .Setup(x => x.Get<IUser>())
                    .Returns(new ContentTypesOptions());
                BaseApiUri = $"/api/{TableauServerVersion.RestApiVersion}";
            }

            protected internal void AssertCustomViewRelativeUri(
                HttpRequestMessage request,
                Guid customViewId,
                string? suffix = null)
            {
                request.AssertRelativeUri($"{BaseApiUri}/{RestUrlKeywords.Sites}/{SiteId}/{RestUrlKeywords.CustomViews}/{customViewId.ToUrlSegment()}{suffix ?? string.Empty}");
            }
        }


        #region - List -

        public class ListClient : PagedListApiClientTestBase<ICustomViewsApiClient, ICustomView, CustomViewsResponse>
        { }

        public class PageAccessor : ApiPageAccessorTestBase<ICustomViewsApiClient, ICustomView, CustomViewsResponse>
        { }

        #endregion

        #region - Get -

        public class GetCustomViewAsync : CustomViewsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<CustomViewResponse>(HttpStatusCode.InternalServerError, null);
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
                    AssertCustomViewRelativeUri(r, contentId);
                });
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<CustomViewResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var contentId = Guid.NewGuid();

                var result = await ApiClient.GetByIdAsync(contentId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                    AssertCustomViewRelativeUri(r, contentId);
                });
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var cvResponse = AutoFixture.CreateResponse<CustomViewResponse>();
                cvResponse.Item!.Workbook = Create<CustomViewResponse.CustomViewType.WorkbookType>();
                cvResponse.Item!.View = Create<CustomViewResponse.CustomViewType.ViewType>();
                cvResponse.Item!.Owner = Create<CustomViewResponse.CustomViewType.OwnerType>();

                var workbook = Create<IContentReference>();
                MockWorkbookFinder.Setup(x => x.FindByIdAsync(cvResponse.Item.Workbook.Id, Cancel))
                    .ReturnsAsync(workbook);

                var view = cvResponse.Item!.View;

                var owner = Create<IContentReference>();
                MockUserFinder.Setup(x => x.FindByIdAsync(cvResponse.Item.Owner.Id, Cancel))
                    .ReturnsAsync(owner);

                var mockResponse = new MockHttpResponseMessage<CustomViewResponse>(cvResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var contentId = Guid.NewGuid();

                var result = await ApiClient.GetByIdAsync(
                    contentId,
                    Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                    AssertCustomViewRelativeUri(r, contentId);
                });

                Assert.Same(workbook, result.Value.Workbook);
                Assert.Equal(view.Id, result.Value.BaseViewId);
                Assert.Equal(view.Name, result.Value.BaseViewName);
                Assert.Same(owner, result.Value.Owner);
            }
        }

        #endregion

        #region - GetCustomViewDefaultUsersAsync -

        public class GetCustomViewDefaultUsersAsync : CustomViewsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<UsersWithCustomViewAsDefaultViewResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var contentId = Guid.NewGuid();

                var result = await ApiClient.GetCustomViewDefaultUsersAsync(contentId, 1, 100, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                    AssertCustomViewRelativeUri(r, contentId, "/default/users");
                });
            }

            [Fact]
            public async Task EmptyResponseAsync()
            {
                var usersResponse = AutoFixture.CreateResponse<UsersWithCustomViewAsDefaultViewResponse>();
                usersResponse.Items = Array.Empty<UsersWithCustomViewAsDefaultViewResponse.UserType>();

                var mockResponse = new MockHttpResponseMessage<UsersWithCustomViewAsDefaultViewResponse>(usersResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var contentId = Guid.NewGuid();

                var result = await ApiClient.GetCustomViewDefaultUsersAsync(
                    contentId,
                    1,
                    100,
                    Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                    AssertCustomViewRelativeUri(r, contentId, "/default/users");
                });

                Assert.Empty(result.Value);
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var usersResponse = AutoFixture.CreateResponse<UsersWithCustomViewAsDefaultViewResponse>();
                usersResponse.Items = CreateMany<UsersWithCustomViewAsDefaultViewResponse.UserType>(3).ToArray();

                var user1 = Create<IContentReference>();
                var user2 = Create<IContentReference>();
                var user3 = Create<IContentReference>();
                MockUserFinder.Setup(x => x.FindByIdAsync(usersResponse.Items[0].Id, Cancel))
                    .ReturnsAsync(user1);
                MockUserFinder.Setup(x => x.FindByIdAsync(usersResponse.Items[1].Id, Cancel))
                    .ReturnsAsync(user2);
                MockUserFinder.Setup(x => x.FindByIdAsync(usersResponse.Items[2].Id, Cancel))
                    .ReturnsAsync(user3);

                var mockResponse = new MockHttpResponseMessage<UsersWithCustomViewAsDefaultViewResponse>(usersResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var contentId = Guid.NewGuid();

                var result = await ApiClient.GetCustomViewDefaultUsersAsync(
                    contentId,
                    1,
                    100,
                    Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                    AssertCustomViewRelativeUri(r, contentId, "/default/users");
                });

                Assert.NotEmpty(result.Value);
                Assert.Equal(3, result.Value.Count);

                Assert.Same(user1, result.Value[0]);
                Assert.Same(user2, result.Value[1]);
                Assert.Same(user3, result.Value[2]);
            }
        }

        #endregion

        #region - GetAllCustomViewDefaultUsersAsync -

        public class GetAllCustomViewDefaultUsersAsync : CustomViewsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<UsersWithCustomViewAsDefaultViewResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var contentId = Guid.NewGuid();

                var result = await ApiClient.GetAllCustomViewDefaultUsersAsync(contentId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                    AssertCustomViewRelativeUri(r, contentId, "/default/users");
                });
            }

            [Fact]
            public async Task EmptyResponseAsync()
            {
                var usersResponse = AutoFixture.CreateResponse<UsersWithCustomViewAsDefaultViewResponse>();
                usersResponse.Items = Array.Empty<UsersWithCustomViewAsDefaultViewResponse.UserType>();

                var mockResponse = new MockHttpResponseMessage<UsersWithCustomViewAsDefaultViewResponse>(usersResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var contentId = Guid.NewGuid();

                var result = await ApiClient.GetAllCustomViewDefaultUsersAsync(contentId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                    AssertCustomViewRelativeUri(r, contentId, "/default/users");
                });

                Assert.Empty(result.Value);
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var usersResponse = AutoFixture.CreateResponse<UsersWithCustomViewAsDefaultViewResponse>();
                usersResponse.Items = CreateMany<UsersWithCustomViewAsDefaultViewResponse.UserType>(3).ToArray();

                var user1 = Create<IContentReference>();
                var user2 = Create<IContentReference>();
                var user3 = Create<IContentReference>();
                MockUserFinder.Setup(x => x.FindByIdAsync(usersResponse.Items[0].Id, Cancel))
                    .ReturnsAsync(user1);
                MockUserFinder.Setup(x => x.FindByIdAsync(usersResponse.Items[1].Id, Cancel))
                    .ReturnsAsync(user2);
                MockUserFinder.Setup(x => x.FindByIdAsync(usersResponse.Items[2].Id, Cancel))
                    .ReturnsAsync(user3);

                var mockResponse = new MockHttpResponseMessage<UsersWithCustomViewAsDefaultViewResponse>(usersResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var contentId = Guid.NewGuid();

                var result = await ApiClient.GetAllCustomViewDefaultUsersAsync(contentId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Get);
                    AssertCustomViewRelativeUri(r, contentId, "/default/users");
                });

                Assert.NotEmpty(result.Value);
                Assert.Equal(3, result.Value.Count);

                Assert.Same(user1, result.Value[0]);
                Assert.Same(user2, result.Value[1]);
                Assert.Same(user3, result.Value[2]);
            }
        }

        #endregion

        #region - Set Custom View Default users -

        public class SetCustomViewDefaultUsersAsync : CustomViewsApiClientTest
        {
            [Fact]
            public async Task SuccessAsync()
            {
                var customViewsResponse = AutoFixture.CreateResponse<CustomViewAsUsersDefaultViewResponse>();

                var mockResponse = new MockHttpResponseMessage<CustomViewAsUsersDefaultViewResponse>(customViewsResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var itemId = Guid.NewGuid();
                var users = AutoFixture.CreateMany<IUser>();

                var result = await ApiClient.SetCustomViewDefaultUsersAsync(itemId, users, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Post);
                    AssertCustomViewRelativeUri(r, itemId, "/default/users");
                });
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<CustomViewAsUsersDefaultViewResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var itemId = Guid.NewGuid();
                var users = AutoFixture.CreateMany<IUser>();

                var result = await ApiClient.SetCustomViewDefaultUsersAsync(itemId, users, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Post);
                    AssertCustomViewRelativeUri(r, itemId, "/default/users");
                });
            }
        }

        #endregion

        #region - Download -

        public class DownloadCustomViewAsync : CustomViewsApiClientTest
        {
            private void AssertCustomViewDownloadUri(Guid CustomViewId, HttpRequestMessage request)
            {
                request.AssertRelativeUri($"{BaseApiUri}/sites/{SiteId}/{RestUrlKeywords.CustomViews}/{CustomViewId}/{RestUrlKeywords.Content}");
            }

            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var CustomViewId = Guid.NewGuid();

                var result = await ApiClient.DownloadCustomViewAsync(CustomViewId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                AssertCustomViewDownloadUri(CustomViewId, request);
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var CustomViewId = Guid.NewGuid();

                var result = await ApiClient.DownloadCustomViewAsync(CustomViewId, Cancel);

                result.AssertFailure();


                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                AssertCustomViewDownloadUri(CustomViewId, request);
            }


            [Fact]
            public async Task SuccessAsync()
            {
                var content = new ByteArrayContent(Constants.DefaultEncoding.GetBytes("hi2u"));

                var mockResponse = new MockHttpResponseMessage(content);
                MockHttpClient.SetupResponse(mockResponse);

                var CustomViewId = Guid.NewGuid();

                var result = await ApiClient.DownloadCustomViewAsync(CustomViewId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                AssertCustomViewDownloadUri(CustomViewId, request);
            }
        }

        #endregion

        #region - PullAsync -
        public class PullAsync : CustomViewsApiClientTest
        {
            private ByteArrayContent? SetupFileDownloadResponse(
                HttpStatusCode? errorStatusCode = null,
                Exception? exception = null)
            {
                if (errorStatusCode is null)
                {
                    var content = new ByteArrayContent(Constants.DefaultEncoding.GetBytes("hi2u"));
                    MockHttpClient.SetupResponse(new MockHttpResponseMessage(content));
                    return content;
                }

                var mockResponse = new MockHttpResponseMessage(errorStatusCode.Value, null);
                MockHttpClient.SetupResponse(mockResponse);

                if (exception is null)
                {
                    return null;
                }

                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                return null;
            }

            private UsersWithCustomViewAsDefaultViewResponse? SetupDefaultUsersResponse(
                HttpStatusCode? errorStatusCode = null,
                Exception? exception = null)
            {
                if (errorStatusCode is null)
                {
                    var response = AutoFixture.CreateResponse<UsersWithCustomViewAsDefaultViewResponse>();
                    MockHttpClient.SetupResponse(
                        new MockHttpResponseMessage<UsersWithCustomViewAsDefaultViewResponse>(response));
                    return response;
                }

                var mockResponse = new MockHttpResponseMessage<UsersWithCustomViewAsDefaultViewResponse>(
                    errorStatusCode.Value,
                    null);

                MockHttpClient.SetupResponse(mockResponse);

                if (exception is null)
                {
                    return null;
                }

                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                return null;
            }

            [Fact]
            public async Task SuccessAsync()
            {
                // Setup mocking for file download
                SetupFileDownloadResponse();

                // Setup mocking for default users
                var defaultUsersResponse = SetupDefaultUsersResponse();
                Assert.NotNull(defaultUsersResponse);

                var customView = AutoFixture.Create<ICustomView>();
                var result = await ApiClient.PullAsync(customView, Cancel);

                // Verify
                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var requests = MockHttpClient.AssertRequestCount(2);

                var actualPublishableCustomView = result.Value;
                Assert.NotNull(actualPublishableCustomView);
                Assert.Equal(defaultUsersResponse.Items.Length, actualPublishableCustomView.DefaultUsers.Count);
            }

            [Fact]
            public async Task FileDownloadFailureResponse()
            {
                // Setup mocking for file download
                var exception = new Exception();
                SetupFileDownloadResponse(HttpStatusCode.InternalServerError, exception);

                // Setup mocking for default users
                var defaultUsersResponse = SetupDefaultUsersResponse();

                var customView = AutoFixture.Create<ICustomView>();
                var result = await ApiClient.PullAsync(customView, Cancel);

                // Verify
                Assert.Null(result.Value);
                Assert.Single(result.Errors);
                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
            }

            [Fact]
            public async Task DefaultUsersFailureResponse()
            {
                // Setup mocking for file download
                SetupFileDownloadResponse();

                // Setup mocking for default users
                var exception = new Exception();
                SetupDefaultUsersResponse(HttpStatusCode.InternalServerError, exception);

                var customView = AutoFixture.Create<ICustomView>();
                var result = await ApiClient.PullAsync(customView, Cancel);

                // Verify
                Assert.Null(result.Value);
                Assert.Single(result.Errors);
                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var requests = MockHttpClient.AssertRequestCount(2);
            }
        }
        #endregion

        #region - DeleteCustomViewAsync -

        public class DeleteCustomViewAsync : CustomViewsApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                //Setup
                var customViewId = Guid.NewGuid();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.NoContent));

                //Act
                var result = await CustomViewsApiClient.DeleteCustomViewAsync(customViewId, Cancel);

                //Test
                result.AssertSuccess();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                    AssertCustomViewRelativeUri(r, customViewId);
                });
            }

            [Fact]
            public async Task Failure()
            {
                //Setup
                var customViewId = Guid.NewGuid();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.InternalServerError));

                //Act
                var result = await CustomViewsApiClient.DeleteCustomViewAsync(customViewId, Cancel);

                //Test
                result.AssertFailure();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                    AssertCustomViewRelativeUri(r, customViewId);
                });
            }
        }

        #endregion

        #region - UpdateCustomViewAsync -

        public class UpdateCustomViewAsync : CustomViewsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<UpdateCustomViewResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var customViewId = Guid.NewGuid();

                var result = await ApiClient.UpdateCustomViewAsync(customViewId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Put);
                    AssertCustomViewRelativeUri(r, customViewId);
                });
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<UpdateCustomViewResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var customViewId = Guid.NewGuid();

                var result = await ApiClient.UpdateCustomViewAsync(customViewId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Put);
                    AssertCustomViewRelativeUri(r, customViewId);
                });
            }

            [Fact]
            public async Task NoUpdatesResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<UpdateCustomViewResponse>(HttpStatusCode.BadRequest, null);
                MockHttpClient.SetupResponse(mockResponse);

                var customViewId = Guid.NewGuid();

                var result = await ApiClient.UpdateCustomViewAsync(customViewId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Put);
                    AssertCustomViewRelativeUri(r, customViewId);
                });
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var wbResponse = AutoFixture.CreateResponse<UpdateCustomViewResponse>();

                var mockResponse = new MockHttpResponseMessage<UpdateCustomViewResponse>(wbResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var customViewId = Guid.NewGuid();

                var result = await ApiClient.UpdateCustomViewAsync(customViewId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Put);
                    AssertCustomViewRelativeUri(r, customViewId);
                });
            }
        }

        #endregion

        #region - PublishCustomViewAsync -
        public class PublishCustomViewAsync : CustomViewsApiClientTest
        {
            public PublishCustomViewAsync()
            {
                MockConfigReader
                  .Setup(x => x.Get<ICustomView>())
                  .Returns(new ContentTypesOptions());
            }

            private void SetupFileUploadErrorResponse(string errorCode)
                => SetupErrorResponse<FileUploadResponse>(new Error() { Code = errorCode });

            private (IHttpResponseMessage<CustomViewsResponse> Response, CustomViewsResponse Content) SetupCustomViewListResponse()
                => SetupSuccessResponse<CustomViewsResponse>();

            private PublishableCustomView CreatePublishableCustomView(CustomViewsResponse.CustomViewResponseType.WorkbookType workBook, CustomViewsResponse.CustomViewResponseType lastCustomView, IContentReference owner)
            {
                var cvToPublish = new CustomView(
                    lastCustomView,
                    new ContentReferenceStub(workBook.Id, Create<string>(),
                    Create<ContentLocation>()),
                    owner);

                var publishableCv = new PublishableCustomView(
                    cvToPublish,
                    Create<List<IContentReference>>(),
                    Create<IContentFileHandle>());

                return publishableCv;
            }

            private IContentReference CreateNewContentReferenceStub()
                => new ContentReferenceStub(Create<Guid>(), Create<string>(), Create<ContentLocation>());

            [Fact]
            public async Task SuccessAsync()
            {
                SetupSuccessResponse<FileUploadResponse, FileUploadResponse.FileUploadType>();

                // Setup for commit file upload request
                SetupSuccessResponse<CustomViewResponse, CustomViewResponse.CustomViewType>();

                var customView = Create<IPublishableCustomView>();

                var result = await ApiClient.PublishAsync(customView, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task Publish_fails()
            {
                SetupErrorResponse<FileUploadResponse>();

                var customView = Create<IPublishableCustomView>();

                var result = await ApiClient.PublishAsync(customView, Cancel);

                result.AssertFailure();
                Assert.Null(result.Value);
            }

            [Fact]
            public async Task Publish_succeeds_on_existing_custom_view()
            {
                SetupFileUploadErrorResponse(RestErrorCodes.CUSTOM_VIEW_ALREADY_EXISTS);
                SetupSuccessResponse();

                var workBook = Create<CustomViewsResponse.CustomViewResponseType.WorkbookType>();

                var customViewListSetup = SetupCustomViewListResponse();
                var customViews = customViewListSetup.Content.Items.ToList();
                var lastCustomView = customViews.Last();
                var owner = CreateNewContentReferenceStub();

                var publishableCv = CreatePublishableCustomView(workBook, lastCustomView, owner);

                var result = await ApiClient.PublishAsync(publishableCv, Cancel);

                MockHttpClient.AssertRequestCount(4);
            }

            [Fact]
            public async Task Publish_fails_for_reasons_other_than_existing_custom_view()
            {
                SetupFileUploadErrorResponse(CreateString());

                var workBook = Create<CustomViewsResponse.CustomViewResponseType.WorkbookType>();
                var customViewListSetup = SetupCustomViewListResponse();

                var customViews = customViewListSetup.Content.Items.ToList();
                var lastCustomView = customViews.Last();
                var owner = CreateNewContentReferenceStub();

                var publishableCv = CreatePublishableCustomView(workBook, lastCustomView, owner);

                var result = await ApiClient.PublishAsync(publishableCv, Cancel);

                result.AssertFailure();
                Assert.Null(result.Value);
            }
        }

        #endregion
    }
}