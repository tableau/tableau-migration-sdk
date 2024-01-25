using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class ILoggerTests
    {
        public class DummyLoggerService
        {
            private readonly ILogger<DummyLoggerService> _logger;

            public DummyLoggerService(
                ILogger<DummyLoggerService> logger)
            {
                _logger = logger;
            }

            public void LogAllLevels()
            {
                _logger.LogTrace("This is a trace");
                _logger.LogDebug("This is a debug");
                _logger.LogInformation("This is an information");
                _logger.LogWarning("This is a warning");
                _logger.LogError("This is an error");
                _logger.LogCritical("This is a critical");
            }
        }

        private readonly IServiceCollection _serviceCollection;
        private readonly Mock<ILoggerProvider> _mockedLoggerProvider;
        private readonly Mock<ILogger> _mockedLogger;

        public ILoggerTests()
        {
            _mockedLogger = new Mock<ILogger>();
            _mockedLoggerProvider = new Mock<ILoggerProvider>();
            _mockedLoggerProvider
                .Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(_mockedLogger.Object);
            _serviceCollection = new ServiceCollection()
                .AddTableauMigrationSdk()
                .AddSingleton(_mockedLoggerProvider.Object)
                .AddSingleton(provider =>
                    new DummyLoggerService(
                        provider.GetRequiredService<ILogger<DummyLoggerService>>()));
        }

        [Fact]
        public void DefaultLogging_LogLevelInformation()
        {
            // Arrange
            using var provider = _serviceCollection
                .AddLogging()
                .BuildServiceProvider();
            var dummyService = provider.GetRequiredService<DummyLoggerService>();

            // Act
            dummyService.LogAllLevels();

            // Assert
            _mockedLogger.Verify(x =>
                x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((obj, typ) => true),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                    Times.Exactly(4));
        }

        [Fact]
        public void LogAllLogs_LogLevelTrace()
        {
            // Arrange
            using var provider = _serviceCollection
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Trace);
                })
                .BuildServiceProvider();
            var dummyService = provider.GetRequiredService<DummyLoggerService>();

            // Act
            dummyService.LogAllLevels();

            // Assert
            _mockedLogger.Verify(x =>
                x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((obj, typ) => true),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                    Times.Exactly(6));
        }
    }
}
