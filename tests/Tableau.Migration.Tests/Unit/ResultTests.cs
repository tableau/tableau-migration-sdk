using System;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class ResultTests
    {
        public class NonGeneric
        {
            public class Succeeded
            {
                [Fact]
                public void Initializes()
                {
                    var result = Result.Succeeded();

                    Assert.True(result.Success);
                }
            }

            public class Failed
            {
                [Fact]
                public void Initializes()
                {
                    var errors = new[]
                    {
                        new Exception(),
                        new Exception()
                    };

                    var result = Result.Failed(errors);

                    Assert.False(result.Success);
                    Assert.True(errors.SequenceEqual(result.Errors));
                }

                [Fact]
                public void InitializesWithSingleError()
                {
                    var exception = new Exception();
                    var result = Result.Failed(exception);

                    Assert.False(result.Success);
                    var error = Assert.Single(result.Errors);
                    Assert.Same(exception, error);
                }

                [Fact]
                public void ThrowsWhenErrorsIsEmpty()
                {
                    Assert.Throws<ArgumentException>(() => Result.Failed(Enumerable.Empty<Exception>()));
                }
            }

            public class FromErrors
            {
                [Fact]
                public void SuccessWithNoErrors()
                {
                    var result = Result.FromErrors(ImmutableArray<Exception>.Empty);

                    Assert.True(result.Success);
                }

                [Fact]
                public void FailureWithErrors()
                {
                    var errors = new[] { new Exception() };
                    var result = Result.FromErrors(errors);

                    Assert.False(result.Success);
                    Assert.True(errors.SequenceEqual(result.Errors));
                }
            }
        }

        public class Generic
        {
            public class Succeeded
            {
                [Fact]
                public void Initializes()
                {
                    var value = new object();

                    var result = Result<object>.Succeeded(value);

                    Assert.True(result.Success);
                    Assert.Same(value, result.Value);
                }
            }

            public class Failed
            {
                [Fact]
                public void Initializes()
                {
                    var errors = new[]
                    {
                        new Exception(),
                        new Exception()
                    };

                    var result = Result<object>.Failed(errors);

                    Assert.False(result.Success);
                    Assert.True(errors.SequenceEqual(result.Errors));
                }

                [Fact]
                public void ThrowsWhenErrorsIsEmpty()
                {
                    Assert.Throws<ArgumentException>(() => Result<object>.Failed(Enumerable.Empty<Exception>()));
                }
            }

            public class Create
            {
                public class TestResult
                {
                    public object? TestValue { get; }

                    public TestResult(object? testValue)
                    {
                        TestValue = testValue;
                    }
                }

                [Fact]
                public void CatchesExceptions()
                {
                    var exception = new Exception();

                    var result = Result<TestResult>.Create(new object(), _ => throw exception);

                    Assert.False(result.Success);

                    var actualException = Assert.Single(result.Errors);

                    Assert.Same(exception, actualException);
                }

                [Fact]
                public void Creates()
                {
                    var value = new object();

                    var result = Result<TestResult>.Create(value, v => new TestResult(v));

                    Assert.True(result.Success);

                    Assert.NotNull(result.Value);
                    Assert.Same(value, result.Value.TestValue);
                }

                [Fact]
                public void CreatesFromSuccessResult()
                {
                    var value = new object();

                    var result = Result<object>.Create(Result.Succeeded(), value);

                    Assert.True(result.Success);
                    Assert.Empty(result.Errors);

                    Assert.NotNull(result.Value);
                    Assert.Same(value, result.Value);
                }

                [Fact]
                public void CreatesFromFailureResult()
                {
                    var value = new object();
                    var errors = new[] { new Exception(), new Exception() };

                    var result = Result<object>.Create(Result.Failed(errors), value);

                    Assert.False(result.Success);
                    Assert.Equal(errors, result.Errors);

                    Assert.NotNull(result.Value);
                    Assert.Same(value, result.Value);
                }

                [Fact]
                public void CreatesFromFailureNullResult()
                {
                    object? value = null;
                    var errors = new[] { new Exception(), new Exception() };

                    var result = Result<object>.Create(Result.Failed(errors), value);

                    Assert.False(result.Success);
                    Assert.Equal(errors, result.Errors);

                    Assert.Null(result.Value);
                }

                [Fact]
                public void CreatesFromSuccessResultBuilder()
                {
                    var value = new object();

                    var builder = new ResultBuilder();

                    var result = Result<object>.Create(builder, value);

                    Assert.True(result.Success);
                    Assert.Empty(result.Errors);

                    Assert.NotNull(result.Value);
                    Assert.Same(value, result.Value);
                }

                [Fact]
                public void CreatesFromFailureResultBuilder()
                {
                    var value = new object();
                    var errors = new[] { new Exception(), new Exception() };

                    var builder = new ResultBuilder();
                    builder.Add(errors);

                    var result = Result<object>.Create(builder, value);

                    Assert.False(result.Success);
                    Assert.Equal(errors, result.Errors);

                    Assert.NotNull(result.Value);
                    Assert.Same(value, result.Value);
                }
            }
        }
    }
}
