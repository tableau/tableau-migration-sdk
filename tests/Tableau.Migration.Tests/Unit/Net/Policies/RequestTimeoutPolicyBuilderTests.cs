using System;
using System.Collections.Generic;
using System.Net.Http;
using Moq;
using Polly.Timeout;
using Tableau.Migration.Config;
using Tableau.Migration.Net.Policies;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Policies
{
    public class RequestTimeoutPolicyBuilderTests
    {
        private readonly Mock<IConfigReader> _mockedConfigReader;
        private readonly MigrationSdkOptions _sdkOptions;
        private readonly RequestTimeoutPolicyBuilder _builder;

        public RequestTimeoutPolicyBuilderTests()
        {
            _mockedConfigReader = new Mock<IConfigReader>();
            _sdkOptions = new MigrationSdkOptions();
            _mockedConfigReader
                .Setup(x => x.Get())
                .Returns(_sdkOptions);
            _builder = new RequestTimeoutPolicyBuilder(
                _mockedConfigReader.Object);
        }

        [Fact]
        public void BuildPolicy_ReturnsDefaultPolicy()
        {
            // Act
            var policy = _builder.Build(new HttpRequestMessage());

            // Assert
            Assert.NotNull(policy);
            Assert.IsType<AsyncTimeoutPolicy<HttpResponseMessage>>(policy);
            var timeoutPolicy = (AsyncTimeoutPolicy<HttpResponseMessage>)policy;
            var timeoutProviderObject = timeoutPolicy.GetFieldValue("_timeoutProvider");
            Assert.NotNull(timeoutProviderObject);
            Assert.IsType<Func<Polly.Context, TimeSpan>>(timeoutProviderObject);
            var timeoutProvider = (Func<Polly.Context, TimeSpan>)timeoutProviderObject;
            Assert.Equal(_sdkOptions.Network.Resilience.PerRequestTimeout, timeoutProvider(new Polly.Context()));
        }

        public static IEnumerable<object[]> GetFileTransferRequests()
        {
            yield return new object[]
            {
                HttpMethod.Get,
                new Uri($"http://localhost/api/3.21/sites/{Guid.NewGuid()}/datasources/{Guid.NewGuid()}/content?includeExtract=true")
            };
            yield return new object[]
            {
                HttpMethod.Get,
                new Uri($"https://localhost/api/3.21/sites/{Guid.NewGuid()}/workbooks/{Guid.NewGuid()}/content?includeExtract=true")
            };
            yield return new object[]
            {
                HttpMethod.Put,
                new Uri($"https://localhost/api/3.21/sites/{Guid.NewGuid()}/fileUploads/{Guid.NewGuid()}")
            };
        }

        [Theory]
        [MemberData(nameof(GetFileTransferRequests))]
        public void BuildPolicyForFileTransfer_ReturnsDefaultPolicy(HttpMethod method, Uri uri)
        {
            // Act
            var policy = _builder.Build(new HttpRequestMessage(method, uri));

            // Assert
            Assert.NotNull(policy);
            Assert.IsType<AsyncTimeoutPolicy<HttpResponseMessage>>(policy);
            var timeoutPolicy = (AsyncTimeoutPolicy<HttpResponseMessage>)policy;
            var timeoutProviderObject = timeoutPolicy.GetFieldValue("_timeoutProvider");
            Assert.NotNull(timeoutProviderObject);
            Assert.IsType<Func<Polly.Context, TimeSpan>>(timeoutProviderObject);
            var timeoutProvider = (Func<Polly.Context, TimeSpan>)timeoutProviderObject;
            Assert.Equal(_sdkOptions.Network.Resilience.PerFileTransferRequestTimeout, timeoutProvider(new Polly.Context()));
        }

        [Fact]
        public void BuildPolicy_CustomTimeoutLimit_ReturnsPolicy()
        {
            // Arrange
            _sdkOptions.Network.Resilience.PerRequestTimeout = TimeSpan.FromSeconds(15);

            // Act
            var policy = _builder.Build(new HttpRequestMessage());

            // Assert
            Assert.NotNull(policy);
            Assert.IsType<AsyncTimeoutPolicy<HttpResponseMessage>>(policy);
            var timeoutPolicy = (AsyncTimeoutPolicy<HttpResponseMessage>)policy;
            var timeoutProviderObject = timeoutPolicy.GetFieldValue("_timeoutProvider");
            Assert.NotNull(timeoutProviderObject);
            Assert.IsType<Func<Polly.Context, TimeSpan>>(timeoutProviderObject);
            var timeoutProvider = (Func<Polly.Context, TimeSpan>)timeoutProviderObject;
            Assert.Equal(_sdkOptions.Network.Resilience.PerRequestTimeout, timeoutProvider(new Polly.Context()));
        }

        [Theory]
        [MemberData(nameof(GetFileTransferRequests))]
        public void BuildPolicyForFileTransfer_CustomTimeoutLimit_ReturnsPolicy(HttpMethod method, Uri uri)
        {
            // Arrange
            _sdkOptions.Network.Resilience.PerFileTransferRequestTimeout = TimeSpan.FromDays(1);

            // Act
            var policy = _builder.Build(new HttpRequestMessage(method, uri));

            // Assert
            Assert.NotNull(policy);
            Assert.IsType<AsyncTimeoutPolicy<HttpResponseMessage>>(policy);
            var timeoutPolicy = (AsyncTimeoutPolicy<HttpResponseMessage>)policy;
            var timeoutProviderObject = timeoutPolicy.GetFieldValue("_timeoutProvider");
            Assert.NotNull(timeoutProviderObject);
            Assert.IsType<Func<Polly.Context, TimeSpan>>(timeoutProviderObject);
            var timeoutProvider = (Func<Polly.Context, TimeSpan>)timeoutProviderObject;
            Assert.Equal(_sdkOptions.Network.Resilience.PerFileTransferRequestTimeout, timeoutProvider(new Polly.Context()));
        }
    }
}
