using System;
using System.Collections.Immutable;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Paging
{
    public class PagedResultTests
    {
        public class Succeeded : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var items = CreateMany<object>().ToImmutableList();

                var r = PagedResult<object>.Succeeded(items, 1, 2, 3);

                r.AssertSuccess();

                Assert.Equal(items, r.Value);
                Assert.Equal(1, r.PageNumber);
                Assert.Equal(2, r.PageSize);
                Assert.Equal(3, r.TotalCount);
            }
        }

        public class Failed
        {
            public class SingleException : AutoFixtureTestBase
            {
                [Fact]
                public void Initializes()
                {
                    var error = new Exception();

                    var r = PagedResult<object>.Failed(error);

                    r.AssertFailure();

                    Assert.Same(error, Assert.Single(r.Errors));
                    Assert.Equal(0, r.PageNumber);
                    Assert.Equal(0, r.PageSize);
                    Assert.Equal(0, r.TotalCount);
                }
            }

            public class ExceptionCollection : AutoFixtureTestBase
            {
                [Fact]
                public void Initializes()
                {
                    var errors = CreateMany<Exception>().ToImmutableList();

                    var r = PagedResult<object>.Failed(errors);

                    r.AssertFailure();

                    Assert.Equal(errors, r.Errors);
                    Assert.Equal(0, r.PageNumber);
                    Assert.Equal(0, r.PageSize);
                    Assert.Equal(0, r.TotalCount);
                }
            }
        }
    }
}
