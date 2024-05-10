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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration
{
    /// <summary>
    /// Interface for classes that delay tasks.
    /// </summary>
    public interface ITaskDelayer
    {
        /// <summary>
        /// Creates a Task that will complete after a time delay.
        /// </summary>
        /// <param name="delay">The time span to wait before completing the returned Task</param>
        /// <param name="cancel">The cancellation token that will be checked prior to completing the returned Task</param>
        /// <returns>A Task that represents the time delay</returns>
        Task DelayAsync(TimeSpan delay, CancellationToken cancel);
    }
}