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

namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// Enum of extract refresh content types.
    /// </summary>
    public class ExtractRefreshType : StringEnum<ExtractRefreshType>
    {
        /// <summary>
        /// Full refresh extract refresh type.
        /// </summary>
        public const string FullRefresh = "FullRefresh";

        /// <summary>
        /// Incremental refresh extract refresh type for Tableau Server.
        /// </summary>
        public const string ServerIncrementalRefresh = "IncrementalRefresh";

        /// <summary>
        /// Incremental refresh extract refresh type for Tableau Cloud.
        /// </summary>
        public const string CloudIncrementalRefresh = "IncrementalExtract";
    }
}
