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

using System;
using System.Linq;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses.Cloud
{
    /// <summary>
    /// Class representing a subscription creation response.
    /// https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_subscriptions.htm#tableau-cloud-request3
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateSubscriptionResponse : TableauServerResponse<UpdateSubscriptionResponse.SubscriptionType>
    {
        /// <summary>
        /// Gets or sets the subscription for the response.
        /// </summary>
        [XmlElement("subscription")]
        public override SubscriptionType? Item { get; set; }

        /// <summary>
        /// Gets or sets the schedule for the response. 
        /// </summary>
        [XmlElement("schedule")]
        public ScheduleType? Schedule { get; set; }

        /// <summary>
        /// Class representing a response subscription item.
        /// </summary>
        public class SubscriptionType : ISubscriptionType
        {
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
            [XmlElement("message")]
            public string? Message { get; set; }

            /// <summary>
            /// Gets or sets the content for the response. 
            /// </summary>
            [XmlElement("content")]
            public ContentType? Content { get; set; }

            ISubscriptionContentType? ISubscriptionType.Content => Content;

            /// <summary>
            /// Gets or sets the user for the response. 
            /// </summary>
            [XmlElement("user")]
            public UserType? User { get; set; }

            IRestIdentifiable? ISubscriptionType.User => User;

            /// <summary>
            /// Creates a new <see cref="SubscriptionType"/> object.
            /// </summary>
            public SubscriptionType()
            { }

            /// <summary>
            /// Creates a new <see cref="SubscriptionType"/> object.
            /// </summary>
            /// <param name="subscription">A subscription response to copy from.</param>
            public SubscriptionType(ISubscriptionType subscription)
            {
                Id = subscription.Id;
                Subject = subscription.Subject;
                AttachImage = subscription.AttachImage;
                AttachPdf = subscription.AttachPdf;
                PageOrientation = subscription.PageOrientation;
                PageSizeOption = subscription.PageSizeOption;
                Suspended = subscription.Suspended;
                Message = subscription.Message;

                Content = subscription.Content is null ? null : new(subscription.Content);
                User = subscription.User is null ? null : new UserType { Id = subscription.User.Id };
            }

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

                /// <summary>
                /// Creates a new <see cref="ContentType"/> object.
                /// </summary>
                public ContentType()
                { }

                /// <summary>
                /// Creates a new <see cref="ContentType"/> object.
                /// </summary>
                /// <param name="content">A subscription content reference.</param>
                public ContentType(ISubscriptionContentType content)
                {
                    Id = content.Id;
                    Type = content.Type;
                    SendIfViewEmpty = content.SendIfViewEmpty;
                }
            }

            /// <summary>
            /// Class representing a subscription user on the response.
            /// </summary>
            public class UserType : IRestIdentifiable
            {
                /// <summary>
                /// Gets or sets the ID for the response. 
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }
        }

        /// <summary>
        /// Class representing a response schedule item.
        /// </summary>
        public class ScheduleType : ICloudScheduleType
        {
            /// <summary>
            /// Gets or sets the frequency for the response.
            /// </summary>
            [XmlAttribute("frequency")]
            public string? Frequency { get; set; }

            /// <summary>
            /// Gets or sets the frequency details for the response.
            /// </summary>
            [XmlElement("frequencyDetails")]
            public FrequencyDetailsType? FrequencyDetails { get; set; }

            /// <summary>
            /// Gets or sets the next run at for the response.
            /// </summary>
            [XmlAttribute("nextRunAt")]
            public string? NextRunAt { get; set; }

            IScheduleFrequencyDetailsType? IScheduleType.FrequencyDetails => FrequencyDetails;

            /// <summary>
            /// Creates a new <see cref="ScheduleType"/> instance.
            /// </summary>
            public ScheduleType()
            { }

            /// <summary>
            /// Creates a new <see cref="ScheduleType"/> instance.
            /// </summary>
            /// <param name="schedule">The schedule to copy from.</param>
            public ScheduleType(ICloudScheduleType schedule)
            {
                Frequency = schedule.Frequency;
                NextRunAt = schedule.NextRunAt;
                FrequencyDetails = schedule.FrequencyDetails is null ? null : new(schedule.FrequencyDetails);
            }

            /// <summary>
            /// Class representing a response frequency details item.
            /// </summary>
            public class FrequencyDetailsType : IScheduleFrequencyDetailsType
            {
                /// <summary>
                /// Gets or sets the start time for the response.
                /// </summary>
                [XmlAttribute("start")]
                public string? Start { get; set; }

                /// <summary>
                /// Gets or sets the end time for the response.
                /// </summary>
                [XmlAttribute("end")]
                public string? End { get; set; }

                /// <summary>
                /// Gets or sets the intervals for the response.
                /// </summary>
                [XmlArray("intervals")]
                [XmlArrayItem("interval")]
                public IntervalType[] Intervals { get; set; } = Array.Empty<IntervalType>();

                /// <inheritdoc/>
                IScheduleIntervalType[] IScheduleFrequencyDetailsType.Intervals => Intervals;

                /// <summary>
                /// Creates a new <see cref="FrequencyDetailsType"/> instance.
                /// </summary>
                public FrequencyDetailsType()
                { }

                /// <summary>
                /// Creates a new <see cref="FrequencyDetailsType"/> instance.
                /// </summary>
                /// <param name="frequencyDetails">The frequency details to copy from.</param>
                public FrequencyDetailsType(IScheduleFrequencyDetailsType frequencyDetails)
                {
                    Start = frequencyDetails.Start;
                    End = frequencyDetails.End;
                    Intervals = frequencyDetails.Intervals.Select(i => new IntervalType(i)).ToArray();
                }

                /// <summary>
                /// Class representing a response interval item.
                /// </summary>
                public class IntervalType : IScheduleIntervalType
                {
                    /// <summary>
                    /// Gets or sets the hours for the response.
                    /// </summary>
                    [XmlAttribute("hours")]
                    public string? Hours { get; set; }

                    /// <summary>
                    /// Gets or sets the minutes for the response.
                    /// </summary>
                    [XmlAttribute("minutes")]
                    public string? Minutes { get; set; }

                    /// <summary>
                    /// Gets or sets the weekday for the response.
                    /// </summary>
                    [XmlAttribute("weekDay")]
                    public string? WeekDay { get; set; }

                    /// <summary>
                    /// Gets or sets the month/day for the response.
                    /// </summary>
                    [XmlAttribute("monthDay")]
                    public string? MonthDay { get; set; }

                    /// <summary>
                    /// Creates a new <see cref="IntervalType"/> instance.
                    /// </summary>
                    public IntervalType()
                    { }

                    /// <summary>
                    /// Creates a new <see cref="IntervalType"/> instance.
                    /// </summary>
                    /// <param name="interval">The interval to copy from.</param>
                    public IntervalType(IScheduleIntervalType interval)
                    {
                        Hours = interval.Hours;
                        Minutes = interval.Minutes;
                        WeekDay = interval.WeekDay;
                        MonthDay = interval.MonthDay;
                    }
                }
            }
        }
    }
}
