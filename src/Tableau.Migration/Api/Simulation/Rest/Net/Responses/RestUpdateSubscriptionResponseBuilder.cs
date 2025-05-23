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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests.Cloud;
using Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal sealed class RestUpdateSubscriptionResponseBuilder : RestResponseBuilderBase<UpdateSubscriptionResponse>
    {
        public RestUpdateSubscriptionResponseBuilder(TableauData data, IHttpContentSerializer serializer)
            : base(data, serializer, requiresAuthentication: true)
        { }

        protected override ValueTask<(UpdateSubscriptionResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(
            HttpRequestMessage request,
            CancellationToken cancel)
        {
            var id = request.GetIdAfterSegment(RestUrlKeywords.Subscriptions);
            if (id is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "Invalid subscription ID.", string.Empty);
            }

            var subscription = Data.CloudSubscriptions.SingleOrDefault(s => s.Id == id);
            if (subscription is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, 025, "Subscription not found.", string.Empty);
            }

            var update = request.GetTableauServerRequest<UpdateSubscriptionRequest>();
            if (update is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "Invalid request.", string.Empty);
            }

            if (update.Subscription is not null)
            {
                if (update.Subscription.Subject is not null)
                {
                    subscription.Subject = update.Subscription.Subject;
                }

                if (update.Subscription.AttachImageSpecified)
                {
                    subscription.AttachImage = update.Subscription.AttachImage;
                }

                if (update.Subscription.AttachPdfSpecified)
                {
                    subscription.AttachPdf = update.Subscription.AttachPdf;
                }

                if (update.Subscription.PageOrientation is not null)
                {
                    subscription.PageOrientation = update.Subscription.PageOrientation;
                }

                if (update.Subscription.PageSizeOption is not null)
                {
                    subscription.PageSizeOption = update.Subscription.PageSizeOption;
                }

                if (update.Subscription.SuspendedSpecified)
                {
                    subscription.Suspended = update.Subscription.Suspended;
                }

                if (update.Subscription.Message is not null)
                {
                    subscription.Message = update.Subscription.Message;
                }

                if (update.Subscription.Content is not null)
                {
                    subscription.Content = new(update.Subscription.Content);
                }

                if (update.Subscription.User is not null)
                {
                    var user = Data.Users.SingleOrDefault(u => u.Id == update.Subscription.User.Id);
                    if (user is null)
                    {
                        return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, 002, $"{nameof(UpdateSubscriptionRequest.SubcriptionType.User)} not found.", string.Empty);
                    }

                    subscription.User = new() { Id = user.Id, Name = user.Name };
                }
            }

            if (update.Schedule is not null)
            {
                subscription.Schedule = new()
                {
                    Frequency = update.Schedule.Frequency,
                    FrequencyDetails = new(update.Schedule.FrequencyDetails!)
                };
            }

            var response = new UpdateSubscriptionResponse
            {
                Item = new(subscription),
                Schedule = new(subscription.Schedule!)
            };

            return ValueTask.FromResult((response, HttpStatusCode.OK));
        }
    }
}
