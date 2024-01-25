using System;
using System.Net.Http;
using Moq;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class HttpGetRequestBuilderTests : AutoFixtureTestBase
    {
        [Fact]
        public void HttpMethod_is_Get()
        {
            var builder = new HttpGetRequestBuilder(Create<Uri>(), new Mock<IHttpClient>().Object);

            Assert.Equal(HttpMethod.Get, builder.Method);
        }
    }
}
