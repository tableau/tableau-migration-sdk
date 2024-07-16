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
using System.Linq;
using System.Threading;
using AutoFixture;
using AutoFixture.Kernel;
using Moq;
using Tableau.Migration.Engine.Manifest;

namespace Tableau.Migration.Tests
{
    public abstract class AutoFixtureTestBase
    {
        /// <summary>
        /// The configured <see cref="IFixture"/> for this instance.
        /// </summary>
        protected readonly IFixture AutoFixture = CreateFixture();

        protected readonly Mock<IMigrationManifestEntryBuilder> MockEntryBuilder;

        protected readonly CancellationTokenSource CancelSource = new();
        protected CancellationToken Cancel => CancelSource.Token;

        protected readonly TimeSpan TestCancellationTimeout;

        public AutoFixtureTestBase()
        {
            var testCancellationTimeoutConfig = Environment.GetEnvironmentVariable("MIGRATIONSDK_TEST_CANCELLATION_TIMEOUT_TIMESPAN");

            if (!TimeSpan.TryParse(testCancellationTimeoutConfig, out TestCancellationTimeout))
            {
                TestCancellationTimeout = TimeSpan.FromSeconds(15);
            }

            MockEntryBuilder = Create<Mock<IMigrationManifestEntryBuilder>>();
        }

        /// <summary>
        /// Creates a new <see cref="IFixture"/> instance.
        /// </summary>
        protected static IFixture CreateFixture() => FixtureFactory.Create();

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
        protected T Create<T>(Action<T>? configure = null)
        {
            var obj = AutoFixture.Create<T>();

            configure?.Invoke(obj);

            return obj;
        }

        /// <summary>
        /// Creates a string variable.
        /// </summary>
        /// <param name="length">The lendth of the string.</param>
        protected string CreateString(int length)
            => new(CreateMany<char>(length).ToArray());

        /// <summary>
        /// Creates a string variable.
        /// </summary>
        protected string CreateString()
            => new(CreateMany<char>().ToArray());

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

        /// <summary>
        /// Freezes the type to a single value.
        /// </summary>
        /// <typeparam name="T">The type of object to freeze.</typeparam>
        /// <param name="value">The value to freeze.</param>
        /// <returns>The value that will subsequently always be created for <typeparamref name="T" />.</returns>
        protected T Freeze<T>(T value) => AutoFixture.Freeze<T>(composer => composer.FromFactory(() => value));
    }
}
