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
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class AsyncDisposableResultTests
    {
        public class AsyncDisposableTestClass : IAsyncDisposable
        {
            public bool IsDisposed { get; private set; }

            public ValueTask DisposeAsync()
            {
                GC.SuppressFinalize(this);
                IsDisposed = true;
                return ValueTask.CompletedTask;
            }
        }

        public class Dispose
        {
            [Fact]
            public async Task DisposesValue()
            {
                var value = new AsyncDisposableTestClass();

                await using (var result = AsyncDisposableResult<AsyncDisposableTestClass>.Succeeded(value))
                { }

                Assert.True(value.IsDisposed);
            }
        }

        public class Succeeded
        {
            [Fact]
            public async Task Initializes()
            {
                var value = new AsyncDisposableTestClass();

                await using var result = AsyncDisposableResult<AsyncDisposableTestClass>.Succeeded(value);

                Assert.True(result.Success);
                Assert.Same(value, result.Value);
            }
        }

        public class Failed
        {
            public class SingleException
            {
                [Fact]
                public async Task Initializes()
                {
                    var error = new Exception();

                    await using var result = AsyncDisposableResult<AsyncDisposableTestClass>.Failed(error);

                    Assert.False(result.Success);
                    Assert.Same(error, Assert.Single(result.Errors));
                }
            }

            public class ExceptionCollection
            {
                [Fact]
                public async Task Initializes()
                {
                    var errors = new[]
                    {
                        new Exception(),
                        new Exception()
                    };

                    await using var result = AsyncDisposableResult<AsyncDisposableTestClass>.Failed(errors);

                    Assert.False(result.Success);
                    Assert.True(errors.SequenceEqual(result.Errors));
                }

                [Fact]
                public void ThrowsWhenErrorsIsEmpty()
                {
                    Assert.Throws<ArgumentException>(() => AsyncDisposableResult<AsyncDisposableTestClass>.Failed(Enumerable.Empty<Exception>()));
                }
            }
        }

        public class Create
        {
            public class TestResult : IAsyncDisposable
            {
                public object? TestValue { get; }

                public TestResult(object? testValue)
                {
                    TestValue = testValue;
                }

                public ValueTask DisposeAsync()
                {
                    GC.SuppressFinalize(this);
                    return ValueTask.CompletedTask;
                }
            }

            [Fact]
            public async Task CatchesExceptions()
            {
                var exception = new Exception();

                await using var result = AsyncDisposableResult<TestResult>.Create(new object(), _ => throw exception);

                Assert.False(result.Success);

                var actualException = Assert.Single(result.Errors);

                Assert.Same(exception, actualException);
            }

            [Fact]
            public async Task Creates()
            {
                var value = new object();

                await using var result = AsyncDisposableResult<TestResult>.Create(value, v => new TestResult(v));

                Assert.True(result.Success);

                Assert.NotNull(result.Value);
                Assert.Same(value, result.Value.TestValue);
            }
        }
    }
}
