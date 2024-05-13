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
using Moq;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class EncryptedFileStreamTests
    {
        public class TestStream : CryptoStream
        {
            public TestStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode)
                : base(stream, transform, mode)
            { }

            public bool Disposed { get; private set; }

            protected override void Dispose(bool disposing)
            {
                Disposed = true;
                base.Dispose(disposing);
            }

            public override ValueTask DisposeAsync()
            {
                Disposed = true;
                GC.SuppressFinalize(this);
                return base.DisposeAsync();
            }
        }

        public class DisposeAsync
        {
            [Fact]
            public async Task TaskDisposesAllOwnedItemsAsync()
            {
                var mockInnerStream = new Mock<IContentFileStream>();

                var mockAes = new Mock<IDisposable>();
                var mockTransform = new Mock<ICryptoTransform>();

                var s = new TestStream(new MemoryStream(), mockTransform.Object, CryptoStreamMode.Read);

                var encryptedStream = new EncryptedFileStream(mockInnerStream.Object, mockAes.Object, mockTransform.Object, s);

                await encryptedStream.DisposeAsync();

                Assert.True(s.Disposed);
                mockTransform.Verify(x => x.Dispose(), Times.Once);
                mockAes.Verify(x => x.Dispose(), Times.Once);
                mockInnerStream.Verify(x => x.DisposeAsync(), Times.Once);
            }
        }
    }
}
