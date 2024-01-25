using System;
using Moq;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class HttpRequestBuilderFactoryTests
    {
        public abstract class HttpRequestBuilderFactoryTest : AutoFixtureTestBase
        {
            protected readonly Mock<IHttpClient> MockHttpClient = new();
            protected readonly Mock<IHttpContentSerializer> MockSerializer = new();
            protected readonly Mock<IRequestBuilder> MockUriBuilder = new();

            internal readonly HttpRequestBuilderFactory Factory;

            public HttpRequestBuilderFactoryTest()
            {
                Factory = new HttpRequestBuilderFactory(MockHttpClient.Object, MockSerializer.Object);
            }
        }

        public class CreateDeleteRequest : HttpRequestBuilderFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var builder = Factory.CreateDeleteRequest(Create<Uri>());
                Assert.IsType<HttpDeleteRequestBuilder>(builder);
            }
        }

        public class CreateGetRequest : HttpRequestBuilderFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var builder = Factory.CreateGetRequest(Create<Uri>());
                Assert.IsType<HttpGetRequestBuilder>(builder);
            }
        }

        public class CreatePatchRequest : HttpRequestBuilderFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var builder = Factory.CreatePatchRequest(Create<Uri>());
                Assert.IsType<HttpPatchRequestBuilder>(builder);
            }
        }

        public class CreatePostRequest : HttpRequestBuilderFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var builder = Factory.CreatePostRequest(Create<Uri>());
                Assert.IsType<HttpPostRequestBuilder>(builder);
            }
        }

        public class CreatePutRequest : HttpRequestBuilderFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var builder = Factory.CreatePutRequest(Create<Uri>());
                Assert.IsType<HttpPutRequestBuilder>(builder);
            }
        }
    }
}
