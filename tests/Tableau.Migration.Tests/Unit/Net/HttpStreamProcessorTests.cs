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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Config;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class HttpStreamProcessorTests
    {
        public abstract class TestObject
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            public override bool Equals(object? obj)
            {
                return obj is TestObject testObject && GetType() == testObject.GetType() && Id.Equals(testObject.Id);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Id);
            }
        }

        public class TestRequestObject : TestObject
        { }

        public class TestResponseObject : TableauServerResponse<TestObject>
        {
            public override TestObject? Item { get; set; }
        }

        public abstract class HttpStreamProcessorTest : AutoFixtureTestBase
        {
            protected event EventHandler<HttpRequestMessage>? OnRequestCreated;

            protected readonly MockHttpClient MockHttpClient = new();
            protected readonly Mock<IConfigReader> MockConfigReader = new();
            protected readonly MigrationSdkOptions MigrationSdkOptions = new();
            protected readonly List<HttpRequestMessage> CreatedRequests = new();

            internal readonly HttpStreamProcessor Processor;

            public HttpStreamProcessorTest()
            {
                MockConfigReader
                    .Setup(x => x.Get())
                    .Returns(MigrationSdkOptions);

                Processor = new(MockHttpClient.Object, MockConfigReader.Object);
            }

            protected HttpRequestMessage CreateRequest(byte[] chunk, int bytesRead)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, TestConstants.LocalhostUri)
                {
                    Content = new ByteArrayContent(chunk, 0, bytesRead)
                };

                CreatedRequests.Add(request);

                OnRequestCreated?.Invoke(this, request);

                return request;
            }

            protected IHttpResponseMessage<TResponse> SetupResponse<TResponse>(HttpRequestMessage request, IHttpResponseMessage<TResponse> response)
                where TResponse : TableauServerResponse
                => MockHttpClient.SetupResponse(request, response);

            protected IHttpResponseMessage<TResponse> SetupResponse<TResponse>(HttpRequestMessage request, Mock<IHttpResponseMessage<TResponse>> mockResponse)
                where TResponse : TableauServerResponse
                => SetupResponse(request, mockResponse.Object);
        }

        #region - ProcessAsync -

        public class ProcessAsync : HttpStreamProcessorTest
        {
            private class MemoryStreamStub : MemoryStream
            {
                private readonly long _stubLength;
                private long _stubPosition = 0;

                public MemoryStreamStub(long stubLength)
                    : base(1)
                {
                    _stubLength = stubLength;
                }

                public override long Length => _stubLength;

                public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                {
                    long n = _stubLength - _stubPosition;
                    if (n > count)
                        n = count;
                    if (n <= 0)
                        return Task.FromResult(0);

                    _stubPosition += n;

                    return Task.FromResult((int)n);
                }

                public override ValueTask<int> ReadAsync(
                    Memory<byte> buffer,
                    CancellationToken cancellationToken = default)
                {
                    var count = buffer.Length;
                    long n = _stubLength - _stubPosition;
                    if (n > count)
                        n = count;
                    if (n <= 0)
                        return ValueTask.FromResult(0);

                    _stubPosition += n;

                    return ValueTask.FromResult((int)n);
                }
            }

            private readonly bool _skipGithubWindowsRunnerTests;

            public ProcessAsync()
            {
                var skipGithubRunnerTests = Environment.GetEnvironmentVariable("MIGRATIONSDK_SKIP_GITHUB_RUNNER_TESTS");

                _skipGithubWindowsRunnerTests = (skipGithubRunnerTests?.Equals("yes", StringComparison.OrdinalIgnoreCase) ?? false) &&
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            }

            [Fact]
            public async Task SendsRequests()
            {
                // Arrange
                using var memoryStream = new MemoryStream(Constants.DefaultEncoding.GetBytes("Test"));

                OnRequestCreated += (o, r) => SetupResponse(r, new MockHttpResponseMessage<TestResponseObject>(HttpStatusCode.OK, new()).Object);

                var responses = await Processor.ProcessAsync<TestResponseObject>(
                    memoryStream,
                    CreateRequest,
                    Cancel);

                foreach (var request in CreatedRequests)
                    MockHttpClient.Verify(c => c.SendAsync<TestResponseObject>(request, Cancel), Times.Once());
            }

            [Fact]
            public async Task ConfigurationChange_ClientSendStreamWithRefreshedConfiguration()
            {
                // Arrange
                MigrationSdkOptions.Network.FileChunkSizeKB = 1;

                using var memoryStream1 = new MemoryStreamStub(100 * 1024);
                using var memoryStream2 = new MemoryStreamStub(100 * 1024);

                OnRequestCreated += (o, r) => SetupResponse(r, new MockHttpResponseMessage<TestResponseObject>(HttpStatusCode.OK, new()).Object);

                // Act/Assert
                var responses = await Processor.ProcessAsync<TestResponseObject>(
                    memoryStream1,
                    CreateRequest,
                    Cancel);

                Assert.Equal(100, responses.Count());

                Assert.All(
                    responses,
                    response => Assert.True(response.IsSuccessStatusCode));

                MigrationSdkOptions.Network.FileChunkSizeKB = 25;

                responses = await Processor.ProcessAsync<TestResponseObject>(
                    memoryStream2,
                    CreateRequest,
                    Cancel);

                Assert.Equal(4, responses.Count());

                Assert.All(
                    responses,
                    response => Assert.True(response.IsSuccessStatusCode));
            }

            [Theory]
            #region - Default Config Cases -
            // 1 MB
            [InlineData("PUT", 2)]
            // 5 MB
            [InlineData("POST", 10)]
            // 50 MB
            [InlineData("PUT", 100)]
            // 500 MB
            [InlineData("POST", 1000)]
            // 5 GB
            [InlineData("PUT", 10000)]
            #endregion
            // Lower than minimum configuration
            // 10 KB
            [InlineData("PUT", 10, 1L, 0)]
            // Larger than maximum configuration
            // 320 MB
            // expectedChunkSizeKB
            // 64 MB
            // configurationChunkSizeKB
            // 150 MB
            [InlineData("POST", 5, 64L * 1024, 150 * 1024)]
            // Custom Configuration
            // 105 MB
            // expectedChunkSizeKB
            // 15 MB
            // configurationChunkSizeKB
            // 15 MB
            [InlineData("PUT", 7, 15L * 1024, 15 * 1024)]
            // This test fails on Github Windows Runner with a 'System.OutOfMemoryException' 
            // The largest allowed document by Tableau Cloud
            // https://help.tableau.com/current/online/en-us/to_site_capacity.htm
            // 25000 GB
            // expectedChunkSizeKB
            // 64 MB
            // configurationChunkSizeKB
            // 64 MB
            [InlineData("POST", 500, 50L * 1024, 50 * 1024, true)]
            public async Task SendsStreamRequests(
                string method,
                int expectedRequests,
                long expectedChunkSizeKB = NetworkOptions.Defaults.FILE_CHUNK_SIZE_KB,
                int? configurationChunkSizeKB = null,
                bool skipOnGithubWindowsRunner = false)
            {
                if (skipOnGithubWindowsRunner && _skipGithubWindowsRunnerTests)
                {
                    return;
                }

                // Arrange
                var httpMethod = new HttpMethod(method);

                OnRequestCreated += (o, r) => SetupResponse(r, new MockHttpResponseMessage<TestResponseObject>(HttpStatusCode.OK, new()).Object);

                if (configurationChunkSizeKB is not null)
                {
                    MigrationSdkOptions.Network.FileChunkSizeKB = configurationChunkSizeKB.Value;
                }

                // requests * chunk Size * 1024 bytes in one KB
                using var memoryStream = new MemoryStreamStub(expectedRequests * expectedChunkSizeKB * 1024);

                // Act/Assert
                var responses = await Processor.ProcessAsync<TestResponseObject>(
                    memoryStream,
                    CreateRequest,
                    Cancel);

                Assert.Equal(expectedRequests, responses.Count());
                Assert.All(
                    responses,
                    response => Assert.True(response.IsSuccessStatusCode));
            }
        }

        #endregion
    }
}
