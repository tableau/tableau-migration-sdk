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

using Tableau.Migration.Content.Schedules;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a subscription.
    /// </summary>
    public interface ISubscription<TSchedule> : IWithSchedule<TSchedule>, IWithOwner
        where TSchedule : ISchedule
    {
        /// <summary>
        /// Gets or sets the subject of the subscription.
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// Gets or sets whether or not an image file should be attached to the notification.
        /// </summary>
        bool AttachImage { get; set; }

        /// <summary>
        /// Gets or sets whether or not a pdf file should be attached to the notification.
        /// </summary>
        bool AttachPdf { get; set; }

        /// <summary>
        /// Gets or set the page orientation of the subscription.
        /// </summary>
        string PageOrientation { get; set; }

        /// <summary>
        /// Gets or set the page page size option of the subscription.
        /// </summary>
        string PageSizeOption { get; set; }

        /// <summary>
        /// Gets or sets whether or not the subscription is suspended.
        /// </summary>
        bool Suspended { get; set; }

        /// <summary>
        /// Gets or sets the message of the subscription.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Gets or set the content reference of the subscription.
        /// </summary>
        ISubscriptionContent Content { get; set; }
    }
}