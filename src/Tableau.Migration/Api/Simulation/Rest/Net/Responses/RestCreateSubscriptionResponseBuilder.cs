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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Requests.Cloud;
using Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal sealed class RestCreateSubscriptionResponseBuilder : RestResponseBuilderBase<CreateSubscriptionResponse>
    {
        public RestCreateSubscriptionResponseBuilder(TableauData data, IHttpContentSerializer serializer)
            : base(data, serializer, requiresAuthentication: true)
        { }

        protected override ValueTask<(CreateSubscriptionResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var createRequest = request.GetTableauServerRequest<CreateSubscriptionRequest>();
            if (createRequest is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "Content cannot be null.", string.Empty);
            }

            var subscription = createRequest.Subscription;
            if (subscription is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, $"{nameof(CreateSubscriptionRequest.Subscription)} cannot be null.", string.Empty);
            }
            if (subscription.Content is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, $"{nameof(CreateSubscriptionRequest.SubscriptionType.Content)} cannot be null.", string.Empty);
            }
            if (subscription.Content is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, $"{nameof(CreateSubscriptionRequest.SubscriptionType.Content)} cannot be null.", string.Empty);
            }
            if (subscription.User is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, $"{nameof(CreateSubscriptionRequest.SubscriptionType.User)} cannot be null.", string.Empty);
            }

            var schedule = createRequest.Schedule;
            if (schedule is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, $"{nameof(CreateSubscriptionRequest.Schedule)} cannot be null.", string.Empty);
            }
            if (schedule.FrequencyDetails is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest, 0, $"{nameof(CreateSubscriptionRequest.ScheduleType.FrequencyDetails)} cannot be null.", string.Empty);
            }

            var user = Data.Users.SingleOrDefault(u => u.Id == subscription.User.Id);
            if (user is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, 002, $"{nameof(CreateSubscriptionRequest.SubscriptionType.User)} not found.", string.Empty);
            }

            switch (subscription.Content.Type?.ToLower())
            {
                case "workbook":
                    var wb = Data.Workbooks.SingleOrDefault(w => w.Id == subscription.Content.Id);
                    if (wb is null)
                        return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, 006, $"Workbook not found.", string.Empty);
                    break;
                case "view":
                    var view = Data.Workbooks.SingleOrDefault(w => w.Views.Any(v => v.Id == subscription.Content.Id));
                    if (view is null)
                        return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, 011, $"View not found.", string.Empty);
                    break;
                default:
                    return BuildEmptyErrorResponseAsync(HttpStatusCode.BadGateway, 0, $"Invalid content type.", string.Empty);
            }

            var result = new GetSubscriptionsResponse.SubscriptionType
            {
                Id = Guid.NewGuid(),
                Subject = subscription.Subject,
                AttachImage = subscription.AttachImage,
                AttachPdf = subscription.AttachPdf,
                PageSizeOption = subscription.PageSizeOption,
                PageOrientation = subscription.PageOrientation,
                Suspended = false, //Created subscriptions never start as suspended.
                Message = subscription.Message,

                Content = new(subscription.Content),
                User = new() { Id = user.Id, Name = user.Name },
                Schedule = new()
                {
                    Frequency = schedule.Frequency,
                    NextRunAt = string.Empty,
                    FrequencyDetails = new(schedule.FrequencyDetails)
                }
            };

            Data.CloudSubscriptions.Add(result);

            return ValueTask.FromResult((new CreateSubscriptionResponse
            {
                Item = new(result, result.Schedule)
            }, HttpStatusCode.Created));
        }
    }
}
