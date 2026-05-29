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
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Interface that represents a read/write stream to a Tableau JSON file.
    /// </summary>
    public interface ITableauFileJsonStream : IAsyncDisposable
    {
        /// <summary>
        /// Gets a read/write stream to the JSON content.
        /// </summary>
        Stream JsonContent { get; }

        /// <summary>
        /// Gets the currently loaded JSON of the file,
        /// parsing the file if necessary.
        /// Changes to the JSON will be automatically saved before publishing.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The mutable JSON node (root of the document).</returns>
        Task<JsonNode> GetJsonAsync(CancellationToken cancel);
    }
}

