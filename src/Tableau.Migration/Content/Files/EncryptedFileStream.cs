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
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// <see cref="IContentFileStream"/> implementation that 
    /// owns and disposes of both the content stream and
    /// related cryptographic objects for encryption/decryption.
    /// </summary>
    public class EncryptedFileStream : ContentFileStream
    {
        private readonly IContentFileStream _innerStream;
        private readonly IDisposable _encryption;
        private readonly ICryptoTransform _transform;

        /// <summary>
        /// Creates a new <see cref="EncryptedFileStream"/> object.
        /// </summary>
        /// <param name="innerStream">The inner file stream.</param>
        /// <param name="encryption">The encryption object to take ownership of.</param>
        /// <param name="transform"></param>
        /// <param name="stream"></param>
        public EncryptedFileStream(IContentFileStream innerStream, IDisposable encryption, ICryptoTransform transform, CryptoStream stream)
            : base(stream)
        {
            _innerStream = innerStream;
            _encryption = encryption;
            _transform = transform;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public override async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await base.DisposeAsync().ConfigureAwait(false);

            _transform.Dispose();
            _encryption.Dispose();

            // Cleanup the inner stream, CryptoStream should have
            // disposed of the underlying _innerStream.Content
            // but the inner stream may have other things it needs to clean up.
            await _innerStream.DisposeAsync().ConfigureAwait(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
    }
}
