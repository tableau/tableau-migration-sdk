﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using Moq;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class HttpRequestBuilderTests
    {
        internal interface ITestHttpRequestBuilder : IHttpRequestBuilder<ITestHttpRequestBuilder>
        { }

        internal class TestHttpRequestBuilder : HttpRequestBuilder<TestHttpRequestBuilder, ITestHttpRequestBuilder>, ITestHttpRequestBuilder
        {
            internal override HttpMethod Method { get; } = HttpMethod.Head;

            public TestHttpRequestBuilder(Uri uri, IHttpClient httpClient)
                : base(uri, httpClient)
            { }
        }

        public abstract class HttpRequestBuilderTest : AutoFixtureTestBase
        {
            protected readonly Mock<IHttpClient> MockHttpClient = new();

            internal readonly TestHttpRequestBuilder Builder;

            public HttpRequestBuilderTest()
            {
                Builder = new(TestConstants.LocalhostUri, MockHttpClient.Object);
            }
        }

        public class Ctor : HttpRequestBuilderTest
        {
            [Fact]
            public void Initializes()
            {
                var builder = new TestHttpRequestBuilder(TestConstants.LocalhostUri, MockHttpClient.Object);

                var request = builder.Request;

                Assert.Equal(builder.Method, request.Method);
                Assert.Equal(TestConstants.LocalhostUri, request.RequestUri);
            }
        }

        public class AddHeader : HttpRequestBuilderTest
        {
            [Fact]
            public void Sets_header()
            {
                var name = Create<string>();
                var value = Create<string>();

                Builder.AddHeader(name, value);

                Builder.Request.AssertSingleHeaderValue(name, value);
            }
        }

        public class Accept : HttpRequestBuilderTest
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Sets_Accept_header(bool clearExisting)
            {
                Builder.Request.Headers.Accept.Add(MediaTypes.Json);

                var contentType = new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Plain);

                Builder.Accept(contentType, clearExisting);

                Assert.Contains(contentType, Builder.Request.Headers.Accept);

                if (clearExisting)
                    Assert.DoesNotContain(MediaTypes.Json, Builder.Request.Headers.Accept);
                else
                    Assert.Contains(MediaTypes.Json, Builder.Request.Headers.Accept);
            }
        }

        public class AcceptJson : HttpRequestBuilderTest
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Sets_Accept_header(bool clearExisting)
            {
                Builder.Request.Headers.Accept.Add(MediaTypes.Xml);

                Builder.AcceptJson(clearExisting);

                Assert.Contains(MediaTypes.Json, Builder.Request.Headers.Accept);

                if (clearExisting)
                    Assert.DoesNotContain(MediaTypes.Xml, Builder.Request.Headers.Accept);
                else
                    Assert.Contains(MediaTypes.Xml, Builder.Request.Headers.Accept);
            }
        }

        public class AcceptXml : HttpRequestBuilderTest
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Sets_Accept_header(bool clearExisting)
            {
                Builder.Request.Headers.Accept.Add(MediaTypes.Json);

                Builder.AcceptXml(clearExisting);

                Assert.Contains(MediaTypes.Xml, Builder.Request.Headers.Accept);

                if (clearExisting)
                    Assert.DoesNotContain(MediaTypes.Json, Builder.Request.Headers.Accept);
                else
                    Assert.Contains(MediaTypes.Json, Builder.Request.Headers.Accept);
            }
        }
    }
}
