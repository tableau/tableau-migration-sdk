//
//  Copyright (c) 2025, Salesforce, Inc.
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
using Tableau.Migration.Resources;

namespace Tableau.Migration
{
    /// <summary>
    /// The exception that is thrown when an operation is not supported for the current <see cref="TableauInstanceType"/>.
    /// </summary>
    public class TableauInstanceTypeNotSupportedException(TableauInstanceType unsupportedInstanceType, string message)
        : NotSupportedException(message),
        IEquatable<TableauInstanceTypeNotSupportedException>
    {
        /// <summary>
        /// Creates a new <see cref="TableauInstanceTypeNotSupportedException"/> object.
        /// </summary>
        /// <param name="unsupportedInstanceType">The unsupported <see cref="TableauInstanceType"/>.</param>
        /// <param name="localizer">The localizer to use for the standard exception message.</param>
        public TableauInstanceTypeNotSupportedException(TableauInstanceType unsupportedInstanceType, ISharedResourcesLocalizer localizer)
            : this(unsupportedInstanceType, localizer[SharedResourceKeys.TableauInstanceTypeNotSupportedMessage, unsupportedInstanceType.GetFriendlyName()])
        { }

        /// <summary>
        /// Gets the unsupported <see cref="TableauInstanceType"/>.
        /// </summary>
        public TableauInstanceType UnsupportedInstanceType { get; } = unsupportedInstanceType;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as TableauInstanceTypeNotSupportedException);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(GetType(), Message, UnsupportedInstanceType);

        /// <inheritdoc />
        public bool Equals(TableauInstanceTypeNotSupportedException? other)
        {
            var baseEquals = this.BaseExceptionEquals(other);
            if (baseEquals is not null)
            {
                return baseEquals.Value;
            }

            return UnsupportedInstanceType == other?.UnsupportedInstanceType;
        }
    }
}
