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
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;
using Tableau.Migration.Tests.Unit;
using Xunit;

namespace Tableau.Migration.Tests
{
    public class MockHttpClient : Mock<IHttpClient>
    {
        private readonly Action<HttpRequestMessage, CancellationToken> _sendAsyncCallback;
        private readonly Action<HttpRequestMessage, HttpCompletionOption, CancellationToken> _sendAsyncWithCompletionOptionCallback;

        private readonly ImmutableList<HttpRequestMessage>.Builder _sentRequests = ImmutableList.CreateBuilder<HttpRequestMessage>();

        public event EventHandler<HttpRequestMessage>? RequestSent;

        public IImmutableList<HttpRequestMessage> SentRequests => _sentRequests.ToImmutable();

        public MockHttpClient()
        {
            _sendAsyncCallback = (r, _) => OnRequestSent(r);
            _sendAsyncWithCompletionOptionCallback = (r, _, __) => OnRequestSent(r);

            Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Callback(_sendAsyncCallback);

            Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<HttpCompletionOption>(), It.IsAny<CancellationToken>()))
                .Callback(_sendAsyncWithCompletionOptionCallback);
        }

        #region - SendAsync Setup -

        #region - Expression Building -

        private static Expression<Func<IHttpClient, Task<IHttpResponseMessage>>> CreateSendAsyncExpression(HttpRequestMessage? request = null)
            => request is not null
                ? c => c.SendAsync(request, It.IsAny<CancellationToken>())
                : c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>());

        private static Expression<Func<IHttpClient, Task<IHttpResponseMessage<TContent>>>> CreateSendAsyncExpression<TContent>(HttpRequestMessage? request = null)
            where TContent : class
            => request is not null
                ? c => c.SendAsync<TContent>(request, It.IsAny<CancellationToken>())
                : c => c.SendAsync<TContent>(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>());

        private static Expression<Func<IHttpClient, Task<IHttpResponseMessage>>> CreateSendAsyncWithCompletionOptionExpression(HttpRequestMessage? request = null)
            => request is not null
                ? c => c.SendAsync(request, It.IsAny<HttpCompletionOption>(), It.IsAny<CancellationToken>())
                : c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<HttpCompletionOption>(), It.IsAny<CancellationToken>());

        #endregion

        #region - Callback Building -

        private Action<HttpRequestMessage, CancellationToken> BuildSendAsyncCallback(Action<HttpRequestMessage>? onRequestSent = null)
        {
            return (r, c) =>
            {
                _sendAsyncCallback.Invoke(r, c);
                onRequestSent?.Invoke(r);
            };
        }

        private Action<HttpRequestMessage, HttpCompletionOption, CancellationToken> BuildSendAsyncWithCompletionOptionCallback(Action<HttpRequestMessage>? onRequestSent = null)
        {
            return (r, o, c) =>
            {
                _sendAsyncWithCompletionOptionCallback.Invoke(r, o, c);
                onRequestSent?.Invoke(r);
            };
        }

        #endregion

        private IHttpResponseMessage SetupSendAsync(
            IHttpResponseMessage response,
            HttpRequestMessage? request = null,
            Action<HttpRequestMessage>? onRequestSent = null)
        {
            Setup(CreateSendAsyncExpression(request))
                .ReturnsAsync(response)
                .Callback(BuildSendAsyncCallback(onRequestSent));

            Setup(CreateSendAsyncWithCompletionOptionExpression(request))
                .ReturnsAsync(response)
                .Callback(BuildSendAsyncWithCompletionOptionCallback(onRequestSent));

            return response;
        }

        private IHttpResponseMessage<TContent> SetupSendAsync<TContent>(
            IHttpResponseMessage<TContent> response,
            HttpRequestMessage? request = null,
            Action<HttpRequestMessage>? onRequestSent = null)
            where TContent : class
        {
            Setup(CreateSendAsyncExpression<TContent>(request))
                .ReturnsAsync(response)
                .Callback(BuildSendAsyncCallback(onRequestSent));

            return response;
        }

        #endregion

        #region - Private Methods -

        private HttpRequestMessage OnRequestSent(HttpRequestMessage request)
        {
            _sentRequests.Add(request);
            RequestSent?.Invoke(this, request);
            return request;
        }

        #endregion

        #region - Response Setup -

        public IHttpResponseMessage<TContent> SetupResponse<TContent>(HttpRequestMessage request, IHttpResponseMessage<TContent> response)
            where TContent : class
            => SetupSendAsync(response, request);

        public IHttpResponseMessage SetupResponse(IHttpResponseMessage response)
            => SetupSendAsync(response);

        public IHttpResponseMessage<TContent> SetupResponse<TContent>(IHttpResponseMessage<TContent> response)
            where TContent : class
            => SetupSendAsync(response);

        public IHttpResponseMessage<TResponse> SetupResponse<TResponse>(Mock<IHttpResponseMessage<TResponse>> mockResponse)
            where TResponse : class
            => SetupSendAsync(mockResponse.Object);

        public IHttpResponseMessage<TResponse> SetupResponse<TResponse>(Mock<IHttpResponseMessage<TResponse>> mockResponse, HttpRequestMessage? request = null, Action <HttpRequestMessage>? onRequestSent = null)
           where TResponse : class
           => SetupSendAsync(mockResponse.Object, request, onRequestSent);

        public IHttpResponseMessage SetupResponse(Mock<IHttpResponseMessage> mockResponse)
            => SetupSendAsync(mockResponse.Object);

        #region - Success Responses -

        public IHttpResponseMessage SetupSuccessResponse(HttpContent? content = null)
            => SetupSendAsync(MockHttpResponseMessage.Success(content).Object);

        public (IHttpResponseMessage<TContent> Response, TContent Content) SetupSuccessResponse<TContent>(
            IFixture autoFixture,
            Action<TContent>? configureContent = null,
            Action<HttpRequestMessage>? onRequestSent = null)
            where TContent : TableauServerResponse, new()
            => SetupSuccessResponse(autoFixture.CreateResponse<TContent>(), configureContent, onRequestSent);

        public (IHttpResponseMessage<TContent> Response, TContent Content) SetupSuccessResponse<TContent>(
            TContent content,
            Action<TContent>? configureContent = null,
            Action<HttpRequestMessage>? onRequestSent = null)
            where TContent : TableauServerResponse, new()
        {
            configureContent?.Invoke(content);

            var response = SetupSendAsync(MockHttpResponseMessage<TContent>.Success(content).Object, onRequestSent: onRequestSent);

            return (response, content);
        }

        public (IHttpResponseMessage<TContent> Response, TContent Content, TItem Item) SetupSuccessResponse<TContent, TItem>(
            IFixture autoFixture,
            Action<TContent>? configureContent = null,
            Action<TItem>? configureItem = null)
            where TContent : TableauServerResponse<TItem>, new()
            where TItem : class
        {
            var content = autoFixture.CreateResponse<TContent>();

            configureContent?.Invoke(content);
            configureItem?.Invoke(content.Item!);

            return SetupSuccessResponse<TContent, TItem>(content);
        }

        public (IHttpResponseMessage<TContent> Response, TContent Content, TItem Item) SetupSuccessResponse<TContent, TItem>(TContent content)
            where TContent : TableauServerResponse<TItem>, new()
            where TItem : class
        {
            var response = SetupSendAsync(MockHttpResponseMessage<TContent>.Success(content).Object);

            return (response, content, content.Item!);
        }

        #endregion

        #region - Error Responses -

        public (IHttpResponseMessage Response, Error Error) SetupErrorResponse(
            IFixture autoFixture,
            Action<Error>? configureError = null)
        {
            var error = autoFixture.CreateErrorResponse().Error!;

            configureError?.Invoke(error);

            var response = SetupSendAsync(MockHttpResponseMessage.Error(error).Object);

            return (response, error);
        }

        public (IHttpResponseMessage<TContent> Response, Error Error) SetupErrorResponse<TContent>(Error error)
            where TContent : TableauServerResponse, new()
        {
            var response = SetupSendAsync(MockHttpResponseMessage<TContent>.Error(error).Object);

            return (response, error);
        }

        public (IHttpResponseMessage<TContent> Response, Error Error) SetupErrorResponse<TContent>(
            IFixture autoFixture,
            Action<Error>? configureError = null)
            where TContent : TableauServerResponse, new()
        {
            var error = autoFixture.CreateErrorResponse().Error!;

            configureError?.Invoke(error);

            return SetupErrorResponse<TContent>(error);
        }

        #endregion

        #region - Exception Responses -

        public (IHttpResponseMessage Response, Exception Exception) SetupExceptionResponse(Exception? exception = null)
        {
            exception ??= new Exception();

            var response = SetupSendAsync(MockHttpResponseMessage.Exception(exception).Object);

            return (response, exception);
        }

        public (IHttpResponseMessage Response, Exception Exception) SetupExceptionResponse()
            => SetupExceptionResponse(new Exception());

        public (IHttpResponseMessage<TContent> Response, Exception Exception) SetupExceptionResponse<TContent>(Exception? exception = null)
            where TContent : TableauServerResponse, new()
        {
            exception ??= new Exception();

            var response = SetupSendAsync(MockHttpResponseMessage<TContent>.Exception(exception).Object);

            return (response, exception);
        }

        #endregion

        #endregion

        #region - Request Assertions -

        public HttpRequestMessage AssertSingleRequest(Action<HttpRequestMessage>? assert = null)
        {
            var request = Assert.Single(_sentRequests);
            assert?.Invoke(request);
            return request;
        }

        public IImmutableList<HttpRequestMessage> AssertRequestCount(int count)
        {
            Assert.Equal(count, _sentRequests.Count);

            return SentRequests;
        }

        public IImmutableList<HttpRequestMessage> AssertAllRequests(int expectedCount, params Action<HttpRequestMessage>[] assert)
        {
            if (expectedCount != assert.Length)
                throw new ArgumentException($"{nameof(expectedCount)} ({expectedCount}) does not match {nameof(assert)}.{nameof(assert.Length)} ({assert.Length}.");

            AssertRequestCount(expectedCount);

            return AssertRequests(assert);
        }

        public IImmutableList<HttpRequestMessage> AssertRequests(params Action<HttpRequestMessage>[] assert)
        {
            Assert.True(_sentRequests.Count >= assert.Length);

            Assert.Collection(_sentRequests.Take(assert.Length), assert);

            return SentRequests;
        }

        public void AssertNoRequests() => Assert.Empty(_sentRequests);

        #endregion
    }
}
