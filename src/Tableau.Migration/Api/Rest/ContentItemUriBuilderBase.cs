//
//  Copyright (c) 2024, Salesforce, Inc.
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
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Api.Rest
{
    /// <summary>
    /// Class representing a URI builder for content items in a general way.
    /// Examples include permissions and tags.
    /// </summary>
    internal class ContentItemUriBuilderBase
    {
        public string Prefix { get; }

        public string Suffix { get; }

        public ContentItemUriBuilderBase(string prefix, string suffix)
        {
            Prefix = Guard.AgainstNullEmptyOrWhiteSpace(prefix, nameof(prefix));
            Suffix = Guard.AgainstNullEmptyOrWhiteSpace(suffix, nameof(suffix));
        }

        public virtual string BuildUri(Guid contentItemId)
            => $"{Prefix}/{contentItemId.ToUrlSegment()}/{Suffix}";
    }
}
