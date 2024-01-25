using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using Moq;
using Tableau.Migration.Net.Handlers;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Handlers
{
    public class UserAgentHttpMessageHandlerTests
    {
        private readonly Mock<IMigrationSdk> _mockedSdkMetadata;

        public UserAgentHttpMessageHandlerTests()
        {
            _mockedSdkMetadata = new Mock<IMigrationSdk>();
        }

        [Fact]
        public void CallSendAsync_AttachUserAgentHeader()
        {
            // Arrange
            var currentVersion = new Version("1.2.3.4");
            _mockedSdkMetadata.Setup(x => x.Version)
                              .Returns(currentVersion);
            _mockedSdkMetadata.Setup(x => x.UserAgent)
                              .Returns($"{Constants.USER_AGENT_PREFIX}/{currentVersion}");
            var handler = new UserAgentHttpMessageHandler(_mockedSdkMetadata.Object);
            handler.InnerHandler = Mock.Of<HttpMessageHandler>();
            var methodInfo = typeof(UserAgentHttpMessageHandler).GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var message = new HttpRequestMessage();
            Assert.Empty(message.Headers.UserAgent);

            // Act
            _ = methodInfo.Invoke(handler, new object[] { message, CancellationToken.None });

            // Assert
            Assert.Single(message.Headers.UserAgent);
            Assert.Collection(
                message.Headers.UserAgent,
                value =>
                {
                    Assert.NotNull(value.Product);
                    Assert.Equal(value.Product.ToString(), _mockedSdkMetadata.Object.UserAgent);
                });
        }

        [Fact]
        public void CallSendAsync_ReplaceUserAgentHeader()
        {
            // Arrange
            var currentVersion = new Version("10.12.23.44");
            _mockedSdkMetadata.Setup(x => x.Version)
                              .Returns(currentVersion);
            _mockedSdkMetadata.Setup(x => x.UserAgent)
                              .Returns($"{Constants.USER_AGENT_PREFIX}/{currentVersion}");
            var handler = new UserAgentHttpMessageHandler(_mockedSdkMetadata.Object);
            handler.InnerHandler = Mock.Of<HttpMessageHandler>();
            var methodInfo = typeof(UserAgentHttpMessageHandler).GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var message = new HttpRequestMessage();
            message.Headers.UserAgent.TryParseAdd("PreSetAgent");
            Assert.Single(message.Headers.UserAgent);

            // Act
            _ = methodInfo.Invoke(handler, new object[] { message, CancellationToken.None });

            // Assert
            Assert.Single(message.Headers.UserAgent);
            Assert.Collection(
                message.Headers.UserAgent,
                value =>
                {
                    Assert.NotNull(value.Product);
                    Assert.Equal(value.Product.ToString(), _mockedSdkMetadata.Object.UserAgent);
                });
        }

        [Fact]
        public void CallSendAsync_ReplaceUserAgent_DontTouchOtherHeaders()
        {
            // Arrange
            var currentVersion = new Version("1.2.3.4");
            var language = "pt-BR";
            _mockedSdkMetadata.Setup(x => x.Version)
                              .Returns(currentVersion);
            _mockedSdkMetadata.Setup(x => x.UserAgent)
                              .Returns($"{Constants.USER_AGENT_PREFIX}/{currentVersion}");
            var handler = new UserAgentHttpMessageHandler(_mockedSdkMetadata.Object);
            handler.InnerHandler = Mock.Of<HttpMessageHandler>();
            var methodInfo = typeof(UserAgentHttpMessageHandler).GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var message = new HttpRequestMessage();
            message.Headers.UserAgent.TryParseAdd("PreSetAgent");
            message.Headers.AcceptLanguage.TryParseAdd(language);
            Assert.Single(message.Headers.UserAgent);
            Assert.Single(message.Headers.AcceptLanguage);

            // Act
            _ = methodInfo.Invoke(handler, new object[] { message, CancellationToken.None });

            // Assert
            Assert.Single(message.Headers.UserAgent);
            Assert.Collection(
                message.Headers.UserAgent,
                value =>
                {
                    Assert.NotNull(value.Product);
                    Assert.Equal(value.Product.ToString(), _mockedSdkMetadata.Object.UserAgent);
                });
            Assert.Single(message.Headers.AcceptLanguage);
            Assert.Collection(
                message.Headers.AcceptLanguage,
                value =>
                {
                    Assert.NotNull(value.Value);
                    Assert.Equal(value.Value.ToString(), language);
                });
        }
    }
}
