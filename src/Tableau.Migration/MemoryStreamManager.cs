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
using Microsoft.IO;

namespace Tableau.Migration
{
    /// <summary>
    /// Default <see cref="IMemoryStreamManager"/> implementation. Wrapper class for <see cref="RecyclableMemoryStreamManager"/>.
    /// </summary>
    internal class MemoryStreamManager : IMemoryStreamManager
    {
        public static readonly MemoryStreamManager Instance = new();

        public RecyclableMemoryStreamManager Inner { get; }

        public MemoryStreamManager()
            : this(new RecyclableMemoryStreamManager.Options())
        { }

        public MemoryStreamManager(RecyclableMemoryStreamManager.Options options)
        {
            Inner = new(options);
        }

        #region - IMemoryManager -

        public virtual RecyclableMemoryStreamManager.Options Settings => Inner.Settings;

        public virtual RecyclableMemoryStream GetStream() => Inner.GetStream();

        public virtual RecyclableMemoryStream GetStream(Guid id) => Inner.GetStream(id);

        public virtual RecyclableMemoryStream GetStream(string? tag) => Inner.GetStream(tag);

        public virtual RecyclableMemoryStream GetStream(Guid id, string? tag) => Inner.GetStream(id, tag);

        public virtual RecyclableMemoryStream GetStream(string? tag, long requiredSize) => Inner.GetStream(tag, requiredSize);

        public virtual RecyclableMemoryStream GetStream(Guid id, string? tag, long requiredSize) => Inner.GetStream(id, tag, requiredSize);

        public virtual RecyclableMemoryStream GetStream(Guid id, string? tag, long requiredSize, bool asContiguousBuffer) => Inner.GetStream(id, tag, requiredSize, asContiguousBuffer);

        public virtual RecyclableMemoryStream GetStream(string? tag, long requiredSize, bool asContiguousBuffer) => Inner.GetStream(tag, requiredSize, asContiguousBuffer);

        public virtual RecyclableMemoryStream GetStream(Guid id, string? tag, byte[] buffer, int offset, int count) => Inner.GetStream(id, tag, buffer, offset, count);

        public virtual RecyclableMemoryStream GetStream(byte[] buffer) => Inner.GetStream(buffer);

        public virtual RecyclableMemoryStream GetStream(string? tag, byte[] buffer, int offset, int count) => Inner.GetStream(tag, buffer, offset, count);

        public virtual RecyclableMemoryStream GetStream(Guid id, string? tag, ReadOnlySpan<byte> buffer) => Inner.GetStream(id, tag, buffer);

        public virtual RecyclableMemoryStream GetStream(ReadOnlySpan<byte> buffer) => Inner.GetStream(buffer);

        public virtual RecyclableMemoryStream GetStream(string? tag, ReadOnlySpan<byte> buffer) => Inner.GetStream(tag,  buffer);

        #endregion
    }
}
