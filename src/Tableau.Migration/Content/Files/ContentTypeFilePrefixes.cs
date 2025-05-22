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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tableau.Migration.Content.Files
{
    internal class ContentTypeFilePrefixes
    {
        /// <summary>
        /// The directory for custom views.
        /// </summary>
        public const string CustomView = "customview";

        /// <summary>
        /// The directory for data sources.
        /// </summary>
        public const string DataSource = "data-source";

        /// <summary>
        /// The directory for flows.
        /// </summary>
        public const string Flow = "flow";

        /// <summary>
        /// The directory for workbooks.
        /// </summary>
        public const string Workbook = "workbook";
    }
}
