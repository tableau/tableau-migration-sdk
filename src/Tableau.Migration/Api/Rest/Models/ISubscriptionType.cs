﻿//
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

using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for a subscription REST response.
    /// </summary>
    public interface ISubscriptionType : IRestIdentifiable
    {
        /// <summary>
        /// Gets the subject for the subscription. 
        /// </summary>
        string? Subject { get; }

        /// <summary>
        /// Gets the attach image flag for the subscription. 
        /// </summary>
        bool AttachImage { get; }

        /// <summary>
        /// Gets the attach pdf flag for the subscription. 
        /// </summary>
        bool AttachPdf { get; }

        /// <summary>
        /// Gets the page orientation of the subscription.
        /// </summary>
        string? PageOrientation { get; set; }

        /// <summary>
        /// Gets the page page size option of the subscription.
        /// </summary>
        string? PageSizeOption { get; set; }

        /// <summary>
        /// Gets the suspended flag for the subscription. 
        /// </summary>
        bool Suspended { get; }

        /// <summary>
        /// Gets the message for the subscription.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets the content reference for the subscription.
        /// </summary>
        ISubscriptionContentType? Content { get; }

        /// <summary>
        /// Gets the user for the subscription.
        /// </summary>
        IRestIdentifiable? User { get; }
    }
}
