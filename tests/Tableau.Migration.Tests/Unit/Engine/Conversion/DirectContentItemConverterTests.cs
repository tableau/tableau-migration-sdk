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
using System.Threading.Tasks;
using Tableau.Migration.Engine.Conversion;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Conversion
{
    public sealed class DirectContentItemConverterTests
    {
        public sealed class ConvertAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task ReturnsItemAsync()
            {
                var item = Create<TestContentType>();

                var converter = Create<DirectContentItemConverter<TestContentType, TestContentType>>();

                var result = await converter.ConvertAsync(item, Cancel);

                Assert.Same(item, result);
            }

            [Fact]
            public async Task ValidCastAsync()
            {
                var item = Create<TestFileContentType>();

                var converter = Create<DirectContentItemConverter<TestFileContentType, IAsyncDisposable>>();

                var result = await converter.ConvertAsync(item, Cancel);

                Assert.Same(item, result);
            }

            [Fact]
            public async Task InvalidCastAsync()
            {
                var item = Create<TestContentType>();

                var converter = Create<DirectContentItemConverter<TestContentType, TestPublishType>>();

                await Assert.ThrowsAsync<InvalidCastException>(() => converter.ConvertAsync(item, Cancel));
            }
        }
    }
}
