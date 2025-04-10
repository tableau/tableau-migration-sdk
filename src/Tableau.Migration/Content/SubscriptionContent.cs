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
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Content
{
    internal class SubscriptionContent : ISubscriptionContent
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public bool SendIfViewEmpty { get; set; }

        public SubscriptionContent(Guid id, string type, bool sendIfViewEmpty)
        {
            Id = id;
            Type = type;
            SendIfViewEmpty = sendIfViewEmpty;
        }

        public SubscriptionContent(ISubscriptionContentType? response)
        {
            Guard.AgainstNull(response, () => response);
            Id = Guard.AgainstDefaultValue(response.Id, () => response.Id);
            Type = Guard.AgainstNull(response.Type, () => response.Type);
            SendIfViewEmpty = response.SendIfViewEmpty;
        }

        public SubscriptionContent(ISubscriptionContent content)
            : this(content.Id, content.Type, content.SendIfViewEmpty)
        { }
    }
}