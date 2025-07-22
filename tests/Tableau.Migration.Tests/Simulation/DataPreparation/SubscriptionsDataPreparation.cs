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
using System.Collections.Immutable;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Api.Simulation;

namespace Tableau.Migration.Tests.Simulation.DataPreparation
{
    /// <summary>
    /// Static class responsible for preparing subscriptions data for migration tests.
    /// </summary>
    public static class SubscriptionsDataPreparation
    {
        /// <summary>
        /// Prepares the source data for migration tests.
        /// </summary>
        /// <param name="sourceApi">The source API simulator.</param>
        /// <param name="fixture">The fixture for creating test data.</param>
        /// <returns>The list of prepared subscriptions.</returns>
        public static IImmutableList<GetSubscriptionsResponse.SubscriptionType> PrepareServerSource(
            TableauApiSimulator sourceApi,
            IFixture fixture)
        {
            ArgumentNullException.ThrowIfNull(sourceApi);
            ArgumentNullException.ThrowIfNull(fixture);

            var subscriptions = ImmutableArray.CreateBuilder<GetSubscriptionsResponse.SubscriptionType>();
            var schedules = SchedulesDataPreparation.PrepareServerSource(sourceApi);

            foreach (var workbook in sourceApi.Data.Workbooks)
            {
                var workbookSubscription = fixture.Create<GetSubscriptionsResponse.SubscriptionType>();

                var user = sourceApi.Data.Users.PickRandom();
                workbookSubscription.User = new() { Id = user.Id, Name = user.Name };
                workbookSubscription.Content = new() { Id = workbook.Id, Type = "workbook" };
                subscriptions.Add(workbookSubscription);

                var schedule = sourceApi.Data.Schedules.PickRandom();
                workbookSubscription.Schedule = new() { Id = schedule.Id, Name = schedule.Name };

                foreach (var view in workbook.Views)
                {
                    var viewSubscription = fixture.Create<GetSubscriptionsResponse.SubscriptionType>();

                    user = sourceApi.Data.Users.PickRandom();
                    viewSubscription.User = new() { Id = user.Id, Name = user.Name };
                    viewSubscription.Content = new() { Id = view.Id, Type = "view" };

                    schedule = sourceApi.Data.Schedules.PickRandom();
                    viewSubscription.Schedule = new() { Id = schedule.Id, Name = schedule.Name };

                    subscriptions.Add(viewSubscription);
                }
            }

            foreach (var subscription in subscriptions)
            {
                sourceApi.Data.ServerSubscriptions.Add(subscription);
            }

            return subscriptions.ToImmutable();
        }
    }
}