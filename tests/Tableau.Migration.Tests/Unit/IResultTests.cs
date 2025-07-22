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
using System.Collections.Immutable;
using Moq;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public sealed class IResultTests
    {
        public sealed class Cast
        {
            [Fact]
            public void CastFailure()
            {
                var errors = new[] { new Exception(), new Exception() }.ToImmutableArray();

                var mockResult = new Mock<IResult<object>>();
                mockResult.CallBase = true;
                mockResult.SetupGet(x => x.Success).Returns(false);
                mockResult.SetupGet(x => x.Errors).Returns(errors);

                var castResult = mockResult.Object.Cast<TestContentType>();

                castResult.AssertFailure();
                Assert.Equal(errors, castResult.Errors);
            }

            [Fact]
            public void CastSuccess()
            {
                var value = new TestContentType();

                var mockResult = new Mock<IResult<object>>();
                mockResult.CallBase = true;
                mockResult.SetupGet(x => x.Success).Returns(true);
                mockResult.SetupGet(x => x.Value).Returns(value);

                var castResult = mockResult.Object.Cast<TestContentType>();

                castResult.AssertSuccess();
                Assert.Same(value, castResult.Value);
            }

            [Fact]
            public void InvalidCast()
            {
                var value = new TestContentType();

                var mockResult = new Mock<IResult<object>>();
                mockResult.CallBase = true;
                mockResult.SetupGet(x => x.Success).Returns(true);
                mockResult.SetupGet(x => x.Value).Returns(value);

                Assert.Throws<InvalidCastException>(() => mockResult.Object.Cast<User>());
            }
        }
    }
}
