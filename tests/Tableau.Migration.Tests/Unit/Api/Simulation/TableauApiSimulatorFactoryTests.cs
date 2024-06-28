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
using Moq;
using Tableau.Migration.Api.Simulation;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Simulation
{
    public class TableauApiSimulatorFactoryTests
    {
        public class TableauApiSimulatorFactoryTest : AutoFixtureTestBase
        {
            protected Dictionary<string, TableauApiSimulator> Simulators { get; } = new();

            protected readonly Mock<ITableauApiSimulatorCollection> MockSimulatorCollection;

            public TableauApiSimulatorFactoryTest()
            {
                MockSimulatorCollection = Freeze<Mock<ITableauApiSimulatorCollection>>();
                MockSimulatorCollection.Setup(x => x.ForServer(It.IsAny<Uri>()))
                    .Returns((Uri u) =>
                    {
                        if (Simulators.TryGetValue(u.Host, out var result))
                            return result;

                        return null;
                    });
                MockSimulatorCollection.Setup(x => x.AddOrUpdate(It.IsAny<TableauApiSimulator>()))
                    .Callback((TableauApiSimulator s) => Simulators[s.ServerUrl.Host] = s);
            }

            protected TableauApiSimulatorFactory CreateFactory()
            {
                return Create<TableauApiSimulatorFactory>();
            }
        }

        public class GetOrCreate : TableauApiSimulatorFactoryTest
        {
            [Fact]
            public void NoReCreation()
            {
                var factory = CreateFactory();

                var simulator1 = factory.GetOrCreate(new("http://localhost"), true);
                var simulator2 = factory.GetOrCreate(new("http://localhost"), true);
                
                Assert.Same(simulator1, simulator2);
                MockSimulatorCollection.Verify(x => x.AddOrUpdate(It.IsAny<TableauApiSimulator>()), Times.Once);
            }

            [Fact]
            public void CreatesAuthenticationUser()
            {
                var factory = CreateFactory();

                var simulator = factory.GetOrCreate(new("http://localhost"), true);

                MockSimulatorCollection.Verify(x => x.AddOrUpdate(simulator), Times.Once);

                Assert.NotNull(simulator.Data.SignIn);
            }
        }
    }
}
