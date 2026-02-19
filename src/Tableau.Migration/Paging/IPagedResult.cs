//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Collections.Immutable;

namespace Tableau.Migration.Paging
{
    /// <summary>
    /// <see cref="IResult"/> interface for a page of data.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    public interface IPagedResult<TItem> : IResult<IImmutableList<TItem>>, IPageInfo
    {
        /// <summary>
        /// Casts a failure result to another type.
        /// </summary>
        /// <typeparam name="UItem">The type to cast to.</typeparam>
        /// <returns>The casted result.</returns>
        /// <exception cref="InvalidOperationException">If the result is not a failure result.</exception>
        public IPagedResult<UItem> CastPagedFailure<UItem>()
        {
            if (Success)
                throw new InvalidOperationException("Cannot case a successful result without a value.");

            return PagedResult<UItem>.Failed(Errors);
        }
    }
}
