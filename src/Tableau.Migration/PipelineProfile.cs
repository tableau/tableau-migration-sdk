// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

namespace Tableau.Migration
{
    /// <summary>
    /// Enumeration of the various supported migration pipeline profiles.
    /// </summary>
    public enum PipelineProfile
    {
        //Custom = 0, //Uncomment when custom pipelines are supported.

        /// <summary>
        /// The pipeline to bulk migrate content from a Tableau Server site to a Tableau Cloud site.
        /// </summary>
        ServerToCloud = 1
    }
}
