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

using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public sealed class SubscriptionsApiClientTests
    {
        public abstract class SubscriptionsApiClientTest : ApiClientTestBase
        {
            public SubscriptionsApiClientTest(bool isCloud = false)
                : base(isCloud)
            { }
        }

        #region  - Cloud -

        #region - CreateSubscriptionAsync -

        public sealed class CreateSubscriptionAsyncTests : SubscriptionsApiClientTest
        {
            public CreateSubscriptionAsyncTests()
                : base(true)
            { }

            [Fact]
            public async Task CreateWorkbookSubscriptionAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var wb = Api.Data.CreateWorkbook(AutoFixture);

                var wbRef = new ContentReferenceStub(wb.Id, wb.ContentUrl!, new([wb.Name!]));

                var subscription = Create<ICloudSubscription>();
                subscription.Content = new SubscriptionContent(wbRef, "Workbook", false);
                subscription.Owner = new ContentReferenceStub(Api.Data.Users.First().Id, "", new());

                var result = await sitesClient.CloudSubscriptions.CreateSubscriptionAsync(subscription, Cancel);

                result.AssertSuccess();

                var createdSub = Api.Data.CloudSubscriptions.SingleOrDefault();
                Assert.NotNull(createdSub);
                Assert.Equal(createdSub.Id, result.Value!.Id);
            }

            [Fact]
            public async Task CreateWorkbookSubscriptionInvalidWorkbookAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var subscription = Create<ICloudSubscription>();
                subscription.Content = new SubscriptionContent(Create<IContentReference>(), "Workbook", false);
                subscription.Owner = new ContentReferenceStub(Api.Data.Users.First().Id, "", new());

                var result = await sitesClient.CloudSubscriptions.CreateSubscriptionAsync(subscription, Cancel);

                result.AssertFailure();
            }

            [Fact]
            public async Task CreateViewSubscriptionAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var wb = Api.Data.CreateWorkbook(AutoFixture);

                var view = wb.Views.First();

                var viewRef = new ContentReferenceStub(view.Id, view.ContentUrl!, new([view.Name!]));

                var subscription = Create<ICloudSubscription>();
                subscription.Content = new SubscriptionContent(viewRef, "View", false);
                subscription.Owner = new ContentReferenceStub(Api.Data.Users.First().Id, "", new());

                var result = await sitesClient.CloudSubscriptions.CreateSubscriptionAsync(subscription, Cancel);

                result.AssertSuccess();

                var createdSub = Api.Data.CloudSubscriptions.SingleOrDefault();
                Assert.NotNull(createdSub);
                Assert.Equal(createdSub.Id, result.Value!.Id);
            }

            [Fact]
            public async Task CreateViewSubscriptionInvalidViewAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var subscription = Create<ICloudSubscription>();
                subscription.Content = new SubscriptionContent(Create<IContentReference>(), "View", false);
                subscription.Owner = new ContentReferenceStub(Api.Data.Users.First().Id, "", new());

                var result = await sitesClient.CloudSubscriptions.CreateSubscriptionAsync(subscription, Cancel);

                result.AssertFailure();
            }
        }

        #endregion

        #region - UpdateSubscriptionAsync -

        public sealed class UpdateSubscriptionAsyncTests : SubscriptionsApiClientTest
        {
            public UpdateSubscriptionAsyncTests()
                : base(true)
            { }

            public async Task UpdatesSubscriptionAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var sub = Create<GetSubscriptionsResponse.SubscriptionType>();
                Api.Data.CloudSubscriptions.Add(sub);

                var result = await sitesClient.CloudSubscriptions.UpdateSubscriptionAsync(sub.Id, Cancel, newSuspended: true);

                result.AssertSuccess();

                Assert.Equal(sub.Id, result.Value!.Id);
            }
        }

        #endregion

        #endregion
    }
}
