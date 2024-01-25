using System;
using System.Net.Http;
using Moq;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class HttpPatchRequestBuilderTests : AutoFixtureTestBase
    {
        [Fact]
        public void HttpMethod_is_Patch()
        {
            var builder = new HttpPatchRequestBuilder(Create<Uri>(), new Mock<IHttpClient>().Object, new Mock<IHttpContentSerializer>().Object);

            Assert.Equal(HttpMethod.Patch, builder.Method);
        }
    }
}
