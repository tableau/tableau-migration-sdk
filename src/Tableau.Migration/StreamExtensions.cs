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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration
{
    internal static class StreamExtensions
    {
        internal static readonly byte[] ZIP_LEAD_BYTES = BitConverter.GetBytes(0x04034b50);

        public static async Task ProcessChunksAsync(
            this Stream stream,
            int chunkSizeBytes,
            Func<byte[], int, CancellationToken, Task<bool>> processChunkAsync,
            CancellationToken cancel)
        {
            byte[] chunkOrPartialChunk = new byte[chunkSizeBytes];
            int chunkReadBytes;

            while ((chunkReadBytes = await stream.ReadAsync(chunkOrPartialChunk.AsMemory(0, chunkSizeBytes), cancel)
                .ConfigureAwait(false)) > 0)
            {
                var @continue = await processChunkAsync(chunkOrPartialChunk, chunkReadBytes, cancel).ConfigureAwait(false);

                if (!@continue)
                    break;

                cancel.ThrowIfCancellationRequested();
            }
        }

        public static bool IsZip(this Stream stream)
        {
            if (!stream.CanSeek || !stream.CanRead)
                throw new ArgumentException("The stream is not seekable and readable.", nameof(stream));

            if (stream.Length < ZIP_LEAD_BYTES.Length)
                return false;

            var originalPosition = stream.Position;

            try
            {
                var bytes = new byte[ZIP_LEAD_BYTES.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(bytes, 0, bytes.Length);
                return bytes.SequenceEqual(ZIP_LEAD_BYTES);
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }
    }
}
