using System;
using System.Linq;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class ResultBuilderTests
    {
        public class Build
        {
            [Fact]
            public void NoErrorsSucceeds()
            {
                var b = new ResultBuilder();

                var result = b.Build();

                result.AssertSuccess();
            }

            [Fact]
            public void FailsOnErrors()
            {
                var errorResult = Result.Failed(new[] { new Exception(), new Exception() });

                var b = new ResultBuilder();
                b.Add(errorResult);

                var result = b.Build();

                result.AssertFailure();
                Assert.Equal(errorResult.Errors, result.Errors);
            }

            [Fact]
            public void ErrorsAggregated()
            {
                var errorResult1 = Result.Failed(new[] { new Exception(), new Exception() });
                var errorResult2 = Result.Failed(new[] { new Exception() });

                var b = new ResultBuilder();
                b.Add(errorResult1, errorResult2);

                var result = b.Build();

                result.AssertFailure();
                Assert.Equal(errorResult1.Errors.Concat(errorResult2.Errors), result.Errors);
            }

            [Fact]
            public void SuccessWithValue()
            {
                var val = new object();
                var b = new ResultBuilder();

                var result = b.Build(val);

                result.AssertSuccess();

                Assert.Same(val, result.Value);
            }

            [Fact]
            public void FailureWithValue()
            {
                object? val = null;
                var b = new ResultBuilder();
                b.Add(new Exception());

                var result = b.Build(val);

                result.AssertFailure();

                Assert.Same(val, result.Value);
            }
        }
    }
}
