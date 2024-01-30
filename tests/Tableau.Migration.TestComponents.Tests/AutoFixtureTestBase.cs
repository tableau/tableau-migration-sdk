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

using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Resources;
using Tableau.Migration.TestComponents.JsonConverters.JsonObjects;

namespace Tableau.Migration.TestComponents.Tests
{
    public abstract class AutoFixtureTestBase
    {
        /// <summary>
        /// The configured <see cref="IFixture"/> for this instance.
        /// </summary>
        protected readonly IFixture AutoFixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

        protected readonly Mock<IMigrationManifestEntryBuilder> MockEntryBuilder;

        public AutoFixtureTestBase()
        {
            MockEntryBuilder = Create<Mock<IMigrationManifestEntryBuilder>>();
            Customize();
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
        protected IEnumerable<T> CreateMany<T>(int count) => AutoFixture.CreateMany<T>(count);

        /// <summary>
        /// Freezes the type to a single value.
        /// </summary>
        /// <typeparam name="T">The type of object to freeze.</typeparam>
        /// <returns>The value that will subsequently always be created for <typeparamref name="T" />.</returns>
        protected T Freeze<T>() => AutoFixture.Freeze<T>();

        /// <summary>
        /// Builds a variable of the requested type
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        protected ICustomizationComposer<T> Build<T>() => AutoFixture.Build<T>();

        /// <summary>
        /// This creates a <see cref="JsonContentReference"/> that follows the requirements of a <see cref="ContentLocation"/>
        /// </summary>
        private JsonContentLocation CreateJsonContentLocation()
        {
            var ret = new JsonContentLocation();

            string[] pathSegments = CreateMany<string>().ToArray();

            ret.PathSeparator = Constants.PathSeparator;
            ret.PathSegments = pathSegments;
            ret.Path = string.Join(Constants.PathSeparator, pathSegments);
            ret.Name = pathSegments.LastOrDefault() ?? string.Empty;
            ret.IsEmpty = (pathSegments.Length == 0);

            return ret;
        }

        /// <summary>
        /// Creates a JsonManifestEntry that follows the requirements of <see cref="MigrationManifestEntry"/>
        /// </summary>
        /// <returns></returns>
        private JsonManifestEntry CreateJsonManifestEntry()
        {
            var ret = new JsonManifestEntry();

            ret.Source = Create<JsonContentReference>();
            ret.Destination = Create<JsonContentReference>();
            ret.MappedLocation = ret.Destination.Location;
            ret.Status = (int)Create<MigrationManifestEntryStatus>();

            return ret;
        }

        private MigrationManifest CreateMigrationManifest()
        {
            var ret = new MigrationManifest(Create<ISharedResourcesLocalizer>(), Create<ILoggerFactory>(), Guid.NewGuid(), Guid.NewGuid());

            foreach (var type in ServerToCloudMigrationPipeline.ContentTypes)
            {
                var p = ret.Entries.GetOrCreatePartition(type.ContentType);
                p.CreateEntries(CreateMany<MigrationManifestEntry>().ToList());
            }

            ret.AddErrors(Create<Exception>());

            return ret;
        }

        private void Customize()
        {
            AutoFixture.Register(() => Create<MockServiceProvider>().Object);

            AutoFixture.Register(() => CreateJsonContentLocation());

            AutoFixture.Customize<JsonContentReference>(composer => composer
                .With(c => c.Id, Guid.NewGuid().ToString()));

            AutoFixture.Register(() => CreateJsonManifestEntry());

            AutoFixture.Register(() => new ContentLocation(CreateMany<string>()));

            AutoFixture.Register(() => new MigrationManifestEntry(MockEntryBuilder.Object, Create<ContentReferenceStub>()));

            AutoFixture.Register<IMigrationManifest>(() => CreateMigrationManifest());
            AutoFixture.Register(() => CreateMigrationManifest());
        }
    }
}
