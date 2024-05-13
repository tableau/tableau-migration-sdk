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

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// <see cref="CryptoStream"/> implementation that supports seeking.
    /// </summary>
    public sealed class SeekableCryptoStream : CryptoStream
    {
        private readonly Stream _innerStream;

        /// <summary>
        /// Gets the read/write mode for the stream.
        /// </summary>
        internal readonly CryptoStreamMode Mode;

        #region - Seek Support Overrides -

        /// <inheritdoc/>
        public override bool CanSeek { get; } = true;

        /// <inheritdoc/>
        public override long Position
        {
            get => _innerStream.Position;
            set => _innerStream.Position = value;
        }

        /// <inheritdoc/>
        public override long Length => _innerStream.Length;

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);

        #endregion

        /// <summary>
        /// Creates a new <see cref="SeekableCryptoStream"/> instance.
        /// </summary>
        /// <param name="innerStream">The inner stream to encrypt or decrypt.</param>
        /// <param name="cryptoTransform">The encryption for the stream.</param>
        /// <param name="mode">The read/write mode for the stream.</param>
        /// <param name="leaveOpen">Whether this instance should take ownership of the inner stream.</param>
        public SeekableCryptoStream(Stream innerStream, ICryptoTransform cryptoTransform, CryptoStreamMode mode, bool leaveOpen)
            : base(innerStream, cryptoTransform, mode, leaveOpen)
        {
            if (!innerStream.CanSeek)
                throw new ArgumentException("The stream must be seekable.", nameof(innerStream));

            Mode = mode;

            _innerStream = innerStream;
        }
    }
}
