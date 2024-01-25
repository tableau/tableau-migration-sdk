using System;
using System.Net.Http;
using Moq;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class HttpPostRequestBuilderTests : AutoFixtureTestBase
    {
        [Fact]
        public void HttpMethod_is_Post()
        {
            var builder = new HttpPostRequestBuilder(Create<Uri>(), new Mock<IHttpClient>().Object, new Mock<IHttpContentSerializer>().Object);

            Assert.Equal(HttpMethod.Post, builder.Method);
        }
    }
}
