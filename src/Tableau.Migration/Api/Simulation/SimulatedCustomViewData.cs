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
using System.Text.Json.Serialization;

namespace Tableau.Migration.Api.Simulation
{
    /// <summary>
    /// Object that holds simulated custom view file data
    /// </summary>
    public class SimulatedCustomViewData
    {
        /// <summary>
        /// Simulated custom views data
        /// </summary>
        public List<SimulatedCustomViewType> CustomViews { get; set; } = new List<SimulatedCustomViewType>();

        /// <summary>
        /// Simulated custom view type data
        /// </summary>
        public class SimulatedCustomViewType
        {
            /// <summary>
            /// Constructor for the simulated custom view.
            /// </summary>
            /// <param name="isSourceView"></param>
            /// <param name="viewName"></param>
            /// <param name="tcv"></param>
            public SimulatedCustomViewType(bool isSourceView, string viewName, string tcv)
            {
                IsSourceView = isSourceView;
                ViewName = viewName;
                Tcv = tcv;
            }

            /// <summary>
            /// Flag that indicates if <see cref="ViewName"/> is the source view 
            /// for the Custom View encoded in <see cref="Tcv"/>.
            /// </summary>
            [JsonPropertyName("isSourceView")]
            public bool IsSourceView { get; set; }

            /// <summary>
            /// Name of the view which this custom view is based on.
            /// </summary>
            [JsonPropertyName("viewName")]
            public string ViewName { get; set; }

            /// <summary>
            /// The encoded Custom View Definition
            /// </summary>
            [JsonPropertyName("tcv")]
            public string Tcv { get; set; }
        }
    }
}
