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
using AutoFixture;
using Moq;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;

namespace Tableau.Migration.Tests.Unit.Content.Schedules
{
    public abstract class ScheduleTestBase : AutoFixtureTestBase
    {
        protected static IInterval CreateInterval(int? hours = null, int? minutes = null, string? weekDay = null, string? monthDay = null)
            => new Interval(hours, minutes, weekDay, monthDay);

        protected static IFrequencyDetails CreateFrequencyDetails(params IInterval[] intervals)
            => CreateFrequencyDetails(null, null, intervals);

        protected static IFrequencyDetails CreateFrequencyDetails(TimeOnly? startAt, TimeOnly? endAt, params IInterval[] intervals)
            => new FrequencyDetails(startAt, endAt, intervals);

        protected static IFrequencyDetails CreateFrequencyDetails(DateTime startAt, TimeSpan duration, params IInterval[] intervals)
            => CreateFrequencyDetails(TimeOnly.FromDateTime(startAt), TimeOnly.FromDateTime(startAt.Add(duration)), intervals);

        protected TSchedule CreateSchedule<TSchedule>(string? frequency = null, IFrequencyDetails? frequencyDetails = null)
            where TSchedule : class, ISchedule
        {
            var mockSchedule = Create<Mock<TSchedule>>(m =>
            {
                if (frequency is not null)
                    m.SetupGet(s => s.Frequency).Returns(frequency);

                if (frequencyDetails is not null)
                    m.SetupGet(s => s.FrequencyDetails).Returns(frequencyDetails);
            });

            return mockSchedule.Object;
        }

        protected ICloudSchedule CreateCloudSchedule(string? frequency = null, IFrequencyDetails? frequencyDetails = null)
            => CreateSchedule<ICloudSchedule>(frequency, frequencyDetails);

        protected IServerSchedule CreateServerSchedule(string? frequency = null, IFrequencyDetails? frequencyDetails = null)
            => CreateSchedule<IServerSchedule>(frequency, frequencyDetails);

        protected TExtractRefreshType CreateExtractRefreshResponse<TExtractRefreshType, TWorkbook, TDataSource>(
            string? type = null,
            ExtractRefreshContentType? contentType = null,
            Guid? contentId = null,
            Action<TExtractRefreshType>? configure = null)
            where TExtractRefreshType : IExtractRefreshType<TWorkbook, TDataSource>
            where TWorkbook : class, IRestIdentifiable
            where TDataSource : class, IRestIdentifiable
        {
            var response = AutoFixture.Build<TExtractRefreshType>()
                .Without(r => r.DataSource)
                .Without(r => r.Workbook)
                .Create();
            type ??= GetRandomType();
            contentType ??= GetRandomContentType();

            response.Type = type;

            if (contentType is ExtractRefreshContentType.DataSource)
                response.DataSource = CreateRestIdentifiable<TDataSource>(contentId);

            if (contentType is ExtractRefreshContentType.Workbook)
                response.Workbook = CreateRestIdentifiable<TWorkbook>(contentId);

            configure?.Invoke(response);

            return response;
        }

        protected static string GetRandomType()
            => new[] { ExtractRefreshType.FullRefresh, ExtractRefreshType.ServerIncrementalRefresh }.PickRandom();

        protected static ExtractRefreshContentType GetRandomContentType()
            => new[] { ExtractRefreshContentType.DataSource, ExtractRefreshContentType.Workbook }.PickRandom();

        protected TIdentifiable CreateRestIdentifiable<TIdentifiable>(Guid? id = null)
            where TIdentifiable : class, IRestIdentifiable
            => Create<Mock<TIdentifiable>>(m =>
            {
                m.CallBase = true;
                m.SetupGet(w => w.Id).Returns(id ?? Create<Guid>());
            })
            .Object;

        protected IContentReference CreateContentReference() => Create<IContentReference>();

        public class ExtractRefreshContentTypeDataAttribute()
            : EnumDataAttribute<ExtractRefreshContentType>(ExtractRefreshContentType.Unknown)
        { }
    }
}
