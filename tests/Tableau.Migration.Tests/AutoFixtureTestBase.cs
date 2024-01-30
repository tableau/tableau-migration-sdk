// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Tests
{
    public abstract class AutoFixtureTestBase
    {
        /// <summary>
        /// The configured <see cref="IFixture"/> for this instance.
        /// </summary>
        protected readonly IFixture AutoFixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

        protected readonly CancellationTokenSource CancelSource = new();
        protected CancellationToken Cancel => CancelSource.Token;

        protected readonly TimeSpan TestCancellationTimeout;

        public AutoFixtureTestBase()
        {
            Customize();

            var testCancellationTimeoutConfig = Environment.GetEnvironmentVariable("MIGRATIONSDK_TEST_CANCELLATION_TIMEOUT_TIMESPAN");

            if (!TimeSpan.TryParse(testCancellationTimeoutConfig, out TestCancellationTimeout))
            {
                TestCancellationTimeout = TimeSpan.FromSeconds(15);
            }
        }

        /// <summary>
        /// Creates a variable of the requested type.
        /// </summary>
        /// <param name="t">The type of object to create.</param>
        /// <returns>An object of type <paramref name="t" />.</returns>
        protected object Create(Type t) => AutoFixture.Create(t, new SpecimenContext(AutoFixture));

        /// <summary>
        /// Creates a variable of the requested type.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <returns>An object of type <typeparamref name="T"/></returns>
        protected T Create<T>() => AutoFixture.Create<T>();

        /// <summary>
        /// Create variables of the requested type.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <returns>A collection of type <typeparamref name="T"/></returns>
        protected IEnumerable<T> CreateMany<T>() => AutoFixture.CreateMany<T>();

        /// <summary>
        /// Create variables of the requested type.
        /// </summary>
        /// <param name="count">The number of objects to create.</param>
        /// <typeparam name="T">The type of objects to create.</typeparam>
        /// <returns>A collection of type <typeparamref name="T"/></returns>
        protected IEnumerable<T> CreateMany<T>(int generatedCount, IEnumerable<T>? first = null, IEnumerable<T>? last = null)
        {
            var list = new List<T>();

            if (first is not null)
                list.AddRange(first);

            list.AddRange(AutoFixture.CreateMany<T>(generatedCount));

            if (last is not null)
                list.AddRange(last);

            return list;
        }

        /// <summary>
        /// Freezes the type to a single value.
        /// </summary>
        /// <typeparam name="T">The type of object to freeze.</typeparam>
        /// <returns>The value that will subsequently always be created for <typeparamref name="T" />.</returns>
        protected T Freeze<T>() => AutoFixture.Freeze<T>();

        private void Customize()
        {
            AutoFixture.Register(() => Create<MockServiceProvider>().Object);

            AutoFixture.Register(() => new ContentLocation(CreateMany<string>()));

            AutoFixture.Register<string, Stream>((string data) =>
            {
                var bytes = Encoding.UTF8.GetBytes(data);
                return new MemoryStream(bytes);
            });

            #region - JobResponse -

            // These properties should return DateTime strings instead of the default Guid-like ones.
            AutoFixture.Customize<JobResponse.JobType>(composer => composer
                .With(j => j.CreatedAt, () => Create<DateTime>().ToIso8601())
                .With(j => j.UpdatedAt, () => Create<DateTime>().ToIso8601())
                .With(j => j.CompletedAt, () => Create<DateTime?>()?.ToIso8601()));

            #endregion

            #region - ImportJobResponse -

            // These properties should return DateTime strings instead of the default Guid-like ones.
            AutoFixture.Customize<ImportJobResponse.ImportJobType>(composer => composer
                .With(j => j.CreatedAt, () => Create<DateTime>().ToIso8601()));

            #endregion

            #region - UsersResponse - 

            // Just to make the strings a little easier to read during test debugging
            AutoFixture.Customize<UsersResponse.UserType.DomainType>(composer => composer
                .With(d => d.Name, $"DomainName{Guid.NewGuid()}"));

            // Wrong - Work item in in backlog
            // The domain does not go into the name for UsersResponse.UserType. Also, domain can never be "local" 
            // here. If code tries something like Create<UsersResponse.UserType>.With(u => domain.name, "local"), then this code is skipped
            AutoFixture.Customize<UsersResponse.UserType>(composer => composer
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
            AutoFixture.Customize<CreateProjectResponse.ProjectType>(composer => composer
                .With(p => p.ParentProjectId, () => Create<Guid?>()?.ToString()));

            #endregion

            #region - ProjectsResponse -

            // These properties should return Guid strings instead of the default PropertyName/Guid ones.
            AutoFixture.Customize<ProjectsResponse.ProjectType>(composer => composer
                .With(p => p.ParentProjectId, () => Create<Guid?>()?.ToString()));

            #endregion

            #region - UpdateDataSourceResponse -

            // These properties should return DateTime strings instead of the default Guid-like ones.
            AutoFixture.Customize<UpdateDataSourceResponse.DataSourceType>(composer => composer
                .With(j => j.CreatedAt, () => Create<DateTime>().ToIso8601())
                .With(j => j.UpdatedAt, () => Create<DateTime>().ToIso8601()));

            #endregion

            #region - UpdateWorkbookResponse -

            // These properties should return DateTime strings instead of the default Guid-like ones.
            AutoFixture.Customize<UpdateWorkbookResponse.WorkbookType>(composer => composer
                .With(j => j.CreatedAt, () => Create<DateTime>().ToIso8601())
                .With(j => j.UpdatedAt, () => Create<DateTime>().ToIso8601()));

            #endregion

            #region - UpdateConnectionRequest -

            // These properties should return nullable bool strings instead of the default Guid-like ones.
            AutoFixture.Customize<UpdateConnectionRequest.ConnectionType>(composer => composer
                .With(j => j.EmbedPassword, () => Create<bool?>().ToString())
                .With(j => j.QueryTaggingEnabled, () => Create<bool?>().ToString()));

            #endregion

            #region - ConnectionsResponse - 

            // These properties should return nullable bool strings instead of the default Guid-like ones.
            AutoFixture.Customize<ConnectionsResponse.ConnectionType>(composer => composer
                .With(j => j.QueryTaggingEnabled, () => Create<bool?>().ToString()));

            #endregion

            #region - ConnectionResponse - 
            
            // These properties should return nullable bool strings instead of the default Guid-like ones.
            AutoFixture.Customize<ConnectionResponse.ConnectionType>(composer => composer
                .With(j => j.QueryTaggingEnabled, () => Create<bool?>().ToString()));

            #endregion
        }
    }
}
