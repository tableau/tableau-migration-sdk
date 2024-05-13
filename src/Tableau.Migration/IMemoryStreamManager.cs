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
    /// Wrapper interface for <see cref="RecyclableMemoryStreamManager"/>.
    /// </summary>
    public interface IMemoryStreamManager
    {
        /// <summary>
        /// Gets the wrapped <see cref="RecyclableMemoryStreamManager"/> instance.
        /// </summary>
        RecyclableMemoryStreamManager Inner { get; }

        /// <summary>
        /// Gets the settings for configuring stream behavior.
        /// </summary>
        RecyclableMemoryStreamManager.Options Settings { get; }

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with no tag and a default initial capacity.
        /// </summary>
        /// <remarks>The stream's ID and tag are used for tracking purposes and not for caching.</remarks>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream();

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with no tag and a default initial capacity.
        /// </summary>
        /// <remarks>The stream's ID and tag are used for tracking purposes and not for caching.</remarks>
        /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream(Guid id);

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with the given tag and a default initial capacity.
        /// </summary>
        /// <remarks>The stream's ID and tag are used for tracking purposes and not for caching.</remarks>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream(string? tag);

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with the given tag and a default initial capacity.
        /// </summary>
        /// <remarks>The stream's ID and tag are used for tracking purposes and not for caching.</remarks>
        /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream(Guid id, string? tag);

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with the given tag and at least the given capacity.
        /// </summary>
        /// <remarks>The stream's ID and tag are used for tracking purposes and not for caching.</remarks>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <param name="requiredSize">The minimum desired capacity for the stream.</param>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream(string? tag, long requiredSize);

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with the given tag and at least the given capacity.
        /// </summary>
        /// <remarks>The stream's ID and tag are used for tracking purposes and not for caching.</remarks>
        /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <param name="requiredSize">The minimum desired capacity for the stream.</param>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream(Guid id, string? tag, long requiredSize);

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with the given tag and at least the given capacity, possibly using
        /// a single contiguous underlying buffer.
        /// </summary>
        /// <remarks>
        /// <para>The stream's ID and tag are used for tracking purposes and not for caching.</para>
        /// <para>
        /// Retrieving a <see cref="RecyclableMemoryStream"/> which provides a single contiguous buffer can be useful in situations
        /// where the initial size is known and it is desirable to avoid copying data between the smaller underlying
        /// buffers to a single large one. This is most helpful when you know that you will always call <see cref="RecyclableMemoryStream.GetBuffer"/>
        /// on the underlying stream.
        /// </para>
        /// </remarks>
        /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <param name="requiredSize">The minimum desired capacity for the stream.</param>
        /// <param name="asContiguousBuffer">Whether to attempt to use a single contiguous buffer.</param>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream(Guid id, string? tag, long requiredSize, bool asContiguousBuffer);

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with the given tag and at least the given capacity, possibly using
        /// a single contiguous underlying buffer.
        /// </summary>
        /// <remarks>
        /// <para>The stream's ID and tag are used for tracking purposes and not for caching.</para>
        /// <para>
        /// Retrieving a <see cref="RecyclableMemoryStream"/> which provides a single contiguous buffer can be useful in situations
        /// where the initial size is known and it is desirable to avoid copying data between the smaller underlying
        /// buffers to a single large one. This is most helpful when you know that you will always call <see cref="RecyclableMemoryStream.GetBuffer"/>
        /// on the underlying stream.
        /// </para>
        /// </remarks>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <param name="requiredSize">The minimum desired capacity for the stream.</param>
        /// <param name="asContiguousBuffer">Whether to attempt to use a single contiguous buffer.</param>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream(string? tag, long requiredSize, bool asContiguousBuffer);

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with the given tag and with contents copied from the provided
        /// buffer. The provided buffer is not wrapped or used after construction.
        /// </summary>
        /// <remarks>
        /// <para>The stream's ID and tag are used for tracking purposes and not for caching.</para>
        /// <para>The new stream's position is set to the beginning of the stream when returned.</para>
        /// </remarks>
        /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <param name="buffer">The byte buffer to copy data from.</param>
        /// <param name="offset">The offset from the start of the buffer to copy from.</param>
        /// <param name="count">The number of bytes to copy from the buffer.</param>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream(Guid id, string? tag, byte[] buffer, int offset, int count);

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with the contents copied from the provided
        /// buffer. The provided buffer is not wrapped or used after construction.
        /// </summary>
        /// <remarks>
        /// <para>The stream's ID and tag are used for tracking purposes and not for caching.</para>
        /// <para>The new stream's position is set to the beginning of the stream when returned.</para>
        /// </remarks>
        /// <param name="buffer">The byte buffer to copy data from.</param>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream(byte[] buffer);

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with the given tag and with contents copied from the provided
        /// buffer. The provided buffer is not wrapped or used after construction.
        /// </summary>
        /// <remarks>
        /// <para>The stream's ID and tag are used for tracking purposes and not for caching.</para>
        /// <para>The new stream's position is set to the beginning of the stream when returned.</para>
        /// </remarks>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <param name="buffer">The byte buffer to copy data from.</param>
        /// <param name="offset">The offset from the start of the buffer to copy from.</param>
        /// <param name="count">The number of bytes to copy from the buffer.</param>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream(string? tag, byte[] buffer, int offset, int count);

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with the given tag and with contents copied from the provided
        /// buffer. The provided buffer is not wrapped or used after construction.
        /// </summary>
        /// <remarks>
        /// <para>The stream's ID and tag are used for tracking purposes and not for caching.</para>
        /// <para>The new stream's position is set to the beginning of the stream when returned.</para>
        /// </remarks>
        /// <param name="id">A unique identifier which can be used to trace usages of the stream.</param>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <param name="buffer">The byte buffer to copy data from.</param>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream(Guid id, string? tag, ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with the contents copied from the provided
        /// buffer. The provided buffer is not wrapped or used after construction.
        /// </summary>
        /// <remarks>
        /// <para>The stream's ID and tag are used for tracking purposes and not for caching.</para>
        /// <para>The new stream's position is set to the beginning of the stream when returned.</para>
        /// </remarks>
        /// <param name="buffer">The byte buffer to copy data from.</param>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Retrieve a new <see cref="RecyclableMemoryStream"/> object with the given tag and with contents copied from the provided
        /// buffer. The provided buffer is not wrapped or used after construction.
        /// </summary>
        /// <remarks>
        /// <para>The stream's ID and tag are used for tracking purposes and not for caching.</para>
        /// <para>The new stream's position is set to the beginning of the stream when returned.</para>
        /// </remarks>
        /// <param name="tag">A tag which can be used to track the source of the stream.</param>
        /// <param name="buffer">The byte buffer to copy data from.</param>
        /// <returns>A <see cref="RecyclableMemoryStream"/>.</returns>
        RecyclableMemoryStream GetStream(string? tag, ReadOnlySpan<byte> buffer);
    }
}
