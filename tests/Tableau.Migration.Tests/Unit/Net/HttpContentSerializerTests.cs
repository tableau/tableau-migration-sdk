﻿using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class HttpContentSerializerTests
    {
        public abstract class HttpContentSerializerTest : AutoFixtureTestBase
        {
            internal readonly Mock<HttpContentSerializer> MockSerializer = new(TableauSerializer.Instance)
            {
                CallBase = true
            };

            public readonly Mock<HttpContent> MockContent = new();
        }

        public class TryDeserializeErrorAsync : HttpContentSerializerTest
        {
            [Fact]
            public async Task Returns_null_when_no_error_found()
            {
                var tsResponse = new EmptyTableauServerResponse();

                Assert.Null(tsResponse.Error);

                var error = await MockSerializer.Object.TryDeserializeErrorAsync(MockContent.Object, Cancel);

                Assert.Null(error);
            }

            [Fact]
            public async Task Returns_error()
            {
                var tsResponse = new EmptyTableauServerResponse(new());

                var content = HttpContentSerializer.Instance.Serialize(tsResponse, MediaTypes.Xml)!;

                MockSerializer.Setup(s => s.DeserializeAsync<EmptyTableauServerResponse>(content, Cancel))
                    .ReturnsAsync(tsResponse);

                var error = await MockSerializer.Object.TryDeserializeErrorAsync(content, Cancel);

                Assert.NotNull(error);
                Assert.Same(tsResponse.Error, error);

                MockSerializer.VerifyAll();
            }

            [Fact]
            public async Task Returns_null_on_deserialization_error()
            {
                var error = await MockSerializer.Object.TryDeserializeErrorAsync(MockContent.Object, Cancel);

                Assert.Null(error);
            }
        }
    }
}
