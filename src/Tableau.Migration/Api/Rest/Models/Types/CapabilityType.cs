﻿//
//  Copyright (c) 2025, Salesforce, Inc.
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

using System.Xml.Serialization;
using Tableau.Migration.Content.Permissions;

namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// Class that defines the Capability Type.
    /// </summary>
    public class CapabilityType
    {
        internal CapabilityType()
        { }

        /// <summary>
        /// Constructor to build from <see cref="ICapability"/>.
        /// </summary>
        /// <param name="capability"></param>
        public CapabilityType(ICapability capability)
        {
            Name = capability.Name;
            Mode = capability.Mode;
        }
        /// <inheritdoc/>
        [XmlAttribute("name")]
        public string? Name { get; set; }

        /// <inheritdoc/>
        [XmlAttribute("mode")]
        public string? Mode { get; set; }
    }
}
