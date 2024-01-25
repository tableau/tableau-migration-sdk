using System;
using System.Net.Http;
using Moq;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class HttpPutRequestBuilderTests : AutoFixtureTestBase
    {
        [Fact]
        public void HttpMethod_is_Put()
        {
            var builder = new HttpPutRequestBuilder(Create<Uri>(), new Mock<IHttpClient>().Object, new Mock<IHttpContentSerializer>().Object);

            Assert.Equal(HttpMethod.Put, builder.Method);
        }
    }
}
