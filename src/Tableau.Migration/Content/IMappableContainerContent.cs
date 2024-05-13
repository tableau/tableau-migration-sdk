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

namespace Tableau.Migration.Content
{
    /// <summary>
    /// Interface for a mappable content item that belongs to a project/container.
    /// </summary>
    public interface IMappableContainerContent
    {
        /// <summary>
        /// Gets the current project/container the content item belongs to.
        /// Null if the content item is a top-level content item (e.g. top-level projects).
        /// </summary>
        IContentReference? Container { get; }

        /// <summary>
        /// Sets the content location, performing any renames as required.
        /// </summary>
        /// <param name="container">The new project/container to use.</param>
        /// <param name="newLocation">The new location to use.</param>
        void SetLocation(IContentReference? container, ContentLocation newLocation);
    }
}
