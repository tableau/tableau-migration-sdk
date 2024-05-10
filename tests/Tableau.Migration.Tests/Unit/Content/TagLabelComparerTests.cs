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
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class TagLabelComparerTests
    {
        public class EqualsMethod : AutoFixtureTestBase
        {
            [Fact]
            public void LabelsEqual()
            {
                var label = Create<string>();

                var x = new Tag(label);
                var y = new Tag(label);

                Assert.True(TagLabelComparer.Instance.Equals(x, y));
            }

            [Fact]
            public void CaseSensitive()
            {
                var x = new Tag("tag");
                var y = new Tag("Tag");

                Assert.False(TagLabelComparer.Instance.Equals(x, y));
            }
        }

        public class GetHashCodeMethod : AutoFixtureTestBase
        {
            [Fact]
            public void LabelsEqual()
            {
                var label = Create<string>();

                var tag = new Tag(label);

                Assert.Equal(label.GetHashCode(StringComparison.Ordinal), TagLabelComparer.Instance.GetHashCode(tag));
            }
        }
    }
}
