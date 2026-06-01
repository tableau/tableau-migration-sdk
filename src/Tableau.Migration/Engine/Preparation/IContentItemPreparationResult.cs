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

using System.Diagnostics.CodeAnalysis;

namespace Tableau.Migration.Engine.Preparation
{
    /// <summary>
    /// <see cref="IResult"/> object for preparting a content item for publishing.
    /// </summary>
    public interface IContentItemPreparationResult<TPublish> : IResult
        where TPublish : class
    {
        /// <summary>
        /// Gets whether the preparation was halted due to a post-pull filter,
        /// and publishing should be skipped.
        /// </summary>
        bool IsSkipped { get; }

        /// <summary>
        /// Gets whether an item was successfully and fully prepared for publishing.
        /// </summary>
        [MemberNotNullWhen(true, nameof(PublishItem))]
        public bool IsPrepared => Success && !IsSkipped;

        /// <summary>
        /// Gets the item to publish,
        /// will be null in case of failure or <see cref="IsSkipped"/>.
        /// </summary>
        TPublish? PublishItem { get; }
    }
}
