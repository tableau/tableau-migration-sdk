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

using System.IO;

namespace Tableau.Migration.Api.Models
{
    /// <summary>
    /// Interface for API client file publish options. 
    /// </summary>
    public interface IPublishFileOptions
    {
        /// <summary>
        /// Get the file content stream.
        /// </summary>
        Stream File { get; }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Gets the type of the file.
        /// </summary>
        string FileType { get; }
    }
}
