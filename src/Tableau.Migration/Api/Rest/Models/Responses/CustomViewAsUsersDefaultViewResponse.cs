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
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a Set Custom View as Default for Users  response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CustomViewAsUsersDefaultViewResponse : TableauServerListResponse<CustomViewAsUsersDefaultViewResponse.CustomViewAsUserDefaultViewType>
    {
        /// <summary>
        /// Gets or sets the groups for the response.
        /// </summary>
        [XmlArray("customViewAsUserDefaultResults")]
        [XmlArrayItem("customViewAsUserDefaultViewResult")]
        public override CustomViewAsUserDefaultViewType[] Items { get; set; } = Array.Empty<CustomViewAsUserDefaultViewType>();

        /// <summary>
        /// Class representing a site response.
        /// </summary>
        public class CustomViewAsUserDefaultViewType
        {
            /// <summary>
            /// Gets or sets the ID for the response.
            /// </summary>
            [XmlAttribute("success")]
            public bool Success { get; set; }
            
            /// <summary>
            /// Gets or sets the error for the response.
            /// </summary>
            [XmlElement("error")]
            public Error? Error { get; set; }

            /// <summary>
            /// Gets or sets the user for the response.
            /// </summary>
            [XmlElement("user")]
            public UserType? User { get; set; }
            
            /// <summary>
            /// Class representing a REST API user response.
            /// </summary>
            public class UserType
            {
                /// <summary>
                /// Gets or sets the ID for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }
        }
    }
}
