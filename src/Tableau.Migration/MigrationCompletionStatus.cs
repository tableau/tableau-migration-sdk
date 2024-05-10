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

namespace Tableau.Migration
{
    /// <summary>
    /// Enumeration of the various ways a migration can reach completion.
    /// </summary>
    public enum MigrationCompletionStatus
    {
        /// <summary>
        /// The migration reached completion normally.
        /// </summary>
        Completed = 0,

        /// <summary>
        /// The migration was canceled before completion.
        /// </summary>
        Canceled,

        /// <summary>
        /// The migration had a fatal error that interrupted completion.
        /// </summary>
        FatalError
    }
}
