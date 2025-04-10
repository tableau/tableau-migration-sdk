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
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;

using CloudModels = Tableau.Migration.Api.Models.Cloud;

namespace Tableau.Migration.Api.Rest.Models.Requests.Cloud
{
    /// <summary>   
    /// <para>
    /// Class representing a create extract refresh task request.
    /// </para>
    /// <para>
    /// See https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_subscriptions.htm#tableau-cloud-request for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CreateSubscriptionRequest : TableauServerRequest
    {
        /// <summary>
        /// Gets or sets the subscription for the request. 
        /// </summary>
        [XmlElement("subscription")]
        public SubscriptionType? Subscription { get; set; }

        /// <summary>
        /// Gets or sets the schedule for the request. 
        /// </summary>
        [XmlElement("schedule")]
        public ScheduleType? Schedule { get; set; }

        /// <summary>
        /// Creates a new <see cref="CreateExtractRefreshTaskRequest"/> object.
        /// </summary>
        public CreateSubscriptionRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="CreateSubscriptionRequest"/> object.
        /// </summary>
        /// <param name="options">The subscription creation options.</param>
        public CreateSubscriptionRequest(CloudModels.ICreateSubscriptionOptions options)
        {
            Subscription = new(options);
            Schedule = new(options.Schedule);
        }

        /// <summary>
        /// Class representing a request subscription item.
        /// </summary>
        public sealed class SubscriptionType
        {
            /// <summary>
            /// Gets or sets the subject for the subscription.
            /// </summary>
            [XmlAttribute("subject")]
            public string? Subject { get; set; }

            /// <summary>
            /// Gets or sets the attach image flag for the subscription. 
            /// </summary>
            [XmlAttribute("attachImage")]
            public bool AttachImage { get; set; }

            /// <summary>
            /// Gets or sets the attach pdf flag for the subscription. 
            /// </summary>
            [XmlAttribute("attachPdf")]
            public bool AttachPdf { get; set; }

            /// <summary>
            /// Gets or sets the page orientation of the subscription.
            /// </summary>
            [XmlAttribute("pageOrientation")]
            public string? PageOrientation { get; set; }

            /// <summary>
            /// Gets or sets the page page size option of the subscription.
            /// </summary>
            [XmlAttribute("pageSizeOption")]
            public string? PageSizeOption { get; set; }

            /// <summary>
            /// Gets or sets the message for the request.
            /// </summary>
            [XmlAttribute("message")]
            public string? Message { get; set; }

            /// <summary>
            /// Gets or sets the content for the subscription. 
            /// </summary>
            [XmlElement("content")]
            public ContentType? Content { get; set; }

            /// <summary>
            /// Gets or sets the user for the subscription. 
            /// </summary>
            [XmlElement("user")]
            public UserType? User { get; set; }

            /// <summary>
            /// Creates a new <see cref="SubscriptionType"/> object.
            /// </summary>
            public SubscriptionType()
            { }

            /// <summary>
            /// Creates a new <see cref="SubscriptionType"/> object.
            /// </summary>
            /// <param name="options">The subscription creation options.</param>
            public SubscriptionType(CloudModels.ICreateSubscriptionOptions options)
            {
                Subject = options.Subject;
                AttachImage = options.AttachImage;
                AttachPdf = options.AttachPdf;
                PageOrientation = options.PageOrientation;
                PageSizeOption = options.PageSizeOption;
                Message = options.Message;

                Content = new(options.Content);
                User = new UserType { Id = options.UserId };
            }

            /// <summary>
            /// Class representing a content type on the request.
            /// </summary>
            public class ContentType : ISubscriptionContentType
            {
                /// <summary>
                /// Creates a new <see cref="ContentType"/> object.
                /// </summary>
                public ContentType()
                { }

                /// <summary>
                /// Creates a new <see cref="ContentType"/> object.
                /// </summary>
                /// <param name="content">A subscription content reference.</param>
                public ContentType(ISubscriptionContent content)
                {
                    Id = content.Id;
                    Type = content.Type;
                    SendIfViewEmpty = content.SendIfViewEmpty;
                }

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
            /// Class representing a subscription user on the request.
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
        /// Class representing a request schedule item.
        /// </summary>
        public class ScheduleType
        {
            /// <summary>
            /// Gets or sets the frequency for the request.
            /// </summary>
            [XmlAttribute("frequency")]
            public string? Frequency { get; set; }

            /// <summary>
            /// Gets or sets the frequency details for the request.
            /// </summary>
            [XmlElement("frequencyDetails")]
            public FrequencyDetailsType? FrequencyDetails { get; set; }

            /// <summary>
            /// Creates a new <see cref="ScheduleType"/> instance.
            /// </summary>
            public ScheduleType()
            { }

            /// <summary>
            /// Creates a new <see cref="ScheduleType"/> instance.
            /// </summary>
            /// <param name="schedule">The schedule to copy from.</param>
            public ScheduleType(ICloudSchedule schedule)
            {
                Frequency = schedule.Frequency;
                FrequencyDetails = new(schedule.FrequencyDetails);
            }

            /// <summary>
            /// Class representing a request frequency details item.
            /// </summary>
            public class FrequencyDetailsType : IScheduleFrequencyDetailsType
            {
                /// <summary>
                /// Gets or sets the start time for the request.
                /// </summary>
                [XmlAttribute("start")]
                public string? Start { get; set; }

                /// <summary>
                /// Gets or sets the end time for the request.
                /// </summary>
                [XmlAttribute("end")]
                public string? End { get; set; }

                /// <summary>
                /// Gets or sets the intervals for the request.
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
                public FrequencyDetailsType(IFrequencyDetails frequencyDetails)
                {
                    Start = frequencyDetails.StartAt?.ToString(Constants.FrequencyTimeFormat);
                    End = frequencyDetails.EndAt?.ToString(Constants.FrequencyTimeFormat);
                    Intervals = frequencyDetails.Intervals.Select(i => new IntervalType(i)).ToArray();
                }

                /// <summary>
                /// Class representing a request interval item.
                /// </summary>
                public class IntervalType : IScheduleIntervalType
                {
                    /// <summary>
                    /// Gets or sets the hours for the request.
                    /// </summary>
                    [XmlAttribute("hours")]
                    public string? Hours { get; set; }

                    /// <summary>
                    /// Gets or sets the minutes for the request.
                    /// </summary>
                    [XmlAttribute("minutes")]
                    public string? Minutes { get; set; }

                    /// <summary>
                    /// Gets or sets the weekday for the request.
                    /// </summary>
                    [XmlAttribute("weekDay")]
                    public string? WeekDay { get; set; }

                    /// <summary>
                    /// Gets or sets the month/day for the request.
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
                    public IntervalType(IInterval interval)
                    {
                        Hours = interval.Hours?.ToString();
                        Minutes = interval.Minutes?.ToString();
                        WeekDay = interval.WeekDay;
                        MonthDay = interval.MonthDay;
                    }
                }
            }
        }
    }
}
