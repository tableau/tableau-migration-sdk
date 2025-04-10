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
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;
using CloudResponse = Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using ServerResponse = Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API subscription methods.
    /// </summary>
    public sealed class SubscriptionsRestApiSimulator
    {
        /// <summary>
        /// Gets the simulated subscription list API method.
        /// </summary>
        public MethodSimulator ListSubscriptions { get; }

        /// <summary>
        /// Gets the simulated subscription create API method.
        /// </summary>
        public MethodSimulator CreateSubscription { get; }

        /// <summary>
        /// Gets the simulated subscription update API method.
        /// </summary>
        public MethodSimulator UpdateSubscription { get; }

        /// <summary>
        /// Gets the simulated subscription delete API method.
        /// </summary>
        public MethodSimulator DeleteSubscription { get; }

        /// <summary>
        /// Creates a new <see cref="SubscriptionsRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public SubscriptionsRestApiSimulator(TableauApiResponseSimulator simulator)
        {
            var subscriptionsUrl = SiteUrl(RestUrlPrefixes.Subscriptions);

            ListSubscriptions =
                simulator.Data.IsTableauServer
                    ? simulator.SetupRestGetList<ServerResponse.GetSubscriptionsResponse, ServerResponse.GetSubscriptionsResponse.SubscriptionType>(
                        subscriptionsUrl, (d, r) => d.ServerSubscriptions, null, true)
                    : simulator.SetupRestGetList<CloudResponse.GetSubscriptionsResponse, CloudResponse.GetSubscriptionsResponse.SubscriptionType>(
                        subscriptionsUrl, (d, r) => d.CloudSubscriptions, null, true);

            CreateSubscription = simulator.SetupRestPost(subscriptionsUrl,
                new RestCreateSubscriptionResponseBuilder(simulator.Data, simulator.Serializer));

            UpdateSubscription = simulator.SetupRestPut(EntityUrl(RestUrlPrefixes.Subscriptions),
                new RestUpdateSubscriptionResponseBuilder(simulator.Data, simulator.Serializer));


            DeleteSubscription = simulator.SetupRestDelete(SiteEntityUrl(RestUrlPrefixes.Subscriptions), BuildDeleteResponseBuilder(simulator));
        }

        private static RestDeleteResponseBuilder BuildDeleteResponseBuilder(TableauApiResponseSimulator simulator)
        {
            return simulator.Data.IsTableauServer ?
                new RestDeleteResponseBuilder(simulator.Data,
                    (data, request) =>
                    {
                        var id = request.GetRequestIdFromUri();
                        var serverSub = data.ServerSubscriptions.SingleOrDefault(s => s.Id == id);
                        if (serverSub != null)
                        {
                            data.ServerSubscriptions.Remove(serverSub);
                            return HttpStatusCode.OK;
                        }
                        return HttpStatusCode.NotFound;
                    }, simulator.Serializer)
                : new RestDeleteResponseBuilder(simulator.Data,
                    (data, request) =>
                    {
                        var id = request.GetRequestIdFromUri();
                        var cloudSub = data.CloudSubscriptions.SingleOrDefault(s => s.Id == id);
                        if (cloudSub != null)
                        {
                            data.CloudSubscriptions.Remove(cloudSub);
                            return HttpStatusCode.OK;
                        }
                        return HttpStatusCode.NotFound;
                    }, simulator.Serializer);
        }
    }
}
