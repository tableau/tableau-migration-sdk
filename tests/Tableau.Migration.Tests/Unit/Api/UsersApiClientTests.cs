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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Tableau.Migration.Api;
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

        public class ListClient : PagedListApiClientTestBase<IUsersApiClient, IUser, UsersResponse>
        { }

        public class PageAccessor : ApiPageAccessorTestBase<IUsersApiClient, IUser, UsersResponse>
        { }

        #endregion

        public class ImportUsersAsync : UsersApiClientTest
        {
            [Fact]
            public async Task MultipleAuthTypes()
            {
                var users = AutoFixture.CreateMany<IUser>();
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
                var expectedPayload = $"<tsRequest>{string.Join("", users.Select(u => $@"<user name=""{u.Name}"" authSetting=""{u.AuthenticationType}"" />"))}</tsRequest>";
                Assert.Equal(expectedPayload, userPayload);

                request.AssertSingleHeaderValue("Accept", MediaTypes.Xml.MediaType!);
            }

            [Fact]
            public async Task SingleAuthType()
            {
                var users = AutoFixture.CreateMany<IUser>();
                foreach (var user in users)
                {
                    user.AuthenticationType = AuthenticationTypes.ServerDefault;
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
                    user.AuthenticationType = string.Empty;
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

        public class AddUserAsync : UsersApiClientTest
        {
            [Fact]
            public async Task Success()
            {
                //Setup
                var addResponse = AutoFixture.CreateResponse<AddUserResponse>();

                var expectedUserId = addResponse?.Item?.Id;
                Assert.NotNull(expectedUserId);

                var expectedUserName = addResponse?.Item?.Name;
                Assert.NotNull(expectedUserName);

                var expectedSiteRole = addResponse?.Item?.SiteRole;
                Assert.NotNull(expectedSiteRole);

                var expectedAuthSetting = addResponse?.Item?.AuthSetting;
                Assert.NotNull(expectedAuthSetting);

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(addResponse));

                //Act
                var result = await UsersApiClient.AddUserAsync(expectedUserName, expectedSiteRole, expectedAuthSetting, Cancel);

                //Test
                Assert.True(result.Success);
                Assert.Empty(result.Errors);
                var addUserResult = result.Value;
                Assert.NotNull(addUserResult);
                Assert.Equal(expectedUserId, addUserResult.Id);
                Assert.Equal(expectedUserName, addUserResult.Name);
                Assert.Equal(expectedSiteRole, addUserResult.SiteRole);
                Assert.Equal(expectedAuthSetting, addUserResult.AuthSetting);
            }

            [Fact]
            public async Task Failure()
            {
                //Setup                
                MockHttpClient.SetupResponse(new MockHttpResponseMessage<AddUserResponse>(HttpStatusCode.InternalServerError, null));

                //Act
                var result = await UsersApiClient.AddUserAsync("testUser", "testSiteRole", null, Cancel);

                //Test
                Assert.False(result.Success);
                Assert.NotEmpty(result.Errors);
                Assert.Single(result.Errors);
                Assert.Null(result.Value);
            }
        }

        public class UpdateUserAsync : UsersApiClientTest
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
                Assert.Equal(expectedAuthSetting, addUserResult.AuthSetting);
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
                                                                  newAuthSetting: expectedAuthSetting);

                //Test
                Assert.True(result.Success);
                Assert.Empty(result.Errors);
                var addUserResult = result.Value;
                Assert.NotNull(addUserResult);
                Assert.Equal(expectedUserName, addUserResult.Name);
                Assert.Equal(expectedFullName, addUserResult.FullName);
                Assert.Equal(expectedEmail, addUserResult.Email);
                Assert.Equal(expectedSiteRole, addUserResult.SiteRole);
                Assert.Equal(expectedAuthSetting, addUserResult.AuthSetting);
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
                                                                  newAuthSetting: "local");

                //Test
                Assert.False(result.Success);
                Assert.NotEmpty(result.Errors);
                Assert.Single(result.Errors);
                Assert.Null(result.Value);
            }
        }

        public class DeleteUserAsync : UsersApiClientTest
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

    }
}
