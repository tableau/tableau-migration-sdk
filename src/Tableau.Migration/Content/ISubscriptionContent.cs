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

namespace Tableau.Migration.Content
{
    /// <summary>
    /// The content of the subscription.
    /// </summary>
    public interface ISubscriptionContent : IContentReference
    {
        /// <summary>
        /// The content type of the subscription.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Whether or not send the notification if the view is empty.
        /// </summary>
        public bool SendIfViewEmpty { get; set; }

        /// <summary>
        /// Creates a new <see cref="ISubscriptionContent"/> object of the same type for the new content reference.
        /// </summary>
        /// <param name="reference">The new content reference.</param>
        /// <returns>The created <see cref="ISubscriptionContent"/>.</returns>
        ISubscriptionContent ForReference(IContentReference reference);
    }
}