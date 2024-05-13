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
using System.Linq;
using System.Xml.Serialization;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// Class that defines the Grantee Capability element for the Tableau REST API XML response.
    /// </summary>
    public class GranteeCapabilityType
    {
        internal GranteeCapabilityType()
        { }

        /// <summary>
        /// Constructor to build from <see cref="IGranteeCapability"/>.
        /// </summary>
        /// <param name="granteeCapability"></param>
        public GranteeCapabilityType(IGranteeCapability granteeCapability)
        {
            switch (granteeCapability.GranteeType)
            {
                case GranteeType.Group:
                    {
                        Group = new GroupType()
                        {
                            Id = granteeCapability.GranteeId
                        };
                        break;
                    }
                case GranteeType.User:
                    {
                        User = new UserType()
                        {
                            Id = granteeCapability.GranteeId
                        };
                        break;
                    }
            };

            Capabilities = granteeCapability
                .Capabilities
                .Select(c => new CapabilityType(c))
                .ToArray();
        }

        /// <summary>
        /// The group element if present.
        /// </summary>
        [XmlElement("group")]
        public GroupType? Group { get; set; }

        /// <summary>
        /// The user element if present.
        /// </summary>
        [XmlElement("user")]
        public UserType? User { get; set; }

        /// <summary>
        /// The collection of grantee capabilities.
        /// </summary>
        [XmlArray("capabilities")]
        [XmlArrayItem("capability")]
        public CapabilityType[] Capabilities { get; set; } = Array.Empty<CapabilityType>();

        /// <summary>
        /// Gets the ID of the grantee.
        /// </summary>
        [XmlIgnore]
        public Guid GranteeId
            => Group?.Id ?? User?.Id ?? throw new InvalidOperationException("Could not determine grantee ID");

        /// <summary>
        /// Gets the type of grantee.
        /// </summary>
        [XmlIgnore]
        public GranteeType GranteeType
            => Group is not null ? GranteeType.Group : User is not null ? GranteeType.User : throw new InvalidOperationException("Could not determine grantee type");

        /// <summary>
        /// Class that defines the group xml element.
        /// </summary>
        public class GroupType
        {
            /// <summary>
            /// The Group Id.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }
        }


        /// <summary>
        /// Class that defines the User xml element.
        /// </summary>
        public class UserType
        {
            /// <summary>
            /// The User Id.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }
        }
    }
}
