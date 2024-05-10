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
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class StreamExtensionsTests
    {
        public abstract class StreamExtensionsTest : AutoFixtureTestBase, IDisposable
        {
            private readonly List<Stream> _streams = new();

            protected Stream CreateStream(int generatedByteCount = 50, IEnumerable<byte>? firstBytes = null)
            {
                var data = CreateMany(generatedByteCount, firstBytes).ToArray();

                var stream = new MemoryStream(data);

                _streams.Add(stream);

                return stream;
            }

            public void Dispose()
            {
                _streams.ForEach(s => s.Dispose());
                CancelSource.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        public class ProcessChunksAsync : StreamExtensionsTest
        {
            [Fact]
            public async Task Processes_chunks()
            {
                using var stream = CreateStream(100);

                var chunkSize = 25;
                var chunkCount = 0;

                await stream.ProcessChunksAsync(
                    chunkSize,
                    (data, size, cancel) =>
                    {
                        Assert.Equal(chunkSize, size);
                        chunkCount++;
                        return Task.FromResult(true);
                    },
                    CancelSource.Token);

                Assert.Equal(4, chunkCount);
            }

            [Fact]
            public async Task Breaks_when_specified()
            {
                using var stream = CreateStream(50);

                var chunkSize = 25;
                var chunkCount = 0;

                await stream.ProcessChunksAsync(
                    chunkSize,
                    (data, size, cancel) =>
                    {
                        Assert.Equal(chunkSize, size);
                        chunkCount++;
                        return Task.FromResult(false);
                    },
                    CancelSource.Token);

                Assert.Equal(1, chunkCount);
            }

            [Fact]
            public async Task Throws_when_canceled()
            {
                using var stream = CreateStream(50);

                var chunkSize = 25;
                var chunkCount = 0;
                var isCanceled = false;

                try
                {
                    await stream.ProcessChunksAsync(
                        chunkSize,
                        (data, size, cancel) =>
                        {
                            Assert.Equal(chunkSize, size);
                            chunkCount++;
                            CancelSource.Cancel();
                            return Task.FromResult(true);
                        },
                        CancelSource.Token);
                }
                catch (OperationCanceledException)
                {
                    isCanceled = true;
                }

                Assert.Equal(1, chunkCount);
                Assert.True(isCanceled);
            }
        }

        public class IsZip : StreamExtensionsTest
        {
            [Fact]
            public void True()
            {
                var stream = CreateStream(firstBytes: StreamExtensions.ZIP_LEAD_BYTES);

                Assert.True(stream.IsZip());
            }

            [Fact]
            public void False_when_stream_length_is_too_short()
            {
                var bytes = StreamExtensions.ZIP_LEAD_BYTES.Take(3);

                var stream = CreateStream(0, bytes);

                Assert.False(stream.IsZip());
            }

            [Fact]
            public void False_when_not_zip_header()
            {
                var stream = CreateStream();

                Assert.False(stream.IsZip());
            }

            [Fact]
            public void False_when_zip_bytes_are_not_first()
            {
                var stream = CreateStream(firstBytes: new[] { Create<byte>() }.Concat(StreamExtensions.ZIP_LEAD_BYTES));

                Assert.False(stream.IsZip());
            }

            [Theory]
            [InlineData(false, false)]
            [InlineData(false, true)]
            [InlineData(true, false)]
            public void Throws_when_stream_is_not_seekable_and_readable(bool canSeek, bool canRead)
            {
                var mockStream = new Mock<Stream>();
                mockStream.SetupGet(s => s.CanSeek).Returns(canSeek);
                mockStream.SetupGet(s => s.CanRead).Returns(canRead);

                Assert.Throws<ArgumentException>(() => mockStream.Object.IsZip());
            }

            [Fact]
            public void Reads_from_beginning()
            {
                var position = 5;

                var stream = CreateStream(firstBytes: StreamExtensions.ZIP_LEAD_BYTES);

                stream.Seek(position, SeekOrigin.Begin);

                Assert.True(stream.IsZip());
            }

            [Fact]
            public void Resets_position()
            {
                var position = 5;

                var stream = CreateStream(firstBytes: StreamExtensions.ZIP_LEAD_BYTES);

                stream.Seek(position, SeekOrigin.Begin);

                stream.IsZip();

                Assert.Equal(position, stream.Position);
            }

            [Fact]
            public void Resets_position_on_exception()
            {
                var position = 5;

                var mockStream = new Mock<Stream>();
                mockStream.SetupGet(s => s.CanSeek).Returns(true);
                mockStream.SetupGet(s => s.CanRead).Returns(true);
                mockStream.SetupGet(s => s.Position).Returns(position);

                mockStream.Setup(s => s.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception());

                mockStream.Object.IsZip();

                Assert.Equal(position, mockStream.Object.Position);
            }
        }
    }
}
