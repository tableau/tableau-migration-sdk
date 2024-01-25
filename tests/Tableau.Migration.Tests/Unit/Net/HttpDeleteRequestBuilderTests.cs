using System;
using System.Net.Http;
using Moq;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class HttpDeleteRequestBuilderTests : AutoFixtureTestBase
    {
        [Fact]
        public void HttpMethod_is_Delete()
        {
            var builder = new HttpDeleteRequestBuilder(Create<Uri>(), new Mock<IHttpClient>().Object);

            Assert.Equal(HttpMethod.Delete, builder.Method);
        }
    }
}
