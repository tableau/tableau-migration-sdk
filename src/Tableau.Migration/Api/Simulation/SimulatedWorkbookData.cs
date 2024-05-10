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

using System.Collections.Generic;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Object that holds simulated workbook file data
    /// </summary>
    public class SimulatedWorkbookData : SimulatedDataWithConnections
    {
        /// <summary>
        /// Simulated Views data
        /// </summary>
        public List<SimulatedViewType> Views { get; set; } = new List<SimulatedViewType>();

        /// <summary>
        /// Simulated view type data
        /// </summary>
        public class SimulatedViewType
        {
            /// <summary>
            /// Default parameterless constructor
            /// </summary>
            public SimulatedViewType()
            { }

            /// <summary>
            /// Simulated view constructor
            /// </summary>
            /// <param name="view"></param>
            /// <param name="hidden"></param>
            public SimulatedViewType(ViewResponse.ViewType view, bool hidden)
            {
                View = view;
                Hidden = hidden;
            }

            /// <summary>
            /// Simulated View
            /// </summary>
            public ViewResponse.ViewType? View { get; set; }

            /// <summary>
            /// Simulated hidden flag
            /// </summary>
            public bool Hidden { get; set; } = false;
        }
    }
}
