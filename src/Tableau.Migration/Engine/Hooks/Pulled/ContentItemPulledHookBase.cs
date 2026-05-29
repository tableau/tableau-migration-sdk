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

using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Pulled
{
    /// <summary>
    /// Abstract base class for <see cref="IContentItemPulledHook{TPrepare}"/> hook implementations.
    /// </summary>
    /// <typeparam name="TPrepare">The pulled content type.</typeparam>
    public abstract class ContentItemPulledHookBase<TPrepare> : IContentItemPulledHook<TPrepare>
        where TPrepare : IContentReference
    {
        /// <inheritdoc />
        public abstract Task<ContentItemPulledContext<TPrepare>?> ExecuteAsync(ContentItemPulledContext<TPrepare> ctx, CancellationToken cancel);
    }
}
