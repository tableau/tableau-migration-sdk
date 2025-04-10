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

namespace Tableau.Migration.Api.Rest.Models.Requests.Cloud
{
    /// <summary>
    /// <para>
    /// Class representing an update workbook request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_subscriptions.htm#tableau-cloud-request3">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateSubscriptionRequest : TableauServerRequest
    {
        /// <summary>
        /// Gets or sets the subscription for the request. 
        /// </summary>
        [XmlElement("subscription")]
        public SubcriptionType? Subscription { get; set; }

        /// <summary>
        /// Gets or sets the schedule for the request. 
        /// </summary>
        [XmlElement("schedule")]
        public ScheduleType? Schedule { get; set; }

        /// <summary>
        /// Creates a new <see cref="UpdateSubscriptionRequest"/> object.
        /// </summary>
        public UpdateSubscriptionRequest() { }

        /// <summary>
        /// Creates a new <see cref="UpdateSubscriptionRequest"/> object.
        /// </summary>
        /// <param name="newSubject">The new subject, or null to not update the subject.</param>
        /// <param name="newAttachImage">The new attach image flag, or null to not update the flag.</param>
        /// <param name="newAttachPdf">The new attach PDF flag, or null to not update the flag.</param>
        /// <param name="newPageOrientation">The new page orientation, or null to not update the page orientation.</param>
        /// <param name="newPageSizeOption">The new page size option, or null to not update the page size option.</param>
        /// <param name="newSuspended">The new suspended flag, or null to not update the flag.</param>
        /// <param name="newMessage">The new message, or null to not update the message.</param>
        /// <param name="newContent">The new content reference, or null to not update the content reference.</param>
        /// <param name="newUserId">The new user ID, or null to not update the user ID.</param>
        /// <param name="newSchedule">The new schedule, or null to not update the schedule.</param>
        public UpdateSubscriptionRequest(
            string? newSubject = null,
            bool? newAttachImage = null,
            bool? newAttachPdf = null,
            string? newPageOrientation = null,
            string? newPageSizeOption = null,
            bool? newSuspended = null,
            string? newMessage = null,
            ISubscriptionContent? newContent = null,
            Guid? newUserId = null,
            ICloudSchedule? newSchedule = null)
        {
            Subscription = new(newAttachImage, newAttachPdf, newSuspended)
            {
                Subject = newSubject,
                PageOrientation = newPageOrientation,
                PageSizeOption = newPageSizeOption,
                Message = newMessage,
                Content = newContent is null ? null : new(newContent),
                User = newUserId is null ? null : new() { Id = newUserId.Value }
            };

            Schedule = newSchedule is null ? null : new(newSchedule);
        }

        /// <summary>
        /// Class representing a request subscription item.
        /// </summary>
        public sealed class SubcriptionType
        {
            /// <summary>
            /// Gets or sets the subject for the subscription.
            /// </summary>
            [XmlElement("subject")]
            public string? Subject { get; set; }

            /// <summary>
            /// Gets or sets the attach image flag for the subscription. 
            /// </summary>
            [XmlAttribute("attachImage")]
            public bool AttachImage
            {
                get => _attachImage!.Value;
                set => _attachImage = value;
            }
            private bool? _attachImage;

            /// <summary>
            /// Defines the serialization for the property <see cref="AttachImage"/>.
            /// </summary>
            [XmlIgnore]
            public bool AttachImageSpecified => _attachImage.HasValue;

            /// <summary>
            /// Gets or sets the attach pdf flag for the subscription. 
            /// </summary>
            [XmlAttribute("attachPdf")]
            public bool AttachPdf
            {
                get => _attachPdf!.Value;
                set => _attachPdf = value;
            }
            private bool? _attachPdf;

            /// <summary>
            /// Defines the serialization for the property <see cref="AttachPdf"/>.
            /// </summary>
            [XmlIgnore]
            public bool AttachPdfSpecified => _attachPdf.HasValue;

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
            /// Gets the suspended state for the subscription.
            /// </summary>
            [XmlAttribute("suspended")]
            public bool Suspended
            {
                get => _suspended!.Value;
                set => _suspended = value;
            }
            private bool? _suspended;

            /// <summary>
            /// Defines the serialization for the property <see cref="Suspended"/>.
            /// </summary>
            [XmlIgnore]
            public bool SuspendedSpecified => _suspended.HasValue;

            /// <summary>
            /// Gets or sets the message for the request.
            /// </summary>
            [XmlElement("message")]
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
            /// Creates a new <see cref="SubcriptionType"/> object.
            /// </summary>
            public SubcriptionType()
            { }

            /// <summary>
            /// Creates a new <see cref="SubcriptionType"/> object.
            /// </summary>
            /// <param name="attachImage">The new attach image flag, or null to not update the flag.</param>
            /// <param name="attachPdf">The new attach PDF flag, or null to not update the flag.</param>
            /// <param name="suspended">The new suspended flag, or null to not update the flag.</param>
            public SubcriptionType(bool? attachImage, bool? attachPdf, bool? suspended)
            {
                _attachImage = attachImage;
                _attachPdf = attachPdf;
                _suspended = suspended;
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
