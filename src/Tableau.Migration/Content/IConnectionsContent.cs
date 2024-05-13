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

using System.Collections.Immutable;

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for content that has connection metadata.
    /// </summary>
    public interface IConnectionsContent
    {
        /// <summary>
        /// Gets the connection metadata.
        /// Connection metadata is read only because connection metadata should
        /// not be transformed directly. Instead, connections should be modified by either: 
        /// 1) manipulating XML before publishing, or 
        /// 2) updating connection metadata in a post-publish hook.
        /// </summary>
        IImmutableList<IConnection> Connections { get; }
    }
}
