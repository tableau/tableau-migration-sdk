using System;
using Xunit;

namespace Tableau.Migration.Tests
{
    public class IResultTests
    {
        public class CastFailure
        {
            [Fact]
            public void CastsFailure()
            {
                var errors = new[] { new Exception(), new Exception() };
                IResult<TestContentType> failure = Result<TestContentType>.Failed(errors);

                var cast = failure.CastFailure<TestFileContentType>();

                cast.AssertFailure();
                Assert.Equal(errors, cast.Errors);
            }

            [Fact]
            public void ThrowsOnSuccess()
            {
                IResult<TestContentType> success = Result<TestContentType>.Succeeded(new());

                Assert.Throws<InvalidOperationException>(() => success.CastFailure<TestFileContentType>());
            }
        }
    }
}
