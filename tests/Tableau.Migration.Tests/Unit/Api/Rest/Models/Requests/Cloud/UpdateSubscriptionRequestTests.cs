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
using Tableau.Migration.Api.Rest.Models.Requests.Cloud;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests.Cloud
{
    public sealed class UpdateSubscriptionRequestTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var subject = Create<string>();
                var attachImage = Create<bool>();
                var attachPdf = Create<bool>();
                var pageOrientation = Create<string>();
                var pageSizeOptions = Create<string>();
                var suspended = Create<bool>();
                var message = Create<string>();
                var content = Create<ISubscriptionContent>();
                var userId = Create<Guid>();
                var schedule = Create<ICloudSchedule>();

                var r = new UpdateSubscriptionRequest(subject, attachImage, attachPdf, pageOrientation, pageSizeOptions, suspended,
                    message, content, userId, schedule);

                Assert.NotNull(r.Subscription);
                Assert.Equal(subject, r.Subscription.Subject);
                Assert.Equal(attachImage, r.Subscription.AttachImage);
                Assert.Equal(attachPdf, r.Subscription.AttachPdf);
                Assert.Equal(pageOrientation, r.Subscription.PageOrientation);
                Assert.Equal(pageSizeOptions, r.Subscription.PageSizeOption);
                Assert.Equal(suspended, r.Subscription.Suspended);
                Assert.Equal(message, r.Subscription.Message);

                Assert.NotNull(r.Subscription.Content);
                Assert.Equal(content.Id, r.Subscription.Content.Id);
                Assert.Equal(content.Type, r.Subscription.Content.Type);
                Assert.Equal(content.SendIfViewEmpty, r.Subscription.Content.SendIfViewEmpty);

                Assert.NotNull(r.Subscription.User);
                Assert.Equal(userId, r.Subscription.User.Id);

                Assert.NotNull(r.Schedule);
                Assert.Equal(schedule.Frequency, r.Schedule.Frequency);

                Assert.NotNull(r.Schedule.FrequencyDetails);
                Assert.Equal(schedule.FrequencyDetails.StartAt?.ToString(Constants.FrequencyTimeFormat), r.Schedule.FrequencyDetails.Start);
                Assert.Equal(schedule.FrequencyDetails.EndAt?.ToString(Constants.FrequencyTimeFormat), r.Schedule.FrequencyDetails.End);
                Assert.Equal(schedule.FrequencyDetails.Intervals.Count, r.Schedule.FrequencyDetails.Intervals.Length);
            }

            [Fact]
            public void NullOptional()
            {
                var r = new UpdateSubscriptionRequest(null);

                Assert.NotNull(r.Subscription);
                Assert.Null(r.Subscription.Subject);
                Assert.False(r.Subscription.AttachImageSpecified);
                Assert.False(r.Subscription.AttachPdfSpecified);
                Assert.Null(r.Subscription.PageOrientation);
                Assert.Null(r.Subscription.PageSizeOption);
                Assert.False(r.Subscription.SuspendedSpecified);
                Assert.Null(r.Subscription.Message);
                Assert.Null(r.Subscription.Content);
                Assert.Null(r.Subscription.User);
                Assert.Null(r.Schedule);
            }
        }
    }
}
