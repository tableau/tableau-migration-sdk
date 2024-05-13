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

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Interface for an object that can resolve file store paths from content items.
    /// </summary>
    public interface IContentFilePathResolver
    {
        /// <summary>
        /// Resolves a relative file store path for the content item.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="contentItem">the content item to resolve the path for.</param>
        /// <param name="originalFileName">The original file name.</param>
        /// <returns>The resolved relative file store path.</returns>
        string ResolveRelativePath<TContent>(TContent contentItem, string originalFileName);
    }
}
