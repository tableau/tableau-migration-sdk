// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Linq;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// Object that represents a definition of a content type
    /// that a pipeline migrates.
    /// </summary>
    /// <param name="ContentType">The content type.</param>
    /// <param name="PublishType">The publish type.</param>
    /// <param name="ResultType">The post-publish result type.</param>
    public record MigrationPipelineContentType(Type ContentType, Type PublishType, Type ResultType)
    {
        /// <summary>
        /// Creates a new <see cref="MigrationPipelineContentType"/> instance.
        /// </summary>
        /// <param name="contentType">The single content type also used for publish and result types.</param>
        public MigrationPipelineContentType(Type contentType)
            : this(contentType, contentType, contentType)
        { }

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineContentType"/> instance.
        /// </summary>
        /// <param name="contentType">The shared content type also used for the result type.</param>
        /// <param name="publishType">The publish type.</param>
        public MigrationPipelineContentType(Type contentType, Type publishType)
            : this(contentType, publishType, contentType)
        { }

        /// <summary>
        /// Gets the <see cref="PublishType"/> value if it implements the given interface, or null if it does not.
        /// </summary>
        /// <param name="interface">The interface to search for.</param>
        public Type[]? GetPublishTypeForInterface(Type @interface)
            => HasInterface(PublishType, @interface) ? new[] { PublishType } : null;

        /// <summary>
        /// Gets the <see cref="ContentType"/> value if it implements the given interface, or null if it does not.
        /// </summary>
        /// <param name="interface">The interface to search for.</param>
        public Type[]? GetContentTypeForInterface(Type @interface)
            => HasInterface(ContentType, @interface) ? new[] { ContentType } : null;

        /// <summary>
        /// Gets the <see cref="PublishType"/> and <see cref="ContentType"/> array if it implements the given interface, or null if it does not.
        /// </summary>
        /// <param name="interface">The interface to search for.</param>
        public Type[]? GetPostPublishTypesForInterface(Type @interface)
            => HasInterface(PublishType, @interface) ? new[] { PublishType, ResultType } : null;

        private static bool HasInterface(Type t, Type @interface)
            => t.GetInterfaces().Contains(@interface);
    }

    /// <summary>
    /// Object that represents a definition of a content type
    /// that a pipeline migrates.
    /// </summary>
    /// <typeparam name="TContent">The content and result type.</typeparam>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    public record MigrationPipelineContentType<TContent, TPublish, TResult>()
        : MigrationPipelineContentType(typeof(TContent), typeof(TPublish), typeof(TResult))
    { }

    /// <summary>
    /// Object that represents a definition of a content type
    /// that a pipeline migrates.
    /// </summary>
    /// <typeparam name="TContent">The content and result type.</typeparam>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    public record MigrationPipelineContentType<TContent, TPublish>()
        : MigrationPipelineContentType(typeof(TContent), typeof(TPublish), typeof(TContent))
    { }

    /// <summary>
    /// Object that represents a definition of a content type
    /// that a pipeline migrates.
    /// </summary>
    /// <typeparam name="TContent">The content and publish type.</typeparam>
    public sealed record MigrationPipelineContentType<TContent>()
        : MigrationPipelineContentType<TContent, TContent>()
    { }
}
