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
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Config;

namespace Tableau.Migration
{
    internal abstract class StreamProcessorBase
    {
        private const int KILOBYTE = 1024;
        // Max upload size allowed by Tableau Server - 64 MB in KB
        // See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_publish.htm#uploading-the-file-in-parts">Tableau API Reference</see> for documentation.
        public const int MAX_UPLOAD_CHUNK_SIZE_KB = 64 * KILOBYTE;
        public const int MIN_UPLOAD_CHUNK_SIZE_KB = 1;

        private readonly IConfigReader _configReader;

        public StreamProcessorBase(IConfigReader configReader)
        {
            _configReader = configReader;
        }

        protected async Task<IEnumerable<TResult>> ProcessAsync<TChunk, TResult>(
            Stream stream,
            Func<byte[], int, TChunk> buildChunk,
            Func<TChunk, CancellationToken, Task<(TResult Result, bool Continue)>> processChunkAsync,
            CancellationToken cancel)
            where TResult : class
        {
            var results = new List<TResult>();
            var chunkSizeBytes = GetChunkSizeBytes();

            await stream.ProcessChunksAsync(
                chunkSizeBytes,
                async (chunk, bytesRead, c) =>
                {
                    var builtChunk = buildChunk(chunk, bytesRead);

                    c.ThrowIfCancellationRequested();

                    var result = await processChunkAsync(builtChunk, c).ConfigureAwait(false);

                    results.Add(result.Result);

                    if (!result.Continue)
                    {
                        return false;
                    }

                    return true;
                },
                cancel)
                .ConfigureAwait(false);

            return results;
        }

        private int GetChunkSizeBytes()
        {
            var chunkSizeKB = _configReader.Get().Network.FileChunkSizeKB;

            if (chunkSizeKB < 1)
            {
                chunkSizeKB = MIN_UPLOAD_CHUNK_SIZE_KB;
            }
            else if (chunkSizeKB > MAX_UPLOAD_CHUNK_SIZE_KB)
            {
                chunkSizeKB = MAX_UPLOAD_CHUNK_SIZE_KB;
            }

            return chunkSizeKB * KILOBYTE;
        }
    }
}
