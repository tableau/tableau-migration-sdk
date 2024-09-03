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
using System.Linq;
using Tableau.Migration.JsonConverters.Exceptions;

namespace Tableau.Migration.JsonConverters.SerializableObjects
{
    /// <summary>
    /// Represents a JSON serializable content location, providing details about the location of content within a migration context.
    /// </summary>
    public class SerializableContentLocation
    {
        /// <summary>
        /// Gets or sets the path segments that make up the content location.
        /// </summary>
        public string[]? PathSegments { get; set; }

        /// <summary>
        /// Gets or sets the path separator used between path segments.
        /// </summary>
        public string? PathSeparator { get; set; }

        /// <summary>
        /// Gets or sets the full path constructed from the path segments and separator.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Gets or sets the name of the content at this location.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content location is empty.
        /// </summary>
        public bool IsEmpty { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableContentLocation"/> class.
        /// </summary>
        public SerializableContentLocation() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableContentLocation"/> class with details from an existing <see cref="ContentLocation"/>.
        /// </summary>
        /// <param name="location">The content location to serialize.</param>
        internal SerializableContentLocation(ContentLocation location)
        {
            PathSegments = location.PathSegments == null ? [] : [.. location.PathSegments];
            PathSeparator = location.PathSeparator;
            Path = location.Path;
            Name = location.Name;
            IsEmpty = location.IsEmpty;
        }

        /// <summary>
        /// Throw exception if any values are still null
        /// </summary>
        public void VerifyDeseralization()
        {
            Guard.AgainstNull(PathSegments, nameof(PathSegments));
            Guard.AgainstNull(PathSeparator, nameof(PathSeparator));
            Guard.AgainstNull(Path, nameof(Path));
            Guard.AgainstNull(Name, nameof(Name));

            var expectedName = PathSegments.LastOrDefault() ?? string.Empty;
            if (!string.Equals(Name, expectedName, StringComparison.Ordinal))
                throw new MismatchException($"{nameof(Name)} should be {expectedName} but is {Name}.");

            var expectedPath = string.Join(PathSeparator, PathSegments);
            if (!string.Equals(Path, expectedPath, StringComparison.Ordinal))
                throw new MismatchException($"{nameof(Path)} should be {expectedPath} but is {Path}.");

            var expectedIsEmpty = (PathSegments.Length == 0);
            if (IsEmpty != expectedIsEmpty)
                throw new MismatchException($"{nameof(IsEmpty)} should be {expectedIsEmpty} but is {IsEmpty}.");
        }

        /// <summary>
        /// Returns the current object as a <see cref="ContentLocation"/>
        /// </summary>
        /// <returns></returns>
        public ContentLocation AsContentLocation()
        {
            VerifyDeseralization();
            var ret = new ContentLocation(PathSeparator!, PathSegments!);
            return ret;
        }
    }
}
