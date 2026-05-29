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

using Tableau.Migration.Engine.Preparation;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Preparation
{
    public sealed class IContentItemPreparationResultTests
    {
        public sealed class IsPrepared : AutoFixtureTestBase
        {
            [Fact]
            public void Failed()
            {
                IContentItemPreparationResult<TestContentType> result =
                    ContentItemPreparationResult<TestContentType>.Failed([new()]);

                Assert.False(result.IsPrepared);
            }

            [Fact]
            public void Skipped()
            {
                IContentItemPreparationResult<TestContentType> result =
                    ContentItemPreparationResult<TestContentType>.Skipped;

                Assert.False(result.IsPrepared);
            }

            [Fact]
            public void Prepared()
            {
                IContentItemPreparationResult<TestContentType> result =
                    ContentItemPreparationResult<TestContentType>.Succeeded(new());

                Assert.True(result.IsPrepared);
            }
        }
    }
}
