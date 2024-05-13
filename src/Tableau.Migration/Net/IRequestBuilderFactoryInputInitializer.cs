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

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for an object that can initialize a <see cref="IRequestBuilderFactoryInput"/> object.
    /// </summary>
    /// <remarks>
    /// This interface is internal because it is only used to build a <see cref="IRequestBuilderFactoryInput"/> object, 
    /// which in turn is used to build an <see cref="IRequestBuilderFactory"/> object.
    /// End users are intended to inject the final <see cref="IRequestBuilderFactoryInput"/> result and not bootstrap objects.
    /// </remarks>
    public interface IRequestBuilderFactoryInputInitializer : IRequestBuilderFactoryInput
    {
        /// <summary>
        /// Initializes the <see cref="IRequestBuilderFactoryInputInitializer"/> object.
        /// </summary>
        /// <param name="serverUri">The server URI to initialize the <see cref="IRequestBuilderFactory"/> with.</param>
        void Initialize(Uri serverUri);
    }
}
