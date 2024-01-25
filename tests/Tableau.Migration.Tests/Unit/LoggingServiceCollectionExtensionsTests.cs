using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class LoggingServiceCollectionExtensionsTests
    {
        /// <summary>
        /// This unit test ensure we have only the reference to the abstraction on the Migration project
        /// </summary>
        [Fact]
        public void ServiceCollectionWithoutLoggerFactoryAndProvider()
        {
            // Arrange/Act
            var serviceCollection = new ServiceCollection()
                .AddTableauMigrationSdk();

            // Assert
            Assert.NotEmpty(serviceCollection.Where(descriptor => descriptor.ServiceType == typeof(ILoggerFactory)));
            Assert.Empty(serviceCollection.Where(descriptor => descriptor.ServiceType == typeof(ILoggerProvider)));
        }

        /// <summary>
        /// This unit test ensure we have only the reference to the abstraction on the Migration project
        /// </summary>
        [Fact]
        public void ServiceProviderWithoutLoggerFactoryAndProvider()
        {
            // Arrange/Act
            var serviceProvider = new ServiceCollection()
                .AddTableauMigrationSdk()
                .BuildServiceProvider();

            // Assert
            Assert.NotNull(serviceProvider.GetService<ILoggerFactory>());
            Assert.Null(serviceProvider.GetService<ILoggerProvider>());
        }

        [Fact]
        public void AddLogging_ServiceProviderWithLoggerFactoryAndWithoutProvider()
        {
            // Arrange/Act
            var serviceProvider = new ServiceCollection()
                .AddTableauMigrationSdk()
                .AddLogging()
                .BuildServiceProvider();

            // Assert
            Assert.NotNull(serviceProvider.GetService<ILoggerFactory>());
            Assert.Null(serviceProvider.GetService<ILoggerProvider>());
        }

        [Fact]
        public void AddLoggingAndProvider_ServiceProviderWithLoggerFactoryAndProvider()
        {
            // Arrange/Act
            var serviceProvider = new ServiceCollection()
                .AddTableauMigrationSdk()
                .AddLogging()
                .AddSingleton(Mock.Of<ILoggerProvider>())
                .BuildServiceProvider();

            // Assert
            Assert.NotNull(serviceProvider.GetService<ILoggerFactory>());
            Assert.NotNull(serviceProvider.GetService<ILoggerProvider>());
        }
    }
}
