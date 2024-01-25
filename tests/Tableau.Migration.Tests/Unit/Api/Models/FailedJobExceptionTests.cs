using Tableau.Migration.Api.Models;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class FailedJobExceptionTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var job = Create<IJob>();
                var mockLocalizer = new MockSharedResourcesLocalizer();

                var ex = new FailedJobException(job, mockLocalizer.Object);

                Assert.Same(job, ex.FailedJob);
            }
        }
    }
}
