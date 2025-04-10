//
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

namespace Tableau.Migration.Api.Rest.Models.Responses.Server
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Class representing a Get Server Subscriptions response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_subscriptions.htm#tableau-server-request2">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class GetSubscriptionsResponse : PagedTableauServerResponse<GetSubscriptionsResponse.SubscriptionType>
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public GetSubscriptionsResponse()
        { }

        /// <summary>
        /// Gets or sets the subscriptions for the response.
        /// </summary>
        [XmlArray("subscriptions")]
        [XmlArrayItem("subscription")]
        public override SubscriptionType[] Items { get; set; } = [];

        /// <summary>
        /// Class representing a subscription on the response.
        /// </summary>
        public class SubscriptionType : ISubscriptionType
        {
            /// <summary>
            /// The default parameterless constructor.
            /// </summary>
            public SubscriptionType()
            { }

            /// <inheritdoc />
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <inheritdoc />
            [XmlAttribute("subject")]
            public string? Subject { get; set; }

            /// <inheritdoc />
            [XmlAttribute("attachImage")]
            public bool AttachImage { get; set; }

            /// <inheritdoc />
            [XmlAttribute("attachPdf")]
            public bool AttachPdf { get; set; }

            /// <inheritdoc />
            [XmlAttribute("pageOrientation")]
            public string? PageOrientation { get; set; }

            /// <inheritdoc />
            [XmlAttribute("pageSizeOption")]
            public string? PageSizeOption { get; set; }

            /// <inheritdoc />
            [XmlAttribute("suspended")]
            public bool Suspended { get; set; }

            /// <inheritdoc />
            [XmlAttribute("message")]
            public string? Message { get; set; }

            /// <summary>
            /// Gets or sets the content for the response. 
            /// </summary>
            [XmlElement("content")]
            public ContentType? Content { get; set; }

            ISubscriptionContentType? ISubscriptionType.Content => Content;

            /// <summary>
            /// Gets or sets the schedule for the response. 
            /// </summary>
            [XmlElement("schedule")]
            public ScheduleType? Schedule { get; set; }

            /// <summary>
            /// Gets or sets the user for the response. 
            /// </summary>
            [XmlElement("user")]
            public UserType? User { get; set; }

            IRestIdentifiable? ISubscriptionType.User => User;

            /// <summary>
            /// Class representing a content type on the response.
            /// </summary>
            public class ContentType : ISubscriptionContentType
            {
                /// <inheritdoc/>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <inheritdoc/>
                [XmlAttribute("type")]
                public string? Type { get; set; }

                /// <inheritdoc/>
                [XmlAttribute("sendIfViewEmpty")]
                public bool SendIfViewEmpty { get; set; }
            }

            /// <summary>
            /// Class representing a schedule on the response.
            /// </summary>
            public class ScheduleType
            {
                /// <summary>
                /// The default parameterless constructor.
                /// </summary>
                public ScheduleType()
                { }

                /// <summary>
                /// Gets or sets the ID for the response. 
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// Gets or sets the name for the response. 
                /// </summary>
                [XmlAttribute("name")]
                public string? Name { get; set; }
            }

            /// <summary>
            /// Class representing a subscription user on the response.
            /// </summary>
            public class UserType : IRestIdentifiable
            {
                /// <summary>
                /// The default parameterless constructor.
                /// </summary>
                public UserType()
                { }

                /// <summary>
                /// Gets or sets the ID for the response. 
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// Gets or sets the name for the response. 
                /// </summary>
                [XmlAttribute("name")]
                public string? Name { get; set; }
            }
        }
    }
}
