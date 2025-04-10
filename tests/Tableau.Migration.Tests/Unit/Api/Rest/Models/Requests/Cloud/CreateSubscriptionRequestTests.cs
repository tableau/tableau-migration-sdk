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

using System.Linq;
using Tableau.Migration.Api.Models.Cloud;
using Tableau.Migration.Api.Rest.Models.Requests.Cloud;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests.Cloud
{
    public sealed class CreateSubscriptionRequestTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var opts = Create<ICreateSubscriptionOptions>();

                var request = new CreateSubscriptionRequest(opts);

                Assert.NotNull(request.Subscription);
                Assert.Equal(opts.Subject, request.Subscription.Subject);
                Assert.Equal(opts.AttachImage, request.Subscription.AttachImage);
                Assert.Equal(opts.AttachPdf, request.Subscription.AttachPdf);
                Assert.Equal(opts.PageSizeOption, request.Subscription.PageSizeOption);
                Assert.Equal(opts.PageOrientation, request.Subscription.PageOrientation);
                Assert.Equal(opts.Message, request.Subscription.Message);

                Assert.NotNull(request.Subscription.Content);
                Assert.Equal(opts.Content.Id, request.Subscription.Content.Id);
                Assert.Equal(opts.Content.Type, request.Subscription.Content.Type);
                Assert.Equal(opts.Content.SendIfViewEmpty, request.Subscription.Content.SendIfViewEmpty);

                Assert.NotNull(request.Subscription.User);
                Assert.Equal(opts.UserId, request.Subscription.User.Id);

                Assert.NotNull(request.Schedule);
                Assert.Equal(opts.Schedule.Frequency, request.Schedule.Frequency);

                Assert.NotNull(request.Schedule.FrequencyDetails);
                Assert.Equal(opts.Schedule.FrequencyDetails.StartAt?.ToString(Constants.FrequencyTimeFormat), request.Schedule.FrequencyDetails.Start);
                Assert.Equal(opts.Schedule.FrequencyDetails.EndAt?.ToString(Constants.FrequencyTimeFormat), request.Schedule.FrequencyDetails.End);

                Assert.Equal(opts.Schedule.FrequencyDetails.Intervals.Count, request.Schedule.FrequencyDetails.Intervals.Count());
            }
        }
    }
}
