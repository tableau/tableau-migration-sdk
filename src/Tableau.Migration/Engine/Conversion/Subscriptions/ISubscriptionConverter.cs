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

namespace Tableau.Migration.Engine.Conversion.Subscriptions
{
    /// <summary>
    /// Interface for converting subscriptions from one type to another.
    /// </summary>
    /// <typeparam name="TSourceSubscription">The source subscription type</typeparam>
    /// <typeparam name="TTargetSubscription">The target subscription type.</typeparam>
    public interface ISubscriptionConverter<TSourceSubscription, TTargetSubscription>
        : IContentItemConverter<TSourceSubscription, TTargetSubscription>
    { }
}
