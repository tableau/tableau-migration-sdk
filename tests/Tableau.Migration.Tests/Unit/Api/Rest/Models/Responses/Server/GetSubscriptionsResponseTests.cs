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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Responses.Server
{
    public class GetSubscriptionsResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void DeSerializes()
            {
                var expectedResponse = Create<GetSubscriptionsResponse>();

                var expectedSubscriptions = expectedResponse.Items.ToList();
                var xmlBuilder = new StringBuilder();

                var xml = BuildInputXml(expectedSubscriptions, xmlBuilder);

                var deserialized = Serializer.DeserializeFromXml<GetSubscriptionsResponse>(xml);
                Assert.NotNull(deserialized);
                var actualSubscriptions = deserialized.Items;
                Assert.Equal(expectedSubscriptions.Count, actualSubscriptions.Length);

                foreach (var expectedSubscription in expectedSubscriptions)
                {
                    AssertSubscription(expectedSubscription, actualSubscriptions.FirstOrDefault(x => x.Id == expectedSubscription.Id));
                }

                static void AssertSubscription(GetSubscriptionsResponse.SubscriptionType? expectedSubscription, GetSubscriptionsResponse.SubscriptionType? actualSubscription)
                {
                    Assert.NotNull(expectedSubscription);
                    Assert.NotNull(actualSubscription);

                    Assert.Equal(expectedSubscription.Id, actualSubscription.Id);
                    Assert.Equal(expectedSubscription.Subject, actualSubscription.Subject);
                    Assert.Equal(expectedSubscription.AttachImage, actualSubscription.AttachImage);
                    Assert.Equal(expectedSubscription.AttachPdf, actualSubscription.AttachPdf);

                    AssertContent(expectedSubscription.Content, actualSubscription.Content);

                    AssertSchedule(expectedSubscription.Schedule, actualSubscription.Schedule);

                    AssertUser(expectedSubscription.User, actualSubscription.User);
                }

                static void AssertSchedule(GetSubscriptionsResponse.SubscriptionType.ScheduleType? expected, GetSubscriptionsResponse.SubscriptionType.ScheduleType? actual)
                {
                    Assert.NotNull(actual);
                    Assert.Equal(expected?.Id, actual?.Id);
                    Assert.Equal(expected?.Name, actual?.Name);
                }

                static void AssertUser(GetSubscriptionsResponse.SubscriptionType.UserType? expected, GetSubscriptionsResponse.SubscriptionType.UserType? actual)
                {
                    Assert.NotNull(actual);
                    Assert.Equal(expected?.Id, actual?.Id);
                    Assert.Equal(expected?.Name, actual?.Name);
                }

                static void AssertContent(GetSubscriptionsResponse.SubscriptionType.ContentType? expected, GetSubscriptionsResponse.SubscriptionType.ContentType? actual)
                {
                    Assert.NotNull(actual);
                    Assert.Equal(expected?.Id, actual.Id);
                    Assert.Equal(expected?.Type, actual.Type);
                    Assert.Equal(expected?.SendIfViewEmpty, actual.SendIfViewEmpty);
                }
            }

            private static string BuildInputXml(List<GetSubscriptionsResponse.SubscriptionType> expectedSubscriptions, StringBuilder xmlBuilder)
            {
                AppendTsResponseStartElement(xmlBuilder);
                AppendPaginationElement(xmlBuilder, 1, 100, expectedSubscriptions.Count);
                xmlBuilder.AppendLine(@"    <subscriptions>");
                foreach (var item in expectedSubscriptions)
                {
                    xmlBuilder.Append($@"
        <subscription id=""{item.Id}"" subject=""{item.Subject}"" attachImage=""{item.AttachImage.ToString().ToLower()}"" attachPdf=""{item.AttachPdf.ToString().ToLower()}"" suspended=""{item.Suspended.ToString().ToLower()}"">
            <content id=""{item.Content?.Id}"" type=""{item.Content?.Type}"" sendIfViewEmpty=""{item.Content?.SendIfViewEmpty.ToString().ToLower()}""/>
            <schedule id=""{item.Schedule?.Id}"" name=""{item.Schedule?.Name}""/>
            <user id=""{item.User?.Id}"" name=""{item.User?.Name}""/>
        </subscription>
                    ");
                }

                xmlBuilder.AppendLine(@"    </subscriptions>");
                AppendTsResponseEndElement(xmlBuilder);
                return xmlBuilder.ToString();
            }

            private static void AppendPaginationElement(StringBuilder builder, int pageNumber, int pageSize, int totalAvailable)
                => builder.AppendLine(@$"    <pagination pageNumber=""{pageNumber}"" pageSize=""{pageSize}"" totalAvailable=""{totalAvailable}""/>");


            private static void AppendTsResponseEndElement(StringBuilder builder)
                => builder.AppendLine(@$"</{TableauServerResponse.XmlTypeName}>");

            private static void AppendTsResponseStartElement(StringBuilder builder)
                => builder.Append($"<{TableauServerResponse.XmlTypeName} xmlns=\"http://tableau.com/api\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://tableau.com/api https://help.tableau.com/samples/en-us/rest_api/ts-api_3_24.xsd\">");

        }
    }
}
