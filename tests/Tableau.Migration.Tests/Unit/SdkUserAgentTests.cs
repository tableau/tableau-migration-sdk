using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Interop;
using Tableau.Migration.Interop.Logging;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class SdkUserAgentTests
    {
        private readonly IServiceCollection _servicesCollection;
        private ServiceProvider _services;

        public SdkUserAgentTests()
        {
            _servicesCollection = new ServiceCollection()
                .AddTableauMigrationSdk();
            _services = _servicesCollection.BuildServiceProvider();
        }

        [Fact]
        public void DefaultValues()
        {
            var sdkMetaData = _services.GetRequiredService<IMigrationSdk>();
            var id = sdkMetaData.UserAgent;
            Assert.NotNull(id);
            Assert.NotEmpty(id);
            Assert.StartsWith(Constants.USER_AGENT_PREFIX, id);

            Assert.Contains(sdkMetaData.Version.ToString(), id);
        }

        [Fact]
        public void PythonValues()
        {
            var mockLoggerFactory = new Mock<Func<IServiceProvider, NonGenericLoggerProvider>>();

            _servicesCollection.AddPythonSupport(mockLoggerFactory.Object);
            _services = _servicesCollection.BuildServiceProvider();

            var sdkMetaData = _services.GetRequiredService<IMigrationSdk>();
            var id = sdkMetaData.UserAgent;
            Assert.NotNull(id);
            Assert.NotEmpty(id);
            Assert.StartsWith(Constants.USER_AGENT_PREFIX, id);
            Assert.Contains(Constants.USER_AGENT_PYTHON_SUFFIX, id);

            Assert.Contains(sdkMetaData.Version.ToString(), id);
        }
    }
}
