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
using System.Diagnostics.CodeAnalysis;

namespace Tableau.Migration.Net
{
    /// <summary>
    /// Interface for an object that contains the input given for a <see cref="IRequestBuilderFactory"/>, 
    /// used to bootstrap request building dependency injection.
    /// </summary>
    /// <remarks>
    /// In almost all cases it is preferrable to inject the <see cref="IRequestBuilderFactory"/> object, 
    /// this interface is only intended to be used to build <see cref="IRequestBuilderFactory"/> object.
    /// </remarks>
    public interface IRequestBuilderFactoryInput
    {
        /// <summary>
        /// Gets whether the input has been initialized.
        /// </summary>
        [MemberNotNullWhen(true, nameof(ServerUri))]
        bool IsInitialized { get; }

        /// <summary>
        /// Gets the server URI to initialize the <see cref="IRequestBuilderFactory"/> with.
        /// </summary>
        Uri? ServerUri { get; }
    }
}
