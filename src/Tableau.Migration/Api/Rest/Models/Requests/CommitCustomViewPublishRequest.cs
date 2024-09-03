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
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// Class representing an commit Custom View request.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CommitCustomViewPublishRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public CommitCustomViewPublishRequest()
        { }

        /// <summary>
        /// The constructor to build from <see cref="IPublishCustomViewOptions"/>.
        /// </summary>
        /// <param name="options">The Custom View publish options.</param>
        public CommitCustomViewPublishRequest(IPublishCustomViewOptions options)
        {
            CustomView = new CustomViewType(options);
        }

        /// <summary>
        /// The Custom View.
        /// </summary>
        [XmlElement("customView")]
        public CustomViewType? CustomView { get; set; }

        /// <summary>
        /// The Custom View type in the API request body.
        /// </summary> 
        public class CustomViewType
        {
            /// <summary>
            /// The default parameterless constructor.
            /// </summary>
            public CustomViewType()
            { }

            /// <summary>
            /// The constructor to build from <see cref="IPublishCustomViewOptions"/>
            /// </summary>
            public CustomViewType(IPublishCustomViewOptions options)
            {
                Name = options.Name;
                Shared = options.Shared;
                Workbook = new WorkbookType(options.WorkbookId);
                Owner = new OwnerType(options.OwnerId);
            }

            /// <summary>
            /// The Custom View name.
            /// </summary>
            [XmlAttribute(AttributeName = "name")]
            public string? Name { get; set; }

            /// <summary>
            /// Flag to indicate if the Custom View is shared.
            /// </summary>
            [XmlAttribute(AttributeName = "shared")]
            public bool Shared { get; set; }

            /// <summary>
            /// The workbook to which this Custom View is linked.
            /// </summary>
            [XmlElement("workbook")]
            public WorkbookType? Workbook { get; set; }
            /// <summary>
            /// The ID of the owner for this Custom View.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }


            /// <summary>
            /// The workbook type in the API request body.
            /// </summary>       
            public class WorkbookType
            {
                /// <summary>
                /// The default parameterless constructor.
                /// </summary>
                public WorkbookType()
                { }

                /// <summary>
                /// The constructor to build from the ID.
                /// </summary>
                /// <param name="id">The workbook ID.</param>
                public WorkbookType(Guid id)
                {
                    Id = id;
                }
                /// <summary>
                /// The workbook Id.
                /// </summary>
                [XmlAttribute(AttributeName = "id")]
                public Guid Id { get; set; }
            }

            /// <summary>
            /// The owner type in the API request body.
            /// </summary> 
            public class OwnerType
            {
                /// <summary>
                /// The default parameterless constructor.
                /// </summary>
                public OwnerType()
                { }

                /// <summary>
                /// The constructor to build from the ID.
                /// </summary>
                /// <param name="id">The owner ID.</param>
                public OwnerType(Guid id)
                {
                    Id = id;
                }
                /// <summary>
                /// The Owner Id.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }
        }
    }
}
