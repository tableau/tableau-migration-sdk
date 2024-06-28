﻿//
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

using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Transformers
{
    /// <summary>
    /// Interface for an object that can run transformations.
    /// </summary>
    public interface IContentTransformerRunner
    {
        /// <summary>
        /// Executes all transformations for the content type in order.
        /// </summary>
        /// <typeparam name="TPublish">The publishable content type.</typeparam>
        /// <param name="itemToTransform">The items to transform.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The resulting transformed item.</returns>
        Task<TPublish> ExecuteAsync<TPublish>(TPublish itemToTransform, CancellationToken cancel);
    }
}
