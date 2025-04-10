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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Conversion
{
    /// <summary>
    /// Converter that returns the item with no conversion, for when the prepartion and publish type are the same.
    /// </summary>
    /// <typeparam name="TPrepare"><inheritdoc /></typeparam>
    /// <typeparam name="TPublish"><inheritdoc /></typeparam>
    public class DirectContentItemConverter<TPrepare, TPublish> : IContentItemConverter<TPrepare, TPublish>
        where TPrepare : class
        where TPublish : class
    {
        /// <inheritdoc />
        public Task<TPublish> ConvertAsync(TPrepare sourceItem, CancellationToken cancel)
        {
            var publish = sourceItem as TPublish;
            if(publish is null)
            {
                throw new InvalidCastException($"Content item of preparation type {typeof(TPrepare)} cannot be converted to publish type {typeof(TPublish)}. Register a converter override in the pipeline.");
            }

            return Task.FromResult(publish);
        }
    }
}
