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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content.Schedules;
using CloudResponses = Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using ServerResponses = Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Tests
{
    public static class FixtureFactory
    {
        public static IFixture Create() => Customize(new Fixture());

        private static IFixture Customize(IFixture fixture)
        {
            fixture = fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });

            fixture.Customizations.Add(new ImmutableCollectionSpecimenBuilder());

            fixture.Register(() => fixture.Create<MockServiceProvider>().Object);

            fixture.Register(() => fixture.Create<Mock<TimeProvider>>().Object);

            fixture.Register(() => new ContentLocation(fixture.CreateMany<string>()));

            fixture.Register<string, Stream>((string data) =>
            {
                var bytes = Constants.DefaultEncoding.GetBytes(data);
                return new MemoryStream(bytes);
            });

            fixture.Register<IMemoryStreamManager>(() => MemoryStreamManager.Instance);

            #region - JobResponse -

            // These properties should return DateTime strings instead of the default Guid-like ones.
            fixture.Customize<JobResponse.JobType>(composer => composer
                .With(j => j.CreatedAt, () => fixture.Create<DateTime>().ToIso8601())
                .With(j => j.UpdatedAt, () => fixture.Create<DateTime>().ToIso8601())
                .With(j => j.CompletedAt, () => fixture.Create<DateTime?>()?.ToIso8601()));

            #endregion

            #region - ScheduleResponse -

            // These properties should return DateTime strings instead of the default Guid-like ones.
            fixture.Customize<ServerResponses.ScheduleResponse.ScheduleType>(composer => composer
                .With(s => s.CreatedAt, () => fixture.Create<DateTime?>()?.ToIso8601())
                .With(s => s.UpdatedAt, () => fixture.Create<DateTime?>()?.ToIso8601())
                .With(s => s.NextRunAt, () => fixture.Create<DateTime?>()?.ToIso8601()));

            SetupInterval<CloudResponses.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.ScheduleType.FrequencyDetailsType.IntervalType>();
            SetupInterval<CloudResponses.CreateExtractRefreshTaskResponse.ScheduleType.FrequencyDetailsType.IntervalType>();
            SetupInterval<ServerResponses.ScheduleResponse.ScheduleType.FrequencyDetailsType.IntervalType>();

            SetupFrequencyDetails<CloudResponses.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.ScheduleType.FrequencyDetailsType>();
            SetupFrequencyDetails<CloudResponses.CreateExtractRefreshTaskResponse.ScheduleType.FrequencyDetailsType>();
            SetupFrequencyDetails<ServerResponses.ScheduleResponse.ScheduleType.FrequencyDetailsType>();

            void SetupFrequencyDetails<TFrequencyDetails>()
                where TFrequencyDetails : IScheduleFrequencyDetailsType
            {
                fixture.Customize<TFrequencyDetails>(composer => composer
                    .With(f => f.Start, () => fixture.Create<TimeOnly?>()?.ToString())
                    .With(f => f.End, () => fixture.Create<TimeOnly?>()?.ToString()));
            }

            void SetupInterval<TInterval>()
                where TInterval : IScheduleIntervalType
            {
                fixture.Customize<TInterval>(composer =>
                {
                    var customized = composer
                        .Without(i => i.Minutes)
                        .Without(i => i.Hours)
                        .Without(i => i.WeekDay)
                        .Without(i => i.MonthDay);

                    const string hours = nameof(IScheduleIntervalType.Hours);
                    const string minutes = nameof(IScheduleIntervalType.Minutes);
                    const string weekDay = nameof(IScheduleIntervalType.WeekDay);
                    const string monthDay = nameof(IScheduleIntervalType.MonthDay);

                    var property = new[] { hours, minutes, weekDay, monthDay }.PickRandom();

                    return property switch
                    {
                        hours => customized.With(i => i.Hours, GetRandomValue(IntervalValues.HoursValues)),
                        minutes => customized.With(i => i.Minutes, GetRandomValue(IntervalValues.MinutesValues)),
                        weekDay => customized.With(i => i.WeekDay, GetRandomValue(IntervalValues.WeekDaysValues)),
                        monthDay => customized.With(i => i.MonthDay, GetRandomValue(IntervalValues.MonthDaysValues)),
                        _ => throw new NotSupportedException($"{nameof(property)} value {property} is not supported.")
                    };

                    static string GetRandomValue<TValue>(IEnumerable<TValue?> values)
                        => values.Select(v => v?.ToString()).ExceptNulls().PickRandom();
                });
            }

            #endregion

            #region - ImportJobResponse -

            // These properties should return DateTime strings instead of the default Guid-like ones.
            fixture.Customize<ImportJobResponse.ImportJobType>(composer => composer
                .With(j => j.CreatedAt, () => fixture.Create<DateTime>().ToIso8601()));

            #endregion

            #region - UsersResponse - 

            // Just to make the strings a little easier to read during test debugging
            fixture.Customize<UsersResponse.UserType.DomainType>(composer => composer
                .With(d => d.Name, $"DomainName{Guid.NewGuid()}"));

            // Wrong - Work item in in backlog
            // The domain does not go into the name for UsersResponse.UserType. Also, domain can never be "local" 
            // here. If code tries something like Create<UsersResponse.UserType>.With(u => domain.name, "local"), then this code is skipped
            fixture.Customize<UsersResponse.UserType>(composer => composer
                .With(
                u => u.Name,
                (UsersResponse.UserType.DomainType domain) =>
                {
                    var plainUserName = $"Name{Guid.NewGuid()}";
                    var domainName = domain?.Name;

                    return string.Equals(domainName, "local", StringComparison.OrdinalIgnoreCase)
                    ? plainUserName
                    : $"{domainName}{Constants.DomainNameSeparator}{plainUserName}";
                }));

            #endregion

            #region - CreateProjectResponse -

            // These properties should return Guid strings instead of the default PropertyName/Guid ones.
            fixture.Customize<CreateProjectResponse.ProjectType>(composer => composer
                .With(p => p.ParentProjectId, () => fixture.Create<Guid?>()?.ToString()));

            #endregion

            #region - ProjectsResponse -

            // These properties should return Guid strings instead of the default PropertyName/Guid ones.
            fixture.Customize<ProjectsResponse.ProjectType>(composer => composer
                .With(p => p.ParentProjectId, () => fixture.Create<Guid?>()?.ToString()));

            #endregion

            #region - UpdateDataSourceResponse -

            // These properties should return DateTime strings instead of the default Guid-like ones.
            fixture.Customize<UpdateDataSourceResponse.DataSourceType>(composer => composer
                .With(j => j.CreatedAt, () => fixture.Create<DateTime>().ToIso8601())
                .With(j => j.UpdatedAt, () => fixture.Create<DateTime>().ToIso8601()));

            #endregion

            #region - UpdateWorkbookResponse -

            // These properties should return DateTime strings instead of the default Guid-like ones.
            fixture.Customize<UpdateWorkbookResponse.WorkbookType>(composer => composer
                .With(j => j.CreatedAt, () => fixture.Create<DateTime>().ToIso8601())
                .With(j => j.UpdatedAt, () => fixture.Create<DateTime>().ToIso8601()));

            #endregion

            #region - UpdateConnectionRequest -

            // These properties should return nullable bool strings instead of the default Guid-like ones.
            fixture.Customize<UpdateConnectionRequest.ConnectionType>(composer => composer
                .With(j => j.EmbedPassword, () => fixture.Create<bool?>().ToString())
                .With(j => j.QueryTaggingEnabled, () => fixture.Create<bool?>().ToString()));

            #endregion

            #region - ConnectionsResponse - 

            // These properties should return nullable bool strings instead of the default Guid-like ones.
            fixture.Customize<ConnectionsResponse.ConnectionType>(composer => composer
                .With(j => j.QueryTaggingEnabled, () => fixture.Create<bool?>().ToString()));

            #endregion

            #region - ConnectionResponse - 

            // These properties should return nullable bool strings instead of the default Guid-like ones.
            fixture.Customize<ConnectionResponse.ConnectionType>(composer => composer
                .With(j => j.QueryTaggingEnabled, () => fixture.Create<bool?>().ToString()));

            #endregion

            #region - Server.ExtractRefreshTasksResponse - 

            fixture.Customize<ServerResponses.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.ScheduleType>(
                composer
                => composer
                    .With(j => j.CreatedAt, () => fixture.Create<DateTime?>()?.ToIso8601())
                    .With(j => j.UpdatedAt, () => fixture.Create<DateTime?>()?.ToIso8601())
                    .With(j => j.NextRunAt, () => fixture.Create<DateTime?>()?.ToIso8601()));

            #endregion

            #region - Cloud.ExtractRefreshTasksResponse - 

            fixture.Customize<CloudResponses.ExtractRefreshTasksResponse.TaskType.ExtractRefreshType.ScheduleType.FrequencyDetailsType>(
                composer
                => composer
                    .With(j => j.Start, () => fixture.Create<TimeOnly?>()?.ToString())
                    .With(j => j.End, () => fixture.Create<TimeOnly?>()?.ToString()));

            #endregion

            #region - Cloud.CreateExtractRefreshTaskResponse - 

            fixture.Customize<CloudResponses.CreateExtractRefreshTaskResponse.ScheduleType>(
                composer
                => composer
                    .With(j => j.NextRunAt, () => fixture.Create<DateTime?>()?.ToIso8601()));
            fixture.Customize<CloudResponses.CreateExtractRefreshTaskResponse.ScheduleType.FrequencyDetailsType>(
                composer
                => composer
                    .With(j => j.Start, () => fixture.Create<TimeOnly?>()?.ToString())
                    .With(j => j.End, () => fixture.Create<TimeOnly?>()?.ToString()));

            #endregion

            #region - Server.ScheduleExtractsResponse - 

            string GetRandomExtractType()
            {
                var extractTypes = new List<string> { "IncrementalRefresh", "FullRefresh" };
                var random = new Random();
                int index = random.Next(extractTypes.Count);
                return extractTypes[index];
            }

            fixture.Customize<ServerResponses.ScheduleExtractRefreshTasksResponse.ExtractType>(
                composer
                => composer
                    .With(j => j.Type, () => GetRandomExtractType())
                    );
            #endregion

            return fixture;
        }
    }
}
