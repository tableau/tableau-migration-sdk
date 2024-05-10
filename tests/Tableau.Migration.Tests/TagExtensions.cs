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

using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;

namespace Tableau.Migration.Tests
{
    public static class TagExtensions
    {
        public static void AssertEqual(this ITag? tag, ITag? other)
            => TagLabelComparer.Instance.Equals(tag, other);

        public static void AssertEqual(this ITag? tag, ITagType? other)
            => TagLabelComparer.Instance.Equals(tag, other is not null ? new Tag(other) : null);

        public static void AssertEqual(this ITagType? tag, ITagType? other)
            => ITagTypeComparer.Instance.Equals(tag, other);

        public static void AssertEqual(this ITagType? tag, ITag? other)
            => AssertEqual(other is not null ? new Tag(other) : null, tag);

        public static void AssertEqual(this IEnumerable<ITag>? tags, IEnumerable<ITag>? others)
            => tags.SequenceEqual(others, t => t.Label);

        public static void AssertEqual(this IEnumerable<ITag>? tags, IEnumerable<ITagType>? others)
            => tags.AssertEqual(others?.Select(t => new Tag(t)));

        public static void AssertEqual(this IEnumerable<ITagType>? tags, IEnumerable<ITagType>? others)
            => tags.SequenceEqual(others, t => t.Label);

        public static void AssertEqual(this IEnumerable<ITagType>? tags, IEnumerable<ITag>? others)
            => AssertEqual(others?.Select(t => new Tag(t)), tags);
    }
}
