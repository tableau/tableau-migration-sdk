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
    /// <para>
    /// Class containing prep flow file type constants.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_flow.htm#publish_flow">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    public class FlowFileTypes : StringEnum<FlowFileTypes>
    {
        /// <summary>
        /// Gets the name of the TFL prep flow file type.
        /// </summary>
        public const string Tfl = "tfl";

        /// <summary>
        /// Gets the name of the TFLX prep flow file type.
        /// </summary>
        public const string Tflx = "tflx";
    }
}
