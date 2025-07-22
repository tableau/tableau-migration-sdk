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
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public sealed class IContentReferenceExtensionsTests
    {
        public sealed class ThrowOnMissingContentReference : AutoFixtureTestBase
        {
            [Fact]
            public void SuccessResult()
            {
                var result = Result<IContentReference>.Succeeded(Create<IContentReference>());

                var contentReference = result.ThrowOnMissingContentReference("");

                Assert.Same(result.Value, contentReference);
            }

            [Fact]
            public void FailedResult()
            {
                var result = Result<IContentReference>.Failed(CreateMany<Exception>());
                var msg = Create<string>();

                var ex = Assert.Throws<AggregateException>(() => result.ThrowOnMissingContentReference(msg));

                Assert.Contains(msg, ex.Message);
            }

            [Fact]
            public void NonNullContentReference()
            {
                var input = Create<IContentReference>();

                var result = input.ThrowOnMissingContentReference("");

                Assert.Same(input, result);
            }

            [Fact]
            public void NullContentReference()
            {
                IContentReference? input = null;
                var msg = Create<string>();

                var ex = Assert.Throws<Exception>(() => input.ThrowOnMissingContentReference(msg));

                Assert.Equal(msg, ex.Message);
            }
        }
    }
}
