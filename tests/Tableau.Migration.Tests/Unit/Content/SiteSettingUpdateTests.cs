﻿//
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
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class SiteSettingUpdateTests
    {
        public class Ctor
        {
            [Fact]
            public void Initializes()
            {
                var id = Guid.NewGuid();
                var u = new SiteSettingsUpdate(id);

                Assert.Equal(id, u.SiteId);
            }
        }

        public class NeedsUpdate : AutoFixtureTestBase
        {
            [Fact]
            public void NoUpdates()
            {
                var u = new SiteSettingsUpdate(Guid.NewGuid());

                Assert.False(u.NeedsUpdate());
            }

            [Fact]
            public void ExtractEncryptionModeUpdated()
            {
                var u = new SiteSettingsUpdate(Guid.NewGuid())
                {
                    ExtractEncryptionMode = Create<string>()
                };

                Assert.True(u.NeedsUpdate());
            }

            [Fact]
            public void DisableSubscriptionsUpdated()
            {
                var u = new SiteSettingsUpdate(Guid.NewGuid())
                {
                    DisableSubscriptions = Create<bool>()
                };

                Assert.True(u.NeedsUpdate());
            }

            [Fact]
            public void GroupSetsEnabledUpdated()
            {
                var u = new SiteSettingsUpdate(Guid.NewGuid())
                {
                    GroupSetsEnabled = Create<bool>()
                };

                Assert.True(u.NeedsUpdate());
            }
        }
    }
}
