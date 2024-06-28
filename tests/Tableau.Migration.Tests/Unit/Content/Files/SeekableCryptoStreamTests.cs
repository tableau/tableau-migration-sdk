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
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.IO;
using Moq;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Tests.Reflection;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class SeekableCryptoStreamTests
    {
        public abstract class SeekableCryptoStreamTest : EncryptedFileStoreTests.EncryptedFileStoreTest
        {
            protected readonly StreamFactory<RecyclableMemoryStream> StreamFactory = new(MemoryStreamManager.Instance.GetStream);

            protected readonly Mock<ICryptoTransform> MockTransform = new();

            public SeekableCryptoStreamTest()
            { }

            protected SeekableCryptoStream CreateCryptoStream(CryptoStreamMode mode, bool leaveOpen)
                => CreateCryptoStream(CreateInnerStream(), mode, leaveOpen);

            protected SeekableCryptoStream CreateCryptoStream(Stream innerStream, CryptoStreamMode mode, bool leaveOpen)
                => CreateCryptoStream(innerStream, MockTransform.Object, mode, leaveOpen);

            protected SeekableCryptoStream CreateCryptoStream(Stream innerStream, ICryptoTransform transform, CryptoStreamMode mode, bool leaveOpen)
                => new(innerStream, transform, mode, leaveOpen);

            protected static Stream CreateInnerStream(byte[] data)
                => MemoryStreamManager.Instance.GetStream(data);

            protected static Stream CreateInnerStream(string text)
                => CreateInnerStream(Constants.DefaultEncoding.GetBytes(text));

            protected Stream CreateInnerStream(int textLength = 100)
                => CreateInnerStream(CreateString(textLength));

            protected static Mock<Stream> CreateMockInnerStream(bool canRead, bool canWrite)
            {
                var mockStream = new Mock<Stream>()
                {
                    CallBase = true
                };

                mockStream.SetupGet(s => s.CanSeek).Returns(true);
                mockStream.SetupGet(s => s.CanRead).Returns(canRead);
                mockStream.SetupGet(s => s.CanWrite).Returns(canWrite);

                return mockStream;
            }

            protected static Mock<Stream> CreateMockInnerStream(CryptoStreamMode mode)
                => CreateMockInnerStream(mode is CryptoStreamMode.Read, mode is CryptoStreamMode.Write);

            protected Mock<Stream> CreateMockInnerStream()
                => CreateMockInnerStream(Create<CryptoStreamMode>());

            protected static void AssertCryptoStream(SeekableCryptoStream stream, CryptoStreamMode mode, Action<SeekableCryptoStream>? assert = null)
            {
                Assert.True(stream.CanSeek);

                Assert.Equal(mode, stream.Mode);

                Assert.Equal(mode is CryptoStreamMode.Read, stream.CanRead);
                Assert.Equal(mode is CryptoStreamMode.Write, stream.CanWrite);

                assert?.Invoke(stream);
            }

            protected static void AssertDisposed(SeekableCryptoStream cryptoStream, bool expectedInnerStreamDisposed = true)
            {
                var wrapper = SeekableCryptoStreamWrapper.InstanceFor(cryptoStream);

                var mockInnerStream = wrapper.GetMockFieldValue(w => w.InnerStream);

                mockInnerStream.AssertDisposed(expectedInnerStreamDisposed);
            }
        }

        public class Ctor : SeekableCryptoStreamTest
        {
            [Theory]
            [EnumData<CryptoStreamMode>]
            public void Initializes(CryptoStreamMode mode)
            {
                var innerStream = CreateMockInnerStream(mode).Object;

                using var cryptoStream = new SeekableCryptoStream(innerStream, MockTransform.Object, mode, false);

                AssertCryptoStream(cryptoStream, mode);
            }

            [Theory]
            [EnumData<CryptoStreamMode>]
            public async Task Disposes_inner_stream_when_leaveOpen_is_false(CryptoStreamMode mode)
            {
                var mockInnerStream = CreateMockInnerStream(mode);

                var cryptoStream = new SeekableCryptoStream(mockInnerStream.Object, MockTransform.Object, mode, false);

                AssertCryptoStream(cryptoStream, mode);

                await cryptoStream.DisposeAsync();

                AssertDisposed(cryptoStream);
            }

            [Theory]
            [EnumData<CryptoStreamMode>]
            public async Task Does_not_dispose_inner_stream_when_leaveOpen_is_true(CryptoStreamMode mode)
            {
                var mockInnerStream = CreateMockInnerStream(mode);

                var cryptoStream = new SeekableCryptoStream(mockInnerStream.Object, MockTransform.Object, mode, true);

                AssertCryptoStream(cryptoStream, mode);

                await cryptoStream.DisposeAsync();

                AssertDisposed(cryptoStream, false);
            }

            [Theory]
            [Values(true, false)]
            public async Task Respects_leaveOpen_flag(bool leaveOpen)
            {
                var mode = Create<CryptoStreamMode>();

                var mockInnerStream = CreateMockInnerStream(mode);

                var cryptoStream = new SeekableCryptoStream(mockInnerStream.Object, MockTransform.Object, mode, leaveOpen);

                AssertCryptoStream(cryptoStream, mode);

                await cryptoStream.DisposeAsync();

                AssertDisposed(cryptoStream, !leaveOpen);
            }
        }

        public class Position : SeekableCryptoStreamTest
        {
            [Fact]
            public async Task Gets()
            {
                var length = 25;

                var innerStream = CreateInnerStream(length);

                await using var cryptoStream = new SeekableCryptoStream(innerStream, MockTransform.Object, CryptoStreamMode.Read, false);

                for (var i = 0; i != length; i++)
                {
                    cryptoStream.Seek(i, SeekOrigin.Begin);

                    Assert.Equal(i, cryptoStream.Position);
                }
            }

            [Fact]
            public async Task Sets()
            {
                var length = 25;

                var innerStream = CreateInnerStream(length);

                await using var cryptoStream = new SeekableCryptoStream(innerStream, MockTransform.Object, CryptoStreamMode.Read, false);

                for (var i = 0; i != length; i++)
                {
                    cryptoStream.Position = i;
                }
            }
        }

        public class Roundtrip : SeekableCryptoStreamTest
        {
            [Fact]
            public async Task Reads_and_writes()
            {
                var content = CreateString(100);

                using var aes = Aes.Create();
                aes.GenerateIV();

                var key = aes.Key;
                var iv = aes.IV;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                await using var innerStream = MemoryStreamManager.Instance.GetStream();

                await using (var writeStream = CreateCryptoStream(innerStream, encryptor, CryptoStreamMode.Write, true))
                {
                    await writeStream.WriteInitializationVectorAsync(iv, Cancel);
                    writeStream.Write(Constants.DefaultEncoding.GetBytes(content));
                }

                innerStream.Seek(0, SeekOrigin.Begin);

                using var encryptedReader = new StreamReader(innerStream, Constants.DefaultEncoding);

                var encrypted = await encryptedReader.ReadToEndAsync();

                Assert.NotEqual(content, encrypted);

                innerStream.Seek(0, SeekOrigin.Begin);

                iv = await innerStream.ReadInitializationVectorAsync(iv.Length, Cancel);

                var decryptor = aes.CreateDecryptor(aes.Key, iv);

                await using var decryptedStream = CreateCryptoStream(innerStream, decryptor, CryptoStreamMode.Read, true);

                using var decryptedReader = new StreamReader(decryptedStream, Constants.DefaultEncoding);

                var decrypted = await decryptedReader.ReadToEndAsync();

                Assert.Equal(content, decrypted);
            }
        }
    }
}
