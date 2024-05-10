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

namespace Tableau.Migration.Content
{
    /// <summary>
    /// This is the base class for all content types (for example: data source, workbooks, projects, sites, etc).
    /// </summary>
    public abstract class ContentBase : IContentReference
    {
        /// <summary>
        /// Gets a <see cref="StringComparer"/> suitable for comparing content URLs for workbooks, views, and data sources.
        /// </summary>
        public static readonly StringComparer ContentUrlComparer = StringComparer.Ordinal;

        /// <summary>
        /// Gets a <see cref="StringComparer"/> suitable for comparing names.
        /// </summary>
        public static readonly StringComparer NameComparer = StringComparer.OrdinalIgnoreCase;

        /// <summary>
        /// Gets a <see cref="StringComparison"/> suitable for comparing names.
        /// </summary>
        public static readonly StringComparison NameComparison = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Creates a new <see cref="ContentBase"/> instance.
        /// </summary>
        public ContentBase()
        { }

        /// <summary>
        /// Creates a new <see cref="ContentBase"/> instance.
        /// </summary>
        /// <param name="reference">The content reference.</param>
        public ContentBase(IContentReference reference)
        {
            Id = reference.Id;
            ContentUrl = reference.ContentUrl;
            Location = reference.Location;
            Name = reference.Name;
        }

        #region - IContentReference Implementation -

        /// <inheritdoc />
        public virtual Guid Id { get; protected set; }

        /// <inheritdoc />
        public virtual string ContentUrl { get; protected set; } = string.Empty;

        /// <inheritdoc />
        public virtual ContentLocation Location { get; protected set; }

        /// <inheritdoc/>
        public virtual string Name { get; protected set; } = string.Empty;

        /// <inheritdoc/>
        public bool Equals(IContentReference? other)
        {
            if (other == null && GetType() != other!.GetType())
                return false;

            return Id.Equals(other.Id) && (ContentUrlComparer.Compare(ContentUrl, other.ContentUrl) == 0) && Location.Equals(other.Location) && Name.Equals(other.Name);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            ContentBase? contentBaseObj = obj as ContentBase;
            if (contentBaseObj == null)
                return false;
            else
                return Equals(contentBaseObj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ContentUrl, Location, Name);
        }


        /// <inheritdoc/>
        public static bool operator ==(ContentBase? a, ContentBase? b)
        {
            if (a is null && b is null) return true;
            if (a is not null && b is null) return false;
            if (a is null & b is not null) return false;

            return a!.Equals(b);
        }

        /// <inheritdoc/>
        public static bool operator !=(ContentBase? a, ContentBase? b)
        {
            if (a is null && b is null) return false;
            if (a is not null && b is null) return true;
            if (a is null & b is not null) return true;

            return !(a!.Equals(b));
        }

        #endregion
    }
}
