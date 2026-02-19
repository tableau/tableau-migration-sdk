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
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public sealed class IContentReferenceExtensionsTests
    {
        public sealed class ThrowOnMissingContentReference : AutoFixtureTestBase
        {
            private readonly ISharedResourcesLocalizer _localizer;

            public ThrowOnMissingContentReference()
            {
                _localizer = new MockSharedResourcesLocalizer().Object;
            }

            [Fact]
            public void SuccessResult()
            {
                var result = Result<IContentReference>.Succeeded(Create<IContentReference>());

                var contentReference = result.ThrowOnMissingContentReference<IUser>(_localizer, "", Create<ContentLocation>());

                Assert.Same(result.Value, contentReference);
            }

            [Fact]
            public void FailedResult()
            {
                var result = Result<IContentReference>.Failed(CreateMany<Exception>());
                var use = Create<string>();

                var ex = Assert.Throws<Exception>(() => result.ThrowOnMissingContentReference<IUser>(_localizer, use, Create<ContentLocation>()));

                Assert.Contains(use, ex.Message);

                var inner = Assert.IsType<AggregateException>(ex.InnerException);
                Assert.Equal(result.Errors, inner.InnerExceptions);
            }

            [Fact]
            public void ResultWithId()
            {
                var result = Result<IContentReference>.Succeeded(Create<IContentReference>());

                var contentReference = result.ThrowOnMissingContentReference<IUser>(_localizer, "", Create<Guid>());

                Assert.Same(result.Value, contentReference);
            }

            [Fact]
            public void NonNullContentReference()
            {
                var input = Create<IContentReference>();

                var result = input.ThrowOnMissingContentReference<IUser>(_localizer, "", Create<ContentLocation>());

                Assert.Same(input, result);
            }

            [Fact]
            public void NullContentReference()
            {
                IContentReference? input = null;
                var use = Create<string>();

                var ex = Assert.Throws<Exception>(() => input.ThrowOnMissingContentReference<IUser>(_localizer, use, Create<ContentLocation>()));

                Assert.Contains(use, ex.Message);
            }

            [Fact]
            public void ContentReferenceWithId()
            {
                var input = Create<IContentReference>();

                var result = input.ThrowOnMissingContentReference<IUser>(_localizer, "", Create<Guid>());

                Assert.Same(input, result);
            }

            [Fact]
            public void ContentReferenceWithContentUrl()
            {
                var input = Create<IContentReference>();

                var result = input.ThrowOnMissingContentReference<IUser>(_localizer, "", Create<string>());

                Assert.Same(input, result);
            }
        }
    }
}
