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
using System.Linq;
using Tableau.Migration.JsonConverters.Exceptions;
using Xunit;

namespace Tableau.Migration.Tests
{
    public class AutoFixtureTestBaseTests : AutoFixtureTestBase
    {
        [Fact]
        public void Verify_CreateErrors_create_all_exceptions()
        {
            // Create all the exceptions
            var errors = FixtureFactory.CreateErrors(AutoFixture);

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var tableauMigrationAssembly = loadedAssemblies.Where(a => a.ManifestModule.Name == "Tableau.Migration.dll").First();

            // Find all the exception types in the Tableau.Migration assembly
            var exceptionTypes = tableauMigrationAssembly.GetTypes()
                .Where(t => t.BaseType == typeof(Exception))
                .Where(t => t != typeof(MismatchException))
                .ToList();

            // Assert that all the types in exceptionTypes exist in the errors list
            Assert.True(exceptionTypes.All(t => errors.Any(e => e.GetType() == t)));
        }

        [Fact]
        public void Verify_CreateErrors_nullable_properties_not_null()
        {
            string[] ignoredPropertyNames = new string[] { "InnerException" };

            // Call CreateErrors
            var errors = FixtureFactory.CreateErrors(AutoFixture);

            // Verify that every property in all the objects is not null
            foreach (var error in errors)
            {
                var properties = error.GetType().GetProperties()
                    .Where(prop => !ignoredPropertyNames.Contains(prop.Name))
                    .ToArray();

                Assert.All(properties, (prop) => Assert.NotNull(prop));

                foreach (var property in properties)
                {
                    var value = property.GetValue(error);
                    Assert.NotNull(value);
                }
            }
        }
    }
}
