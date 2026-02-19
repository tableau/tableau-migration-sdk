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

using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Content
{
    internal class SubscriptionContent : ContentBase, ISubscriptionContent
    {
        /// <inheritdoc />
        public string Type { get; set; }

        /// <inheritdoc />
        public bool SendIfViewEmpty { get; set; }

        public SubscriptionContent(IContentReference contentReference, string type, bool sendIfViewEmpty)
            : base(contentReference)
        {
            Type = type;
            SendIfViewEmpty = sendIfViewEmpty;
        }

        public SubscriptionContent(IContentReference contentReference, ISubscriptionContentType response)
            : this(contentReference, Guard.AgainstNull(response.Type, () => response.Type), response.SendIfViewEmpty)
        { }

        public SubscriptionContent(ISubscriptionContent content)
            : this(content, content.Type, content.SendIfViewEmpty)
        { }

        /// <inheritdoc />
        public ISubscriptionContent ForReference(IContentReference reference)
            => new SubscriptionContent(reference, Type, SendIfViewEmpty);
    }
}