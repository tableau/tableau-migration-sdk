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
using System.Collections.Generic;

namespace Tableau.Migration.Engine.Preparation
{
    internal record ContentItemPreparationResult<TPublish> : Result, IContentItemPreparationResult<TPublish>
        where TPublish : class
    {
        /// <summary>
        /// The <see cref="ContentItemPreparationResult{TPublish}"/> for items that aborted preparation
        /// due to post-pull filtering.
        /// </summary>
        public static readonly ContentItemPreparationResult<TPublish> Skipped = new(success: true, isSkipped: true, publishItem: null);

        /// <inheritdoc />
        public bool IsSkipped { get; }

        /// <inheritdoc />
        public TPublish? PublishItem { get; }

        protected ContentItemPreparationResult(bool success, bool isSkipped,
            TPublish? publishItem, params IEnumerable<Exception> errors)
            : base(success, errors)
        {
            IsSkipped = isSkipped;
            PublishItem = publishItem;
        }

        /// <summary>
        /// Creates a new <see cref="ContentItemPreparationResult{TPublish}"/> instance for successful operations.
        /// </summary>
        /// <param name="publishItem">The prepared item to publish.</param>
        /// <returns>A new <see cref="ContentItemPreparationResult{TPublish}"/> instance.</returns>
        public static ContentItemPreparationResult<TPublish> Succeeded(TPublish publishItem)
            => new(success: true, isSkipped: false, publishItem);

        /// <summary>
        /// Creates a new <see cref="ContentItemPreparationResult{TPublish}"/> instance for failed operations.
        /// </summary>
        /// <param name="errors">The errors encountered during the operation.</param>
        /// <returns>A new <see cref="ContentItemPreparationResult{TPublish}"/> instance.</returns>
        new public static ContentItemPreparationResult<TPublish> Failed(IEnumerable<Exception> errors)
            => new(success: false, isSkipped: false, publishItem: null, errors);
    }
}
