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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tableau.Migration.Content.Files
{
    /// <summary>
    /// Interface that represents a read/write stream to a Tableau XML file.
    /// </summary>
    public interface ITableauFileXmlStream : IAsyncDisposable
    {
        /// <summary>
        /// Gets a read/write stream to the XML content.
        /// </summary>
        Stream XmlContent { get; }

        /// <summary>
        /// Gets the currently loaded XML of the file,
        /// parsing the file if necessary.
        /// Changes to the XML will be automatically saved before publishing.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The XML document.</returns>
        Task<XDocument> GetXmlAsync(CancellationToken cancel);
    }
}
