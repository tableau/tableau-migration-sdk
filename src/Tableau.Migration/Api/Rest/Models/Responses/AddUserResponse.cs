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
    /// Class representing a user creation response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class AddUserResponse : TableauServerResponse<AddUserResponse.UserType>
    {
        /// <summary>
        /// The User object.
        /// </summary>
        [XmlElement("user")]
        public override UserType? Item { get; set; }

        /// <summary>
        /// Type for the User object.
        /// </summary>
        public class UserType : IRestIdentifiable
        {
            /// <summary>
            /// The uniquer identifier for the user.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// The Username.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// The site role for the user.
            /// </summary>
            [XmlAttribute("siteRole")]
            public string? SiteRole { get; set; }

            /// <summary>
            /// The site role for the user.
            /// </summary>
            [XmlAttribute("authSetting")]
            public string? AuthSetting { get; set; }
        }
    }
}
