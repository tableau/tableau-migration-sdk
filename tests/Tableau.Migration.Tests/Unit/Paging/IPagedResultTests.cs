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

using System;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Paging
{
    public sealed class IPagedResultTests
    {
        public sealed class CastPagedFailure : AutoFixtureTestBase
        {
            [Fact]
            public void CastSuccessThrows()
            {
                IPagedResult<int> intResult = PagedResult<int>.Succeeded([1, 2, 3], 1, 10, 10, false);

                Assert.Throws<InvalidOperationException>(() => intResult.CastPagedFailure<Guid>());
            }

            [Fact]
            public void CastsFailureResult()
            {
                IPagedResult<int> intResult = PagedResult<int>.Failed(CreateMany<Exception>());

                var castResult = intResult.CastPagedFailure<Guid>();

                Assert.NotSame(intResult, castResult);
                Assert.Equal(intResult.Errors, castResult.Errors);
            }
        }
    }
}
