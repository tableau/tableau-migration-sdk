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

namespace Tableau.Migration.Content.Schedules
{
    /// <summary>
    /// Enum of extract refresh content types.
    /// </summary>
    public enum ExtractRefreshContentType
    {
        /// <summary>
        /// Unknown content type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Workbook content type.
        /// </summary>
        Workbook,

        /// <summary>
        /// Data source content type.
        /// </summary>
        DataSource
    }
}
