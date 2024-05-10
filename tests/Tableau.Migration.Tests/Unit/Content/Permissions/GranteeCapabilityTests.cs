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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Permissions
{
    public class GranteeCapabilityTests : AutoFixtureTestBase
    {
        public class GranteeCapabilityTest : GranteeCapabilityTests
        {
            public class Ctor : GranteeCapabilityTest
            {
                [Fact]
                public void Capabilities_always_unique()
                {
                    var dupeCapability = Create<ICapability>();
                    var dupeCapabilityUpper = new Capability(dupeCapability.Name.ToUpper(), dupeCapability.Mode.ToUpper());
                    var nonDupeCapability = Create<ICapability>();

                    var testData = new GranteeCapability(
                        GranteeType.Group,
                        Guid.NewGuid(),
                        new[] { dupeCapability, dupeCapability, dupeCapabilityUpper, nonDupeCapability });

                    var result = testData.Capabilities;

                    // The result should contain 1 of dupeCapability and 1 of nonDupeCapability
                    Assert.Equal(2, result.Count);

                    // testData has 2 of these but the result should contain 1
                    var dupeCapabilityCount = result.Where(c => ICapabilityComparer.Instance.Equals(c, dupeCapability)).Count();
                    Assert.Equal(1, dupeCapabilityCount);

                    // testData has 1 of these and the result should contain 1
                    var nonDupeCapabilityCount = result.Where(c => ICapabilityComparer.Instance.Equals(c, nonDupeCapability)).Count();
                    Assert.Equal(1, nonDupeCapabilityCount);
                }
            }

            public class ResolveCapabilityModeConflicts : GranteeCapabilityTest
            {
                [Fact]
                public void Non_conflicting_modes_unchanged()
                {
                    var testCapabilities = new ICapability[]
                    {
                        new Capability(Create<string>(), PermissionsCapabilityModes.Allow),
                        new Capability(Create<string>(), PermissionsCapabilityModes.Deny),
                        new Capability(Create<string>(), PermissionsCapabilityModes.Deny)
                    };

                    var testGranteeCapability = new GranteeCapability(
                       GranteeType.Group,
                       Guid.NewGuid(),
                       testCapabilities);

                    var result = testGranteeCapability.Capabilities;
                    Assert.Equal(testCapabilities.Length, result.Count);
                }

                [Fact]
                public void Resolves_conflicting_modes()
                {
                    var repeatedCapabilityName = Create<string>();
                    var uniqueNameCount = 3;

                    var testCapabilities =
                        CreateMany<ICapability>(uniqueNameCount)
                        .Append(new Capability(repeatedCapabilityName, PermissionsCapabilityModes.Allow))
                        .Append(new Capability(repeatedCapabilityName.ToLower(), PermissionsCapabilityModes.Allow))
                        .Append(new Capability(repeatedCapabilityName, PermissionsCapabilityModes.Deny))
                        .Append(new Capability(repeatedCapabilityName.ToLower(), PermissionsCapabilityModes.Deny));

                    var testGranteeCapability = new GranteeCapability(
                       GranteeType.Group,
                       Guid.NewGuid(),
                       testCapabilities);

                    testGranteeCapability.ResolveCapabilityModeConflicts();

                    var result = testGranteeCapability.Capabilities;

                    // There should not be a capability that has a conflict with another (has the same name but different mode)
                    Assert.Equal(uniqueNameCount + 1, result.Count);
                    Assert.All(result, cap =>
                    {
                        Assert.False(cap.Mode == PermissionsCapabilityModes.Allow && cap.Name == repeatedCapabilityName);
                    });

                    Assert.Single(
                        result.Where(
                            cap =>
                            cap.Name == repeatedCapabilityName &&
                            cap.Mode == PermissionsCapabilityModes.Deny));
                }
                [Fact]
                public void Does_not_delete_all_allow()
                {
                    var repeatedCapabilityName = Create<string>();
                    var uniqueNameCount = 3;

                    var testCapabilities =
                        CreateMany<ICapability>(uniqueNameCount)
                        .Append(new Capability(repeatedCapabilityName, PermissionsCapabilityModes.Allow))
                        .Append(new Capability(repeatedCapabilityName.ToLower(), PermissionsCapabilityModes.Allow));

                    var testGranteeCapability = new GranteeCapability(
                       GranteeType.Group,
                       Guid.NewGuid(),
                       testCapabilities);

                    var result = testGranteeCapability.Capabilities;

                    // There should not be a capability that has a conflict with another (has the same name but different mode)
                    Assert.Equal(uniqueNameCount + 1, result.Count);

                    Assert.Single(
                        result.Where(
                            cap =>
                            cap.Name == repeatedCapabilityName &&
                            cap.Mode == PermissionsCapabilityModes.Allow));
                }
            }
        }
    }
}
