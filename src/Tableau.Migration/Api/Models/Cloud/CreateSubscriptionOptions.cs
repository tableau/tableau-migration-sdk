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
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;

namespace Tableau.Migration.Api.Models.Cloud
{
    /// <summary>
    /// Default <see cref="ICreateSubscriptionOptions"/> implementation.
    /// </summary>
    public class CreateSubscriptionOptions : ICreateSubscriptionOptions
    {
        /// <inheritdoc />
        public string Subject { get; }

        /// <inheritdoc />
        public bool AttachImage { get; }

        /// <inheritdoc />
        public bool AttachPdf { get; }

        /// <inheritdoc />
        public string PageOrientation { get; }

        /// <inheritdoc />
        public string PageSizeOption { get; }

        /// <inheritdoc />
        public string? Message { get; }

        /// <inheritdoc />
        public ISubscriptionContent Content { get; }

        /// <inheritdoc />
        public Guid UserId { get; }

        /// <inheritdoc />
        public ICloudSchedule Schedule { get; }

        /// <summary>
        /// Creates a new <see cref="CreateSubscriptionOptions"/> object.
        /// </summary>
        /// <param name="subscription">The subcription to create.</param>
        public CreateSubscriptionOptions(ICloudSubscription subscription)
        {
            Subject = subscription.Subject;
            AttachImage = subscription.AttachImage;
            AttachPdf = subscription.AttachPdf;
            PageOrientation = subscription.PageOrientation;
            PageSizeOption = subscription.PageSizeOption;
            Message = subscription.Message;
            Content = subscription.Content;
            UserId = subscription.Owner.Id;
            Schedule = subscription.Schedule;
        }
    }
}
