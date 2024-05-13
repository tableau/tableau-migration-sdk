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

using Tableau.Migration.Api.Rest.Models;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Class representing the default <see cref="ITag"/> implementation.
    /// </summary>
    public class Tag : ITag
    {
        /// <summary>
        /// Creates a new <see cref="Tag"/> object.
        /// </summary>
        /// <param name="label">The tag label.</param>
        public Tag(string label)
        {
            Label = Guard.AgainstNullOrEmpty(label, () => label);
        }

        /// <summary>
        /// Creates a new <see cref="Tag"/> object.
        /// </summary>
        /// <param name="response">The API response tag to build from.</param>
        public Tag(ITagType response)
        {
            Label = Guard.AgainstNullOrEmpty(response.Label, () => response.Label);
        }

        /// <summary>
        /// Creates a new <see cref="Tag"/> object.
        /// </summary>
        /// <param name="item">The tag to copy from.</param>
        public Tag(ITag item)
        {
            Label = Guard.AgainstNullOrEmpty(item.Label, () => item.Label);
        }

        /// <inheritdoc />
        public string Label { get; set; }
    }
}