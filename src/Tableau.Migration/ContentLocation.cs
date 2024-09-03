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
using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Content;

namespace Tableau.Migration
{
    /// <summary>
    /// Structure representing a logical location of a content item on a Tableau site.
    /// For example, for workbooks this represents the project path and the workbook name.
    /// </summary>
    /// <param name="PathSegments">Gets the individual segments of the location path.</param>
    /// <param name="PathSeparator">Gets the separator to use between segments in the location path.</param>
    public readonly record struct ContentLocation(ImmutableArray<string> PathSegments, string PathSeparator = Constants.PathSeparator)
        : IEquatable<ContentLocation>, IComparable<ContentLocation>
    {
        /// <summary>
        /// Gets the full path of the location.
        /// </summary>
        public readonly string Path { get; } = string.Join(PathSeparator, PathSegments);

        /// <summary>
        /// Gets the non-pathed name of the location.
        /// </summary>
        public readonly string Name { get; } = PathSegments.LastOrDefault() ?? string.Empty;

        /// <summary>
        /// Gets whether this location reprents an empty path.
        /// </summary>
        public readonly bool IsEmpty { get; } = PathSegments.IsEmpty;

        #region - Ctor -

        /// <summary>
        /// Creates a new <see cref="ContentLocation"/> value.
        /// </summary>
        /// <param name="segments">The location path segments.</param>
        public ContentLocation(params string[] segments)
            : this((IEnumerable<string>)segments)
        { }

        /// <summary>
        /// Creates a new <see cref="ContentLocation"/> value.
        /// </summary>
        /// <param name="parent">The parent location to use as a base path.</param>
        /// <param name="name">The item name to use as the last path segment.</param>
        public ContentLocation(ContentLocation parent, string name)
            : this(parent.PathSegments.Append(name).ToImmutableArray(), parent.PathSeparator)
        { }

        /// <summary>
        /// Creates a new <see cref="ContentLocation"/> value.
        /// </summary>
        /// <param name="segments">The location path segments.</param>
        public ContentLocation(IEnumerable<string> segments)
            : this(segments.ToImmutableArray())
        { }

        /// <summary>
        /// Creates a new <see cref="ContentLocation"/> value.
        /// </summary>
        /// <param name="pathSeparator">The separator between path segments to use.</param>
        /// <param name="segments">The location path segments.</param>
        public ContentLocation(string pathSeparator, IEnumerable<string> segments)
            : this(segments.ToImmutableArray(), pathSeparator)
        { }

        #endregion

        #region - Object Overrides -

        /// <summary>
        /// Indicates whether this value and a specified value are equal.
        /// </summary>
        /// <param name="other">The value to compare the current value.</param>
        /// <returns>true if <paramref name="other"/> and this value represents the same value; otherwise, false.</returns>
        public readonly bool Equals(ContentLocation other)
            => string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Returns the hash code for this value.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode()
            => string.GetHashCode(Path, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Returns the string representation of the value.
        /// </summary>
        /// <returns>The string representation.</returns>
        public readonly override string ToString() => Path;

        #endregion

        #region - Static Factory Methods -

        /// <summary>
        /// Creates a new <see cref="ContentLocation"/> value with the standard user/group name separator.
        /// </summary>
        /// <param name="domain">The user/group domain.</param>
        /// <param name="username">The user/group name.</param>
        /// <returns>The newly created <see cref="ContentLocation"/>.</returns>
        public static ContentLocation ForUsername(string domain, string username)
            => new([domain, username], Constants.DomainNameSeparator);

        /// <summary>
        /// Creates a new <see cref="ContentLocation"/> value from a string.
        /// </summary>
        /// <param name="contentLocationPath">The full path of the location.</param>
        /// <param name="pathSeparator">The separator to use between segments in the location path.</param>
        /// <returns>The newly created <see cref="ContentLocation"/>.</returns>
        public static ContentLocation FromPath(
            string contentLocationPath, 
            string pathSeparator = Constants.PathSeparator)
            => new(
                contentLocationPath
                    .Split(
                        pathSeparator, 
                        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .ToImmutableArray(),
                pathSeparator);

        /// <summary>
        /// Creates a new <see cref="ContentLocation"/> with the appropriate path separator for the content type.
        /// </summary>
        /// <typeparam name="TContent">The content type to create the location for.</typeparam>
        /// <param name="pathSegments">The location path segments.</param>
        /// <returns></returns>
        public static ContentLocation ForContentType<TContent>(params string[] pathSegments)
            => ForContentType(typeof(TContent), (IEnumerable<string>)pathSegments);

        /// <summary>
        /// Creates a new <see cref="ContentLocation"/> with the appropriate path separator for the content type.
        /// </summary>
        /// <typeparam name="TContent">The content type to create the location for.</typeparam>
        /// <param name="pathSegments">The location path segments.</param>
        /// <returns></returns>
        public static ContentLocation ForContentType<TContent>(IEnumerable<string> pathSegments)
            => ForContentType(typeof(TContent), pathSegments);

        /// <summary>
        /// Creates a new <see cref="ContentLocation"/> with the appropriate path separator for the content type.
        /// </summary>
        /// <param name="contentType">The content type to create the location for.</param>
        /// <param name="pathSegments">The location path segments.</param>
        /// <returns></returns>
        public static ContentLocation ForContentType(Type contentType, params string[] pathSegments)
            => ForContentType(contentType, (IEnumerable<string>)pathSegments);

        /// <summary>
        /// Creates a new <see cref="ContentLocation"/> with the appropriate path separator for the content type.
        /// </summary>
        /// <param name="contentType">The content type to create the location for.</param>
        /// <param name="pathSegments">The location path segments.</param>
        /// <returns></returns>
        public static ContentLocation ForContentType(Type contentType, IEnumerable<string> pathSegments)
        {
            string pathSeparator;
            switch(contentType)
            {
                case Type t when t == typeof(IUser) || t == typeof(IGroup):
                    pathSeparator = Constants.DomainNameSeparator;
                    break;
                default:
                    pathSeparator = Constants.PathSeparator;
                    break;
            }

            return new(pathSeparator, pathSegments);
        }

        #endregion

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The
        /// return value has these meanings:
        /// Value – Meaning
        /// Less than zero – This instance precedes other in the sort order.
        /// Zero – This instance occurs in the same position in the sort order as other.
        /// Greater than zero – This instance follows other in the sort order.
        /// </returns>
        public readonly int CompareTo(ContentLocation other)
            => string.Compare(Path, other.Path, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Creates a new <see cref="ContentLocation"/> with a new path segment appended.
        /// </summary>
        /// <param name="name">The name to append to the path.</param>
        /// <returns>The new <see cref="ContentLocation"/> with the appended path.</returns>
        public ContentLocation Append(string name)
            => new(this, name);

        /// <summary>
        /// Creates a new <see cref="ContentLocation"/> with the last path segment replaced.
        /// </summary>
        /// <param name="newName">The new name to replace the last path segment with.</param>
        /// <returns>The renamed <see cref="ContentLocation"/>.</returns>
        public ContentLocation Rename(string newName)
            => new(Parent(), newName);

        /// <summary>
        /// Creates a new <see cref="ContentLocation"/> with the last path segment removed.
        /// </summary>
        /// <returns>The new <see cref="ContentLocation"/> with the parent path.</returns>
        public ContentLocation Parent()
        {
            ImmutableArray<string> parentPath;
            if (PathSegments.Length < 2)
            {
                parentPath = ImmutableArray<string>.Empty;
            }
            else
            {
                parentPath = PathSegments.Take(PathSegments.Length - 1).ToImmutableArray();
            }

            return new(parentPath, PathSeparator);
        }
    }
}
