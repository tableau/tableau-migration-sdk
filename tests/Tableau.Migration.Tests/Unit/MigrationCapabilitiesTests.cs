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

using System.Reflection;
using AutoFixture.Kernel;
using Xunit;


namespace Tableau.Migration.Tests.Unit
{
    public class MigrationCapabilitiesTests : AutoFixtureTestBase
    {
        [Fact]
        public void Clone_ShouldCloneAllPropertiesCorrectly()
        {
            // Arrange
            var original = new MigrationCapabilities();
            SetNonDefaultValues(original);

            // Act
            var clone = original.Clone();

            // Assert
            foreach (PropertyInfo property in typeof(MigrationCapabilities).GetProperties())
            {
                var originalValue = property.GetValue(original);
                var cloneValue = property.GetValue(clone);
                Assert.Equal(originalValue, cloneValue);
            }
        }

        private void SetNonDefaultValues(MigrationCapabilities instance)
        {
            foreach (PropertyInfo property in typeof(MigrationCapabilities).GetProperties())
            {
                if (property.PropertyType == typeof(bool))
                {
                    var currentValue = (bool)(property.GetValue(instance) ?? false);
                    property.SetValue(instance, !currentValue);
                }
                else
                {
                    var value = AutoFixture.Create(property.PropertyType, new SpecimenContext(AutoFixture));
                    property.SetValue(instance, value);
                }
            }
        }
    }
}

