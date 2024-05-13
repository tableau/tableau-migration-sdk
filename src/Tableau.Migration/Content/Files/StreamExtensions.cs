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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Files
{
    internal static class StreamExtensions
    {
        public static async ValueTask WriteInitializationVectorAsync(
            this Stream stream,
            byte[] iv,
            CancellationToken cancel)
            => await stream.WriteAsync(iv, cancel).ConfigureAwait(false);

        public static async ValueTask<byte[]> ReadInitializationVectorAsync(
            this Stream stream,
            int ivLength,
            CancellationToken cancel)
        {
            byte[] iv = new byte[ivLength];
            int leftToRead = iv.Length;
            int totalBytesRead = 0;

            while (leftToRead > 0)
            {
                int bytesRead = await stream.ReadAsync(iv.AsMemory(totalBytesRead, leftToRead), cancel)
                    .ConfigureAwait(false);

                if (bytesRead is 0)
                {
                    break;
                }

                totalBytesRead += bytesRead;
                leftToRead -= bytesRead;
            }

            return iv;
        }
    }
}
