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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client server subscriptions operations.
    /// </summary>
    public interface ICloudSubscriptionsApiClient : IPublishApiClient<ICloudSubscription>, IDeleteApiClient
    {
        /// <summary>
        /// Gets all subscriptions on the cloud site.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The list of subscriptions on the site.</returns>
        Task<IPagedResult<ICloudSubscription>> GetAllSubscriptionsAsync(int pageNumber, int pageSize, CancellationToken cancel);

        /// <summary>
        /// Creates a new subscription on the cloud site.
        /// </summary>
        /// <param name="subscription">The subscription to create.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The newly created cloud subscription.</returns>
        Task<IResult<ICloudSubscription>> CreateSubscriptionAsync(ICloudSubscription subscription, CancellationToken cancel);

        /// <summary>
        /// Updates a subscription on the cloud site.
        /// </summary>
        /// <param name="subscriptionId">The ID for the subscription to update.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <param name="newSubject">The new subject, or null to not update the subject.</param>
        /// <param name="newAttachImage">The new attach image flag, or null to not update the flag.</param>
        /// <param name="newAttachPdf">The new attach PDF flag, or null to not update the flag.</param>
        /// <param name="newPageOrientation">The new page orientation, or null to not update the page orientation.</param>
        /// <param name="newPageSizeOption">The new page size option, or null to not update the page size option.</param>
        /// <param name="newSuspended">The new suspended flag, or null to not update the flag.</param>
        /// <param name="newMessage">The new message, or null to not update the message.</param>
        /// <param name="newContent">The new content reference, or null to not update the content reference.</param>
        /// <param name="newUserId">The new user ID, or null to not update the user ID.</param>
        /// <param name="newSchedule">The new schedule, or null to not update the schedule.</param>
        /// <returns>The updated cloud subscription.</returns>
        Task<IResult<ICloudSubscription>> UpdateSubscriptionAsync(Guid subscriptionId, CancellationToken cancel,
            string? newSubject = null,
            bool? newAttachImage = null,
            bool? newAttachPdf = null,
            string? newPageOrientation = null,
            string? newPageSizeOption = null,
            bool? newSuspended = null,
            string? newMessage = null,
            ISubscriptionContent? newContent = null,
            Guid? newUserId = null,
            ICloudSchedule? newSchedule = null);
    }
}