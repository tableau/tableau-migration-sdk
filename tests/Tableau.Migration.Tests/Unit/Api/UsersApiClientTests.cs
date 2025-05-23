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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class UsersApiClientTests
    {
        public abstract class UsersApiClientTest : ApiClientTestBase<IUsersApiClient>
        {
            protected readonly Mock<IJobsApiClient> MockJobsApiClient = new();

            internal UsersApiClient UsersApiClient => GetApiClient<UsersApiClient>();
        }

        #region - List -

        public sealed class ListClient : PagedListApiClientTestBase<IUsersApiClient, IUser, UsersResponse>
        { }

        public sealed class PageAccessor : ApiPageAccessorTestBase<IUsersApiClient, IUser, UsersResponse>
        { }

        #endregion

        #region - ImportUsersAsync -

        public sealed class ImportUsersAsync : UsersApiClientTest
        {
            [Fact]
            public async Task MultipleAuthTypes()
            {
                var users = AutoFixture.CreateMany<IUser>();
                foreach (var u in users)
                {
                    u.Authentication = UserAuthenticationType.ForConfigurationId(Guid.NewGuid());
                }

                using var dataStream = Migration.Api.UsersApiClient.GenerateUserCsvStream(users);
                var userData = await new StreamContent(dataStream).ReadAsStringAsync(Cancel);
                dataStream.Seek(0, SeekOrigin.Begin);

                var importResponse = AutoFixture.CreateResponse<ImportJobResponse>();

                var mockResponse = new MockHttpResponseMessage<ImportJobResponse>(importResponse);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await UsersApiClient.ImportUsersAsync(users, dataStream, Cancel);

                Assert.True(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/users/import");

                var requestContent = Assert.IsType<MultipartFormDataContent>(request.Content);

                Assert.Equal(2, requestContent.Count());

                var requestStreamContent = Assert.IsType<StreamContent>(requestContent.ElementAt(0));
                var requestUserContent = Assert.IsType<StringContent>(requestContent.ElementAt(1));

                Assert.Equal("tableau_user_import", requestStreamContent.Headers.ContentDisposition?.Name);
                Assert.Equal(userData, await requestStreamContent.ReadAsStringAsync());

                Assert.Equal("request_payload", requestUserContent.Headers.ContentDisposition?.Name);

                var userPayload = await requestUserContent.ReadAsStringAsync();
                var expectedPayload = $"<tsRequest>{string.Join("", users.Select(u => $@"<user name=""{u.Name}"" idpConfigurationId=""{u.Authentication.IdpConfigurationId}"" />"))}</tsRequest>";
                Assert.Equal(expectedPayload, userPayload);

                request.AssertSingleHeaderValue("Accept", MediaTypes.Xml.MediaType!);
            }

            [Fact]
            public async Task SingleAuthType()
            {
                var users = AutoFixture.CreateMany<IUser>();
                foreach (var user in users)
                {
                    user.Authentication = UserAuthenticationType.ForAuthenticationType(AuthenticationTypes.ServerDefault);
                }

                using var dataStream = Migration.Api.UsersApiClient.GenerateUserCsvStream(users);
                var userData = await new StreamContent(dataStream).ReadAsStringAsync(Cancel);
                dataStream.Seek(0, SeekOrigin.Begin);

                var importResponse = AutoFixture.CreateResponse<ImportJobResponse>();

                var mockResponse = new MockHttpResponseMessage<ImportJobResponse>(importResponse);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await UsersApiClient.ImportUsersAsync(users, dataStream, Cancel);

                Assert.True(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/users/import");

                var requestContent = Assert.IsType<MultipartFormDataContent>(request.Content);

                Assert.Equal(2, requestContent.Count());

                var requestStreamContent = Assert.IsType<StreamContent>(requestContent.ElementAt(0));
                var requestUserContent = Assert.IsType<StringContent>(requestContent.ElementAt(1));

                Assert.Equal("tableau_user_import", requestStreamContent.Headers.ContentDisposition?.Name);
                Assert.Equal(userData, await requestStreamContent.ReadAsStringAsync());

                Assert.Equal("request_payload", requestUserContent.Headers.ContentDisposition?.Name);

                var userPayload = await requestUserContent.ReadAsStringAsync();
                Assert.Equal("<tsRequest><user authSetting=\"ServerDefault\" /></tsRequest>", userPayload);

                request.AssertSingleHeaderValue("Accept", MediaTypes.Xml.MediaType!);
            }

            [Fact]
            public async Task NoAuthTypes()
            {
                var users = AutoFixture.CreateMany<IUser>();
                foreach (var user in users)
                {
                    user.Authentication = UserAuthenticationType.Default;
                }

                using var dataStream = Migration.Api.UsersApiClient.GenerateUserCsvStream(users);
                var userData = await new StreamContent(dataStream).ReadAsStringAsync(Cancel);
                dataStream.Seek(0, SeekOrigin.Begin);

                var importResponse = AutoFixture.CreateResponse<ImportJobResponse>();

                var mockResponse = new MockHttpResponseMessage<ImportJobResponse>(importResponse);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await UsersApiClient.ImportUsersAsync(users, dataStream, Cancel);

                Assert.True(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/users/import");

                var requestContent = Assert.IsType<MultipartFormDataContent>(request.Content);

                Assert.Equal(2, requestContent.Count());

                var requestStreamContent = Assert.IsType<StreamContent>(requestContent.ElementAt(0));
                var requestUserContent = Assert.IsType<StringContent>(requestContent.ElementAt(1));

                Assert.Equal("tableau_user_import", requestStreamContent.Headers.ContentDisposition?.Name);
                Assert.Equal(userData, await requestStreamContent.ReadAsStringAsync());

                Assert.Equal("request_payload", requestUserContent.Headers.ContentDisposition?.Name);

                var userPayload = await requestUserContent.ReadAsStringAsync();
                Assert.Equal("<tsRequest><user /></tsRequest>", userPayload);

                request.AssertSingleHeaderValue("Accept", MediaTypes.Xml.MediaType!);
            }

            [Fact]
            public async Task ReturnsFailure()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<ImportJobResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await UsersApiClient.ImportUsersAsync(Array.Empty<IUser>(), new MemoryStream(), Cancel);

                Assert.False(result.Success);

                var error = Assert.Single(result.Errors);

                Assert.Same(exception, error);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/users/import");
            }
        }

        #endregion

        #region - AddUserAsync -

        public sealed class AddUserAsync : UsersApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                //Setup
                var addResponse = AutoFixture.CreateResponse<AddUserResponse>();
                addResponse.Item!.IdpConfigurationId = Guid.NewGuid().ToString();

                var expectedUserId = addResponse.Item.Id;

                var expectedUserName = addResponse.Item.Name;
                Assert.NotNull(expectedUserName);

                var expectedSiteRole = addResponse.Item.SiteRole;
                Assert.NotNull(expectedSiteRole);

                var expectedAuthSetting = addResponse.Item.AuthSetting;
                Assert.NotNull(expectedAuthSetting);

                addResponse.Item.IdpConfigurationId = null;

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(addResponse));

                //Act
                var result = await UsersApiClient.AddUserAsync(expectedUserName, expectedSiteRole, UserAuthenticationType.ForAuthenticationType(expectedAuthSetting), Cancel);

                //Test
                Assert.True(result.Success);
                Assert.Empty(result.Errors);
                var addUserResult = result.Value;
                Assert.NotNull(addUserResult);
                Assert.Equal(expectedUserId, addUserResult.Id);
                Assert.Equal(expectedUserName, addUserResult.Name);
                Assert.Equal(expectedSiteRole, addUserResult.SiteRole);
                Assert.Equal(expectedAuthSetting, addUserResult.Authentication.AuthenticationType);
                Assert.Null(addUserResult.Authentication.IdpConfigurationId);
            }

            [Fact]
            public async Task Failure()
            {
                //Setup                
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(HttpStatusCode.InternalServerError, null));

                //Act
                var result = await UsersApiClient.AddUserAsync("testUser", "testSiteRole", UserAuthenticationType.Default, Cancel);

                //Test
                Assert.False(result.Success);
                Assert.NotEmpty(result.Errors);
                Assert.Single(result.Errors);
                Assert.Null(result.Value);
            }

            [Fact]
            public async Task Succeeds_when_user_exists()
            {
                // Setup
                var existingUserResponse = new UsersResponse
                {
                    Items = new[] { AutoFixture.Create<UsersResponse.UserType>() },
                    Pagination = new Pagination { PageSize = 1, PageNumber = 1, TotalAvailable = 1 }
                };

                var existingUser = existingUserResponse.Items[0];

                var addUserResponse = new AddUserResponse
                {
                    Error = new Error
                    {
                        Code = UsersApiClient.USER_NAME_CONFLICT_ERROR_CODE
                    }
                };

                // Mock the AddUserAsync response to return a conflict error
                var conflictResponse = new MockHttpResponseMessage<AddUserResponse>(HttpStatusCode.Conflict, addUserResponse);
                MockHttpClient.SetupResponse(conflictResponse);

                // Mock the GetAllUsersAsync response to return the existing user
                var getAllUsersResponse = new MockHttpResponseMessage<UsersResponse>(existingUserResponse);
                MockHttpClient.SetupResponse(getAllUsersResponse);

                // Act
                var result = await UsersApiClient.AddUserAsync(existingUser.Name!, existingUser.SiteRole!, UserAuthenticationType.ForAuthenticationType(existingUser.AuthSetting!), Cancel);

                // Test
                Assert.True(result.Success);
                Assert.Empty(result.Errors);
                var addUserResult = result.Value;
                Assert.NotNull(addUserResult);
                Assert.Equal(existingUser.Id, addUserResult.Id);
                Assert.Equal(existingUser.Name, addUserResult.Name);
                Assert.Equal(existingUser.SiteRole, addUserResult.SiteRole);
                Assert.Equal(existingUser.AuthSetting, addUserResult.Authentication.AuthenticationType);
                Assert.Null(addUserResult.Authentication.IdpConfigurationId);
            }
        }

        #endregion

        #region - UpdateUserAsync -

        public sealed class UpdateUserAsync : UsersApiClientTest
        {
            [Fact]
            public async Task Success_With_Required_Params()
            {
                //Setup
                var updateResponse = AutoFixture.CreateResponse<UpdateUserResponse>();

                var expectedSiteRole = updateResponse?.Item?.SiteRole;
                Assert.NotNull(expectedSiteRole);

                var expectedFullName = updateResponse?.Item?.FullName;
                Assert.NotNull(expectedFullName);

                var expectedEmail = updateResponse?.Item?.Email;
                Assert.NotNull(expectedEmail);

                var expectedUserName = updateResponse?.Item?.Name;
                Assert.NotNull(expectedUserName);

                var expectedAuthSetting = updateResponse?.Item?.AuthSetting;
                Assert.NotNull(expectedAuthSetting);

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UpdateUserResponse>(updateResponse));

                //Act
                var result = await UsersApiClient.UpdateUserAsync(id: Guid.NewGuid(), newSiteRole: expectedSiteRole, Cancel);

                //Test
                Assert.True(result.Success);
                Assert.Empty(result.Errors);
                var addUserResult = result.Value;
                Assert.NotNull(addUserResult);
                Assert.Equal(expectedUserName, addUserResult.Name);
                Assert.Equal(expectedFullName, addUserResult.FullName);
                Assert.Equal(expectedEmail, addUserResult.Email);
                Assert.Equal(expectedSiteRole, addUserResult.SiteRole);
                Assert.Equal(expectedAuthSetting, addUserResult.Authentication.AuthenticationType);
            }

            [Fact]
            public async Task Success_With_All_Params()
            {
                //Setup
                var updateResponse = AutoFixture.CreateResponse<UpdateUserResponse>();

                var expectedSiteRole = updateResponse?.Item?.SiteRole;
                Assert.NotNull(expectedSiteRole);

                var expectedFullName = updateResponse?.Item?.FullName;
                Assert.NotNull(expectedFullName);

                var expectedEmail = updateResponse?.Item?.Email;
                Assert.NotNull(expectedEmail);

                var expectedUserName = updateResponse?.Item?.Name;
                Assert.NotNull(expectedUserName);

                var expectedAuthSetting = updateResponse?.Item?.AuthSetting;
                Assert.NotNull(expectedAuthSetting);

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UpdateUserResponse>(updateResponse));

                //Act
                var result = await UsersApiClient.UpdateUserAsync(id: Guid.NewGuid(),
                                                                  newSiteRole: expectedSiteRole,
                                                                  Cancel,
                                                                  newfullName: expectedFullName,
                                                                  newEmail: expectedEmail,
                                                                  newPassword: "Old-McD0nald-H@d-a-F@rm",
                                                                  newAuthentication: UserAuthenticationType.ForAuthenticationType(expectedAuthSetting));

                //Test
                Assert.True(result.Success);
                Assert.Empty(result.Errors);
                var addUserResult = result.Value;
                Assert.NotNull(addUserResult);
                Assert.Equal(expectedUserName, addUserResult.Name);
                Assert.Equal(expectedFullName, addUserResult.FullName);
                Assert.Equal(expectedEmail, addUserResult.Email);
                Assert.Equal(expectedSiteRole, addUserResult.SiteRole);
                Assert.Equal(expectedAuthSetting, addUserResult.Authentication.AuthenticationType);
            }
            [Fact]
            public async Task Failure()
            {
                //Setup
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UpdateUserResponse>(HttpStatusCode.InternalServerError, null));

                //Act
                var result = await UsersApiClient.UpdateUserAsync(id: Guid.NewGuid(),
                                                                  newSiteRole: "RandomRole",
                                                                  Cancel,
                                                                  newfullName: "Jack Sparrow",
                                                                  newEmail: "jsparrow@example.com",
                                                                  newPassword: "Why-Is-Th3-Rum-Gone?",
                                                                  newAuthentication: UserAuthenticationType.ForAuthenticationType("local"));

                //Test
                Assert.False(result.Success);
                Assert.NotEmpty(result.Errors);
                Assert.Single(result.Errors);
                Assert.Null(result.Value);
            }
        }

        #endregion

        #region - DeleteUserAsync -

        public sealed class DeleteUserAsync : UsersApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                //Setup
                var userId = Guid.NewGuid();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.NoContent));

                //Act
                var result = await UsersApiClient.DeleteUserAsync(userId, Cancel);

                //Test
                result.AssertSuccess();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                    r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/users/{userId}");
                });
            }

            [Fact]
            public async Task Failure()
            {
                //Setup
                var userId = Guid.NewGuid();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.InternalServerError));

                //Act
                var result = await UsersApiClient.DeleteUserAsync(userId, Cancel);

                //Test
                result.AssertFailure();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                    r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/users/{userId}");
                });
            }
        }

        #endregion

        #region - RetrieveUserSavedCredentialsAsync -

        public sealed class RetrieveUserSavedCredentialsAsync : UsersApiClientTest
        {
            [Fact]
            public async Task Returns_success()
            {
                var userId = Create<Guid>();
                var options = Create<IDestinationSiteInfo>();
                var response = AutoFixture.CreateResponse<RetrieveKeychainResponse>();

                var mockResponse = new MockHttpResponseMessage<RetrieveKeychainResponse>(response);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.RetrieveUserSavedCredentialsAsync(userId, options, Cancel);

                Assert.True(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/users/{userId}/retrieveSavedCreds");

                var requestContent = Assert.IsType<StringContent>(request.Content);

                var requestModel = await HttpContentSerializer.Instance.DeserializeAsync<RetrieveUserSavedCredentialsRequest>(requestContent, Cancel);

                Assert.NotNull(requestModel);
                Assert.Equal(options.ContentUrl, requestModel.DestinationSiteUrlNamespace);
                Assert.Equal(options.SiteUrl, requestModel.DestinationServerUrl);
                Assert.Equal(options.SiteId, requestModel.DestinationSiteLuid);
            }

            [Fact]
            public async Task Returns_failure()
            {
                var userId = Create<Guid>();
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<RetrieveKeychainResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await ApiClient.RetrieveUserSavedCredentialsAsync(userId, Create<IDestinationSiteInfo>(), Cancel);

                Assert.False(result.Success);

                var error = Assert.Single(result.Errors);

                Assert.Same(exception, error);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/users/{userId}/retrieveSavedCreds");
            }
        }

        #endregion

        #region - UploadUserSavedCredentialsAsync -

        public sealed class UploadUserSavedCredentialsAsync : UsersApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.InternalServerError);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var userId = Create<Guid>();
                var encryptedKeychains = Create<IEnumerable<string>>();

                var result = await ApiClient.UploadUserSavedCredentialsAsync(userId, encryptedKeychains, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/users/{userId}/uploadSavedCreds");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var mockResponse = new MockHttpResponseMessage(HttpStatusCode.OK);
                MockHttpClient.SetupResponse(mockResponse);

                var userId = Create<Guid>();
                var encryptedKeychains = Create<IEnumerable<string>>();

                var result = await ApiClient.UploadUserSavedCredentialsAsync(userId, encryptedKeychains, Cancel);

                result.AssertSuccess();

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/users/{userId}/uploadSavedCreds");
            }
        }

        #endregion

        #region - PublishAsync -

        public sealed class PublishAsync : UsersApiClientTest
        {
            [Fact]
            public async Task AddsAndUpdatesServerAsync()
            {
                InstanceType = TableauInstanceType.Server;

                var user = Create<IUser>();

                var addUserResponse = AutoFixture.CreateResponse<AddUserResponse>();
                var updateUserResponse = AutoFixture.CreateResponse<UpdateUserResponse>();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(HttpStatusCode.Created, addUserResponse));
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UpdateUserResponse>(updateUserResponse));

                var result = await ApiClient.PublishAsync(user, Cancel);

                result.AssertSuccess();

                Assert.Equal(addUserResponse.Item!.Id, result.Value!.Id);
            }

            [Fact]
            public async Task AddsAndUpdatesCloudAsync()
            {
                InstanceType = TableauInstanceType.Cloud;

                var user = Create<IUser>();

                var addUserResponse = AutoFixture.CreateResponse<AddUserResponse>();
                var updateUserResponse = AutoFixture.CreateResponse<UpdateUserResponse>();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(HttpStatusCode.Created, addUserResponse));
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UpdateUserResponse>(updateUserResponse));

                var result = await ApiClient.PublishAsync(user, Cancel);

                result.AssertSuccess();

                Assert.Equal(addUserResponse.Item!.Id, result.Value!.Id);
            }

            [Fact]
            public async Task InsufficientLicensesAsync()
            {
                var user = Create<IUser>();

                var addUserResponse = AutoFixture.CreateResponse<AddUserResponse>();
                var updateUserResponse = AutoFixture.CreateResponse<UpdateUserResponse>();
                updateUserResponse.Item!.SiteRole = SiteRoles.Unlicensed;

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(HttpStatusCode.Created, addUserResponse));
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UpdateUserResponse>(updateUserResponse));

                var result = await ApiClient.PublishAsync(user, Cancel);

                result.AssertFailure();
            }

            [Fact]
            public async Task AddFailsAsync()
            {
                var user = Create<IUser>();

                var addUserResponse = AutoFixture.CreateErrorResponse<AddUserResponse>();
                var updateUserResponse = AutoFixture.CreateResponse<UpdateUserResponse>();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(addUserResponse));
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UpdateUserResponse>(updateUserResponse));

                var result = await ApiClient.PublishAsync(user, Cancel);

                result.AssertFailure();
            }

            [Fact]
            public async Task UpdateFailsAsync()
            {
                var user = Create<IUser>();

                var addUserResponse = AutoFixture.CreateResponse<AddUserResponse>();
                var updateUserResponse = AutoFixture.CreateErrorResponse<UpdateUserResponse>();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(addUserResponse));
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<UpdateUserResponse>(updateUserResponse));

                var result = await ApiClient.PublishAsync(user, Cancel);

                result.AssertFailure();
            }
        }

        #endregion
    }
}
