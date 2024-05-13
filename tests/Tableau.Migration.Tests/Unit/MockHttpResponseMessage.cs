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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using AutoFixture;
using Moq;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Tests.Unit
{
    public abstract class MockHttpResponseMessageBase<TMockResponseMessage, TResponseMessage> : Mock<TResponseMessage>
        where TMockResponseMessage : Mock<TResponseMessage>, new()
        where TResponseMessage : class, IHttpResponseMessage
    {
        protected MockHttpResponseMessageBase()
        {
            SetupGet(x => x.Headers).Returns(new Mock<HttpHeaders>().Object);
        }

        protected static TMockResponseMessage WithStatusCode(TMockResponseMessage mockResponse, HttpStatusCode statusCode)
            => mockResponse.WithStatusCode<TMockResponseMessage, TResponseMessage>(statusCode);

        protected static TMockResponseMessage WithSuccessStatusCode(TMockResponseMessage mockResponse)
            => mockResponse.WithSuccessStatusCode<TMockResponseMessage, TResponseMessage>();

        protected static TMockResponseMessage WithContent(TMockResponseMessage mockResponse, HttpContent? content)
            => mockResponse.WithContent<TMockResponseMessage, TResponseMessage>(content);

        protected static TMockResponseMessage WithEnsureSuccessStatusCodeException(TMockResponseMessage mockResponse, Exception exception)
            => mockResponse.WithEnsureSuccessStatusCodeException<TMockResponseMessage, TResponseMessage>(exception);

        public abstract TMockResponseMessage WithStatusCode(HttpStatusCode statusCode);

        public abstract TMockResponseMessage WithSuccessStatusCode();

        public abstract TMockResponseMessage WithContent(HttpContent? content);

        public abstract TMockResponseMessage WithEnsureSuccessStatusCodeException(Exception exception);

        public static TMockResponseMessage Success(HttpContent? content = null)
            => new TMockResponseMessage()
                .WithSuccessStatusCode<TMockResponseMessage, TResponseMessage>()
                .WithContent<TMockResponseMessage, TResponseMessage>(content);

        public static TMockResponseMessage Exception(Exception exception, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            => new TMockResponseMessage()
                .WithStatusCode<TMockResponseMessage, TResponseMessage>(statusCode)
                .WithEnsureSuccessStatusCodeException<TMockResponseMessage, TResponseMessage>(exception);
    }

    public class MockHttpResponseMessage : MockHttpResponseMessageBase<MockHttpResponseMessage, IHttpResponseMessage>
    {
        public MockHttpResponseMessage()
            : this(null)
        { }

        public MockHttpResponseMessage(HttpContent? content)
            : this(HttpStatusCode.OK, content)
        { }

        public MockHttpResponseMessage(HttpStatusCode statusCode, HttpContent? content = null)
            => WithStatusCode(statusCode).WithContent(content);

        public override MockHttpResponseMessage WithStatusCode(HttpStatusCode statusCode)
            => WithStatusCode(this, statusCode);

        public override MockHttpResponseMessage WithSuccessStatusCode()
            => WithSuccessStatusCode(this);

        public override MockHttpResponseMessage WithContent(HttpContent? content)
            => WithContent(this, content);

        public override MockHttpResponseMessage WithEnsureSuccessStatusCodeException(Exception exception)
            => WithEnsureSuccessStatusCodeException(this, exception);

        public static MockHttpResponseMessage Error(Error error, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            => new MockHttpResponseMessage()
                .WithStatusCode(statusCode)
                .WithContent(HttpContentSerializer.Instance.Serialize(new TestTableauServerResponse { Error = error }, MediaTypes.Xml));

        public static MockHttpResponseMessage Error(IFixture autoFixture, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            => Error(autoFixture.CreateErrorResponse().Error!, statusCode);
    }

    public class MockHttpResponseMessage<TContent> : MockHttpResponseMessageBase<MockHttpResponseMessage<TContent>, IHttpResponseMessage<TContent>>
        where TContent : TableauServerResponse, new()
    {
        public MockHttpResponseMessage()
            : this(null)
        { }

        public MockHttpResponseMessage(TContent? deserializedContent)
            : this(HttpStatusCode.OK, deserializedContent)
        { }

        public MockHttpResponseMessage(HttpStatusCode statusCode, TContent? deserializedContent = null)
            => WithStatusCode(statusCode).WithDeserializedContent(deserializedContent);

        public override MockHttpResponseMessage<TContent> WithStatusCode(HttpStatusCode statusCode)
            => WithStatusCode(this, statusCode);

        public override MockHttpResponseMessage<TContent> WithSuccessStatusCode()
            => WithSuccessStatusCode(this);

        public override MockHttpResponseMessage<TContent> WithContent(HttpContent? content)
            => WithContent(this, content);

        public override MockHttpResponseMessage<TContent> WithEnsureSuccessStatusCodeException(Exception exception)
            => WithEnsureSuccessStatusCodeException(this, exception);

        public MockHttpResponseMessage<TContent> WithDeserializedContent(TContent? content)
            => this.WithDeserializedContent<MockHttpResponseMessage<TContent>, IHttpResponseMessage<TContent>, TContent>(content);

        public static MockHttpResponseMessage<TContent> Success(TContent? content = null)
            => new MockHttpResponseMessage<TContent>()
                .WithSuccessStatusCode()
                .WithDeserializedContent(content);

        public static MockHttpResponseMessage<TContent> Error(Error error, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            => new MockHttpResponseMessage<TContent>()
                .WithStatusCode(statusCode)
                .WithDeserializedContent(new TContent { Error = error });

        public static MockHttpResponseMessage<TContent> Error(IFixture autoFixture, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            => Error(autoFixture.CreateErrorResponse().Error!, statusCode);
    }
}