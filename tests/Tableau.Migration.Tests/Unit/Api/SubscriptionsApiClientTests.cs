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
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Xunit;

using CloudResponses = Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using ServerResponses = Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Tests.Unit.Api
{
    public sealed class SubscriptionsApiClientTests
    {
        public abstract class SubscriptionsApiClientTest : ApiClientTestBase<ISubscriptionsApiClient>
        {
            internal virtual SubscriptionsApiClient SubscriptionsApiClient => GetApiClient<SubscriptionsApiClient>();

            protected TableauInstanceType CurrentInstanceType { get; set; }

            public SubscriptionsApiClientTest()
                => MockSessionProvider.SetupGet(p => p.InstanceType).Returns(() => CurrentInstanceType);

            protected static List<TSubscription> AssertSuccess<TSubscription, TSchedule>(
                IResult<IImmutableList<TSubscription>> result)
                where TSubscription : ISubscription<TSchedule>
                where TSchedule : ISchedule
            {
                Assert.NotNull(result);
                Assert.Empty(result.Errors);

                var actualSubscriptions = result.Value?.ToList();
                Assert.NotNull(actualSubscriptions);
                return actualSubscriptions;
            }

            protected TResponse CreateGetResponse<TResponse>(string contentType)
                where TResponse : TableauServerResponse
            {
                AutoFixture.Customize<ISubscriptionContentType>(
                    composer => composer.With(j => j.Type, () => contentType));

                return AutoFixture.CreateResponse<TResponse>();
            }
        }

        public class ForServer : SubscriptionsApiClientTest
        {
            [Theory]
            [EnumData<TableauInstanceType>(TableauInstanceType.Server)]
            public void Fails_when_current_instance_is_not_server(TableauInstanceType instanceType)
            {
                CurrentInstanceType = instanceType;

                var exception = Assert.Throws<TableauInstanceTypeNotSupportedException>(SubscriptionsApiClient.ForServer);

                Assert.Equal(instanceType, exception.UnsupportedInstanceType);

                MockSessionProvider.VerifyGet(p => p.InstanceType, Times.Once);
            }

            [Fact]
            public void Returns_client_when_current_instance_is_server()
            {
                CurrentInstanceType = TableauInstanceType.Server;

                var client = SubscriptionsApiClient.ForServer();

                MockSessionProvider.VerifyGet(p => p.InstanceType, Times.Once);
            }
        }

        public class ForCloud : SubscriptionsApiClientTest
        {
            [Theory]
            [EnumData<TableauInstanceType>(TableauInstanceType.Cloud)]
            public void Fails_when_current_instance_is_not_cloud(TableauInstanceType instanceType)
            {
                CurrentInstanceType = instanceType;

                var exception = Assert.Throws<TableauInstanceTypeNotSupportedException>(SubscriptionsApiClient.ForCloud);

                Assert.Equal(instanceType, exception.UnsupportedInstanceType);

                MockSessionProvider.VerifyGet(p => p.InstanceType, Times.Once);
            }

            [Fact]
            public void Returns_client_when_current_instance_is_cloud()
            {
                CurrentInstanceType = TableauInstanceType.Cloud;

                var client = SubscriptionsApiClient.ForCloud();

                MockSessionProvider.VerifyGet(p => p.InstanceType, Times.Once);
            }
        }

        #region - Cloud -

        public class Cloud
        {
            public abstract class CloudSubscriptionsApiClientTest : SubscriptionsApiClientTest
            {
                internal ICloudSubscriptionsApiClient CloudSubscriptionsApiClient => SubscriptionsApiClient;

                public CloudSubscriptionsApiClientTest()
                {
                    CurrentInstanceType = TableauInstanceType.Cloud;
                }

                protected CloudResponses.GetSubscriptionsResponse CreateCloudResponse(string contentType)
                {
                    AutoFixture.Customize<CloudResponses.GetSubscriptionsResponse.SubscriptionType.ContentType>(
                             composer => composer.With(j => j.Type, () => contentType));

                    return AutoFixture.CreateResponse<CloudResponses.GetSubscriptionsResponse>();
                }
            }

            #region - GetAllSubscriptionsAsync -

            public class GetAllSubscriptionsAsync : CloudSubscriptionsApiClientTest
            {
                [Fact]
                public async Task Gets_view_subscriptions()
                {
                    var response = CreateGetResponse<CloudResponses.GetSubscriptionsResponse>("view");

                    SetupSuccessResponse(response);

                    var result = await CloudSubscriptionsApiClient.GetAllSubscriptionsAsync(0, 100, Cancel);

                    var actualSubscriptions = AssertSuccess<ICloudSubscription, ICloudSchedule>(result);
                    var expectedSubscriptions = response.Items.ToList();

                    Assert.Equal(expectedSubscriptions.Count, actualSubscriptions.Count);
                }

                [Fact]
                public async Task Gets_workbook_subscriptions()
                {
                    var response = CreateGetResponse<CloudResponses.GetSubscriptionsResponse>("workbook");

                    SetupSuccessResponse(response);

                    var result = await CloudSubscriptionsApiClient.GetAllSubscriptionsAsync(0, 100, Cancel);

                    var actualSubscriptions = AssertSuccess<ICloudSubscription, ICloudSchedule>(result);
                    var expectedSubscriptions = response.Items.ToList();

                    Assert.Equal(expectedSubscriptions.Count, actualSubscriptions.Count);
                }
            }

            #endregion

            #region - CreateSubscriptionAsync -

            public sealed class CreateSubscriptionAsync : CloudSubscriptionsApiClientTest
            {
                [Fact]
                public async Task CreatesSubscriptionAsync()
                {
                    SetupSuccessResponse<CloudResponses.CreateSubscriptionResponse>();

                    var sub = Create<ICloudSubscription>();

                    var result = await CloudSubscriptionsApiClient.CreateSubscriptionAsync(sub, Cancel);

                    result.AssertSuccess();
                }

                [Fact]
                public async Task ReturnsFailureAsync()
                {
                    SetupErrorResponse<CloudResponses.CreateSubscriptionResponse>();

                    var sub = Create<ICloudSubscription>();

                    var result = await CloudSubscriptionsApiClient.CreateSubscriptionAsync(sub, Cancel);

                    result.AssertFailure();
                }
            }

            #endregion

            #region - UpdateSubscriptionAsync -

            public sealed class UpdateSubscriptionAsync : CloudSubscriptionsApiClientTest
            {
                [Fact]
                public async Task UpdatesSubscriptionAsync()
                {
                    SetupSuccessResponse<CloudResponses.UpdateSubscriptionResponse>();

                    var sub = Create<ICloudSubscription>();

                    var result = await CloudSubscriptionsApiClient.UpdateSubscriptionAsync(Guid.NewGuid(), Cancel);

                    result.AssertSuccess();
                }

                [Fact]
                public async Task ReturnsFailureAsync()
                {
                    SetupErrorResponse<CloudResponses.UpdateSubscriptionResponse>();

                    var sub = Create<ICloudSubscription>();

                    var result = await CloudSubscriptionsApiClient.UpdateSubscriptionAsync(Guid.NewGuid(), Cancel);

                    result.AssertFailure();
                }
            }

            #endregion

            #region - DeleteAsync -
            public sealed class DeleteAsync : CloudSubscriptionsApiClientTest
            {
                [Fact]
                public async Task DeletesSubscriptionAsync()
                {
                    SetupSuccessResponse();

                    var result = await CloudSubscriptionsApiClient.DeleteAsync(Guid.NewGuid(), Cancel);

                    result.AssertSuccess();
                }

                [Fact]
                public async Task ReturnsFailureAsync()
                {
                    SetupErrorResponse();

                    var result = await CloudSubscriptionsApiClient.DeleteAsync(Guid.NewGuid(), Cancel);

                    result.AssertFailure();
                }
            }
            #endregion
        }

        #endregion

        #region - Server -

        public class Server
        {
            public abstract class ServerSubscriptionsApiClientTest : SubscriptionsApiClientTest
            {
                internal IServerSubscriptionsApiClient ServerSubscriptionsApiClient => SubscriptionsApiClient;

                public ServerSubscriptionsApiClientTest()
                {
                    CurrentInstanceType = TableauInstanceType.Server;
                }
            }

            #region - GetAllSubscriptionsAsync -

            public class GetAllSubscriptionsAsync : ServerSubscriptionsApiClientTest
            {
                [Fact]
                public async Task Gets_view_subscriptions()
                {
                    MockSessionProvider.SetupGet(p => p.InstanceType).Returns(TableauInstanceType.Server);

                    var response = CreateGetResponse<ServerResponses.GetSubscriptionsResponse>("view");

                    SetupSuccessResponse(response);

                    var result = await ServerSubscriptionsApiClient.GetAllSubscriptionsAsync(0, 100, Cancel);

                    var actualSubscriptions = AssertSuccess<IServerSubscription, IServerSchedule>(result);
                    var expectedSubscriptions = response.Items.ToList();

                    Assert.Equal(expectedSubscriptions.Count, actualSubscriptions.Count);
                }

                [Fact]
                public async Task Gets_workbook_subscriptions()
                {
                    var response = CreateGetResponse<ServerResponses.GetSubscriptionsResponse>("workbook");

                    SetupSuccessResponse(response);

                    var result = await ServerSubscriptionsApiClient.GetAllSubscriptionsAsync(0, 100, Cancel);

                    var actualSubscriptions = AssertSuccess<IServerSubscription, IServerSchedule>(result);
                    var expectedSubscriptions = response.Items.ToList();

                    Assert.Equal(expectedSubscriptions.Count, actualSubscriptions.Count);
                }
            }

            #endregion      
        }

        #endregion
    }
}
