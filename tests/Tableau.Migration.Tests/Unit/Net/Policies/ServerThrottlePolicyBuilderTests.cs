using System.Net.Http;
using Moq;
using Polly.Retry;
using Tableau.Migration.Config;
using Tableau.Migration.Net.Policies;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Policies
{
    public class ServerThrottlePolicyBuilderTests
    {
        private readonly Mock<IConfigReader> _mockedConfigReader;
        private readonly MigrationSdkOptions _sdkOptions;
        private readonly ServerThrottlePolicyBuilder _builder;

        public ServerThrottlePolicyBuilderTests()
        {
            _mockedConfigReader = new Mock<IConfigReader>();
            _sdkOptions = new MigrationSdkOptions();
            _mockedConfigReader
                .Setup(x => x.Get())
                .Returns(_sdkOptions);
            _builder = new(_mockedConfigReader.Object);
        }

        [Fact]
        public void PolicyDisabled()
        {
            // Arrange
            _sdkOptions.Network.Resilience.ServerThrottleEnabled = false;

            // Act
            var policy = _builder.Build(new HttpRequestMessage());

            // Assert
            Assert.Null(policy);
        }

        [Fact]
        public void PolicyEnabled()
        {
            // Arrange
            _sdkOptions.Network.Resilience.ClientThrottleEnabled = true;

            // Act
            var policy = _builder.Build(new HttpRequestMessage());

            // Assert
            Assert.NotNull(policy);
            Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(policy);
        }

        [Fact]
        public void RetriesLimited()
        {
            // Arrange
            _sdkOptions.Network.Resilience.ServerThrottleLimitRetries = true;

            // Act
            var policy = _builder.Build(new HttpRequestMessage());

            // Assert
            Assert.NotNull(policy);
            Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(policy);
        }
    }
}
