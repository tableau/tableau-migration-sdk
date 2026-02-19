//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Services;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Services
{
    public sealed class MigrationServiceBuilderTests
    {
        #region - Test Classes -

        public class MigrationServiceBuilderTest : AutoFixtureTestBase
        {
            protected readonly IImmutableList<Type> SupportedServices;

            internal readonly MigrationServiceBuilder Builder;

            public MigrationServiceBuilderTest()
            {
                SupportedServices = CreateMany<Type>().ToImmutableList();

                Builder = new(SupportedServices);
            }

            protected TestContentType TestFactory(MigrationServiceFactoryContext _) => new();
        }

        #endregion

        #region - SupportedServices -

        public sealed class SupportedServices : MigrationServiceBuilderTest
        {
            [Fact]
            public void AssignedFromCtor()
            {
                Assert.Same(SupportedServices, Builder.SupportedServices);
            }
        }

        #endregion

        #region - GetServiceFactory/Set -

        public sealed class GetSet : MigrationServiceBuilderTest
        {
            [Fact]
            public void NoFactoryRegistered()
            {
                Assert.Null(Builder.GetServiceFactory(typeof(TestContentType)));
            }

            [Fact]
            public void GetsRegisteredFactory()
            {
                var factory = new MigrationServiceFactory(TestFactory);

                Builder.Set<TestContentType>(factory);
                var f = Builder.GetServiceFactory(typeof(TestContentType));

                Assert.Same(factory, f);
            }

            [Fact]
            public void Generic()
            {
                var factory = new MigrationServiceFactory(TestFactory);

                Builder.Set<TestContentType>(factory);
                var f = Builder.GetServiceFactory<TestContentType>();

                Assert.Same(factory, f);
            }
        }

        #endregion

        #region - GetService -

        public sealed class GetService : MigrationServiceBuilderTest
        {
            [Fact]
            public void NoFactoryFallsBackToServiceProvider()
            {
                var o = new TestContentType();

                var serviceCollection = new ServiceCollection();
                serviceCollection.AddSingleton(o);

                using var services = serviceCollection.BuildServiceProvider();

                var s = Builder.GetService<TestContentType>(services);

                Assert.Same(o, s);
            }

            [Fact]
            public void UsesRegisteredFactory()
            {
                var o = new TestContentType();

                TestContentType MyFactory(MigrationServiceFactoryContext _) => o;

                var serviceCollection = new ServiceCollection();
                using var services = serviceCollection.BuildServiceProvider();

                Builder.Set<TestContentType>(MyFactory);
                var s = Builder.GetService<TestContentType>(services);

                Assert.Same(o, s);
            }

            [Fact]
            public void UsesOpenGenericFactory()
            {
                var o = Freeze<IContentReferenceFinder<IUser>>();

                object MyFactory(MigrationServiceFactoryContext ctx) => Create(ctx.Type);
                
                var serviceCollection = new ServiceCollection();
                using var services = serviceCollection.BuildServiceProvider();

                Builder.Set(typeof(IContentReferenceFinder<>), MyFactory);
                var s = Builder.GetService<IContentReferenceFinder<IUser>>(services);

                Assert.Same(o, s);
            }

            [Fact]
            public void UsesSpecificOverOpenGenericFactory()
            {
                var o1 = Create<IContentReferenceFinder<IUser>>();
                var o2 = Freeze<IContentReferenceFinder<IUser>>();

                object OpenGenericFactory(MigrationServiceFactoryContext ctx) => Create(ctx.Type);

                object SpecificFactory(MigrationServiceFactoryContext ctx) => o1;

                var serviceCollection = new ServiceCollection();
                using var services = serviceCollection.BuildServiceProvider();

                Builder.Set(typeof(IContentReferenceFinder<>), OpenGenericFactory);
                Builder.Set(typeof(IContentReferenceFinder<IUser>), SpecificFactory);
                var s = Builder.GetService<IContentReferenceFinder<IUser>>(services);

                Assert.Same(o1, s);
            }
        }

        #endregion

        #region - Remove -

        public sealed class Remove : MigrationServiceBuilderTest
        {
            [Fact]
            public void Noop()
            {
                Builder.Remove(typeof(TestContentType));
            }

            [Fact]
            public void RemovesFactory()
            {
                Builder.Set(typeof(TestContentType), TestFactory);
                Builder.Remove(typeof(TestContentType));

                Assert.Null(Builder.GetServiceFactory(typeof(TestContentType)));
            }

            [Fact]
            public void Generic()
            {
                Builder.Set<TestContentType>(TestFactory);
                Builder.Remove<TestContentType>();

                Assert.Null(Builder.GetServiceFactory<TestContentType>());
            }
        }

        #endregion
    }
}
