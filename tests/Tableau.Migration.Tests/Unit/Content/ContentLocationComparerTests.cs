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

using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class ContentLocationComparerTests
    {
        public class Instance
        {
            [Fact]
            public void IsSingleton()
            {
                var instance1 = ContentLocationComparer<TestContentType>.Instance;
                var instance2 = ContentLocationComparer<TestContentType>.Instance;

                Assert.Same(instance1, instance2);
            }
        }

        public class Compare : AutoFixtureTestBase
        {
            [Fact]
            public void BothNull()
            {
                TestContentType? a = null, b = null;

                var result = ContentLocationComparer<TestContentType>.Instance.Compare(a, b);

                Assert.Equal(0, result);
            }

            [Fact]
            public void SingleNull()
            {
                TestContentType? a = null, b = Create<TestContentType>();

                var result = ContentLocationComparer<TestContentType>.Instance.Compare(a, b);

                Assert.Equal(-1, result);

                result = ContentLocationComparer<TestContentType>.Instance.Compare(b, a);

                Assert.Equal(1, result);
            }

            [Fact]
            public void CompareByLocation()
            {
                TestContentType a = Create<TestContentType>(), b = Create<TestContentType>();

                var result = ContentLocationComparer<TestContentType>.Instance.Compare(a, b);

                Assert.Equal(a.Location.CompareTo(b.Location), result);
            }
        }
    }
}
