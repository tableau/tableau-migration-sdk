//
//  Copyright (c) 2026, Salesforce, Inc.
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

using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Static class with extension methods for <see cref="ISubscriptionContentType"/> objects.
    /// </summary>
    public static class ISubscriptionContentTypeExtensions
    {
        /// <summary>
        /// Gets the referenced content type of the favorite.
        /// </summary>
        public static SubscriptionContentType GetContentType(this ISubscriptionContentType content)
        {
            return content.Type?.ToLower() switch
            {
                "workbook" => SubscriptionContentType.Workbook,
                "view" => SubscriptionContentType.View,
                _ => SubscriptionContentType.Unknown
            };
        }
    }
}
