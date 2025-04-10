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

using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Responses.Cloud
{
    public sealed class UpdateSubscriptionResponseTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void InitializesSubscription()
            {
                var s = Create<ISubscriptionType>();

                var r = new UpdateSubscriptionResponse.SubscriptionType(s);

                Assert.Equal(s.Id, r.Id);
                Assert.Equal(s.Subject, r.Subject);
                Assert.Equal(s.AttachImage, r.AttachImage);
                Assert.Equal(s.AttachPdf, r.AttachPdf);
                Assert.Equal(s.PageOrientation, r.PageOrientation);
                Assert.Equal(s.PageSizeOption, r.PageSizeOption);
                Assert.Equal(s.Suspended, r.Suspended);
                Assert.Equal(s.Message, r.Message);

                Assert.NotNull(r.Content);
                Assert.Equal(s.Content!.Id, r.Content.Id);
                Assert.Equal(s.Content.Type, r.Content.Type);
                Assert.Equal(s.Content.SendIfViewEmpty, r.Content.SendIfViewEmpty);

                Assert.NotNull(r.User);
                Assert.Equal(s.User!.Id, r.User.Id);
            }

            [Fact]
            public void InitializesSchedule()
            {
                var s = Create<ICloudScheduleType>();

                var r = new UpdateSubscriptionResponse.ScheduleType(s);

                Assert.Equal(s.Frequency, r.Frequency);

                Assert.NotNull(r.FrequencyDetails);
                Assert.Equal(s.FrequencyDetails!.Start, r.FrequencyDetails.Start);
                Assert.Equal(s.FrequencyDetails.End, r.FrequencyDetails.End);
                Assert.Equal(s.FrequencyDetails.Intervals.Length, r.FrequencyDetails.Intervals.Length);
            }
        }
    }
}
