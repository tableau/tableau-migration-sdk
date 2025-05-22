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
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks
{
    public sealed class InitializeMigrationHookResultTests
    {
        public sealed class Succeeded : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var services = Create<IServiceProvider>();
                var session = Create<IServerSession>();

                var r = InitializeMigrationHookResult.Succeeded(services, session);

                Assert.Same(services, r.ScopedServices);
                Assert.Same(session, r.DestinationServerSession);
            }
        }

        public sealed class ToFailure : AutoFixtureTestBase
        {
            [Fact]
            public void SetsFailure()
            {
                var services = Create<IServiceProvider>();
                var session = Create<IServerSession>();

                var errors = CreateMany<Exception>().ToArray();

                IInitializeMigrationHookResult r = InitializeMigrationHookResult.Succeeded(services, session);
                r = r.ToFailure(errors);

                r.AssertFailure();

                Assert.Equal(errors, r.Errors);
                Assert.Same(services, r.ScopedServices);
                Assert.Same(session, r.DestinationServerSession);
            }

            [Fact]
            public void AppendsErrors()
            {
                var services = Create<IServiceProvider>();
                var session = Create<IServerSession>();

                var errors1 = CreateMany<Exception>().ToArray();
                var errors2 = CreateMany<Exception>().ToArray();

                IInitializeMigrationHookResult r = InitializeMigrationHookResult.Succeeded(services, session);
                r = r.ToFailure(errors1);
                r = r.ToFailure(errors2);

                r.AssertFailure();

                Assert.Equal(errors1.Concat(errors2), r.Errors);
                Assert.Same(services, r.ScopedServices);
                Assert.Same(session, r.DestinationServerSession);
            }
        }
    }
}
