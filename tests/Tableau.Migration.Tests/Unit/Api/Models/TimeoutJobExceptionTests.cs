using Tableau.Migration.Api.Models;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class TimeoutJobExceptionTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes_with_job()
            {
                var job = Create<IJob>();
                var mockLocalizer = new MockSharedResourcesLocalizer();

                var ex = new TimeoutJobException(job, mockLocalizer.Object);

                Assert.Same(job, ex.Job);
            }

            [Fact]
            public void Initializes_without_job()
            {
                var mockLocalizer = new MockSharedResourcesLocalizer();

                var ex = new TimeoutJobException(null, mockLocalizer.Object);

                Assert.Null(ex.Job);
            }
        }
    }
}
