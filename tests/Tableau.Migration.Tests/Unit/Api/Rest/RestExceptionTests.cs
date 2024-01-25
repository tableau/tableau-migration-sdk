using System;
using Microsoft.Extensions.Localization;
using Moq;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest
{
    public class RestExceptionTests
    {
        public abstract class RestExceptionTest : AutoFixtureTestBase
        { }

        public class Ctor : RestExceptionTest
        {
            [Fact]
            public void Initializes()
            {
                var mockLocalizer = new Mock<ISharedResourcesLocalizer>();
                mockLocalizer
                    .Setup(x => x[It.IsAny<string>()])
                    .Returns(new LocalizedString("Error", "Error"));
                var error = Create<Error>();

                var exception = new RestException(
                    new Uri("http://localhost"),
                    error,
                    mockLocalizer.Object);

                Assert.Equal(error.Code, exception.Code);
                Assert.Equal(error.Detail, exception.Detail);
                Assert.Equal(error.Summary, exception.Summary);
            }
        }
    }
}
