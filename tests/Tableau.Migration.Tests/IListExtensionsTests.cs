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

using System.Collections.Generic;
using Xunit;

namespace Tableau.Migration.Tests
{
    public sealed class IListExtensionsTests
    {
        public sealed class ExceptIfAny : AutoFixtureTestBase
        {
            [Fact]
            public void EmptyExceptDoesNotAllocate()
            {
                var list = new List<int> { 1, 2, 3 };

                var result = list.ExceptIfAny([]);

                Assert.Same(list, result);
            }

            [Fact]
            public void Except()
            {
                var list = new List<int> { 1, 2, 3 };

                var result = list.ExceptIfAny([2]);

                Assert.NotSame(list, result);
                Assert.Equal([1, 3], result);
            }
        }
    }
}
