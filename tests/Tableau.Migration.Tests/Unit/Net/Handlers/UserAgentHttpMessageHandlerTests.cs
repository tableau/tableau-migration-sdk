//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using Moq;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Handlers;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Handlers
{
    public class UserAgentHttpMessageHandlerTests
    {
        private readonly Mock<IUserAgentProvider> _mockedUserAgentProvider;

        public UserAgentHttpMessageHandlerTests()
        {
            _mockedUserAgentProvider = new Mock<IUserAgentProvider>();
        }

        [Fact]
        public void CallSendAsync_AttachUserAgentHeader()
        {
            // Arrange
            var currentVersion = new Version("1.2.3.4");
            _mockedUserAgentProvider.Setup(x => x.UserAgent)
                              .Returns($"{Constants.USER_AGENT_PREFIX}/{currentVersion}");
            var handler = new UserAgentHttpMessageHandler(_mockedUserAgentProvider.Object);
            handler.InnerHandler = Mock.Of<HttpMessageHandler>();
            var methodInfo = typeof(UserAgentHttpMessageHandler).GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var message = new HttpRequestMessage();
            Assert.Empty(message.Headers.UserAgent);

            // Act
            _ = methodInfo.Invoke(handler, [message, CancellationToken.None]);

            // Assert
            Assert.Single(message.Headers.UserAgent);
            Assert.Collection(
                message.Headers.UserAgent,
                value =>
                {
                    Assert.NotNull(value.Product);
                    Assert.Equal(value.Product.ToString(), _mockedUserAgentProvider.Object.UserAgent);
                });
        }

        [Fact]
        public void CallSendAsync_ReplaceUserAgentHeader()
        {
            // Arrange
            var currentVersion = new Version("10.12.23.44");
            _mockedUserAgentProvider.Setup(x => x.UserAgent)
                              .Returns($"{Constants.USER_AGENT_PREFIX}/{currentVersion}");
            var handler = new UserAgentHttpMessageHandler(_mockedUserAgentProvider.Object);
            handler.InnerHandler = Mock.Of<HttpMessageHandler>();
            var methodInfo = typeof(UserAgentHttpMessageHandler).GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var message = new HttpRequestMessage();
            message.Headers.UserAgent.TryParseAdd("PreSetAgent");
            Assert.Single(message.Headers.UserAgent);

            // Act
            _ = methodInfo.Invoke(handler, [message, CancellationToken.None]);

            // Assert
            Assert.Single(message.Headers.UserAgent);
            Assert.Collection(
                message.Headers.UserAgent,
                value =>
                {
                    Assert.NotNull(value.Product);
                    Assert.Equal(value.Product.ToString(), _mockedUserAgentProvider.Object.UserAgent);
                });
        }

        [Fact]
        public void CallSendAsync_ReplaceUserAgent_DontTouchOtherHeaders()
        {
            // Arrange
            var currentVersion = new Version("1.2.3.4");
            var language = "pt-BR";
            _mockedUserAgentProvider.Setup(x => x.UserAgent)
                              .Returns($"{Constants.USER_AGENT_PREFIX}/{currentVersion}");
            var handler = new UserAgentHttpMessageHandler(_mockedUserAgentProvider.Object);
            handler.InnerHandler = Mock.Of<HttpMessageHandler>();
            var methodInfo = typeof(UserAgentHttpMessageHandler).GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var message = new HttpRequestMessage();
            message.Headers.UserAgent.TryParseAdd("PreSetAgent");
            message.Headers.AcceptLanguage.TryParseAdd(language);
            Assert.Single(message.Headers.UserAgent);
            Assert.Single(message.Headers.AcceptLanguage);

            // Act
            _ = methodInfo.Invoke(handler, [message, CancellationToken.None]);

            // Assert
            Assert.Single(message.Headers.UserAgent);
            Assert.Collection(
                message.Headers.UserAgent,
                value =>
                {
                    Assert.NotNull(value.Product);
                    Assert.Equal(value.Product.ToString(), _mockedUserAgentProvider.Object.UserAgent);
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
