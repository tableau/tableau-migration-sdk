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

using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    public class UpdateSiteRequestTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var update = Create<ISiteSettingsUpdate>();

                var req = new UpdateSiteRequest(update);

                Assert.NotNull(req.Site);
                Assert.Equal(update.ExtractEncryptionMode, req.Site.ExtractEncryptionMode);
                Assert.Equal(update.DisableSubscriptions, req.Site.DisableSubscriptions);
                Assert.Equal(update.GroupSetsEnabled, req.Site.GroupSetsEnabled);
            }
        }

        public sealed class DisableSubscriptions : AutoFixtureTestBase
        {
            [Fact]
            public void GetsNull()
            {
                var req = new UpdateSiteRequest(Create<ISiteSettingsUpdate>());
                Assert.NotNull(req.Site);

                req.Site.DisableSubscriptionsValue = null;
                Assert.Null(req.Site.DisableSubscriptions);
            }

            [Fact]
            public void GetsBool()
            {
                var req = new UpdateSiteRequest(Create<ISiteSettingsUpdate>());
                Assert.NotNull(req.Site);

                req.Site.DisableSubscriptionsValue = "true";
                Assert.True(req.Site.DisableSubscriptions);
            }

            [Fact]
            public void SetsNull()
            {
                var req = new UpdateSiteRequest(Create<ISiteSettingsUpdate>());
                Assert.NotNull(req.Site);

                req.Site.DisableSubscriptions = null;
                Assert.Null(req.Site.DisableSubscriptionsValue);
            }

            [Fact]
            public void SetsBool()
            {
                var req = new UpdateSiteRequest(Create<ISiteSettingsUpdate>());
                Assert.NotNull(req.Site);

                req.Site.DisableSubscriptions = true;
                Assert.Equal("true", req.Site.DisableSubscriptionsValue);
            }
        }

        public sealed class GroupSetsEnabled : AutoFixtureTestBase
        {
            [Fact]
            public void GetsNull()
            {
                var req = new UpdateSiteRequest(Create<ISiteSettingsUpdate>());
                Assert.NotNull(req.Site);

                req.Site.GroupSetsEnabledValue = null;
                Assert.Null(req.Site.GroupSetsEnabled);
            }

            [Fact]
            public void GetsBool()
            {
                var req = new UpdateSiteRequest(Create<ISiteSettingsUpdate>());
                Assert.NotNull(req.Site);

                req.Site.GroupSetsEnabledValue = "true";
                Assert.True(req.Site.GroupSetsEnabled);
            }

            [Fact]
            public void SetsNull()
            {
                var req = new UpdateSiteRequest(Create<ISiteSettingsUpdate>());
                Assert.NotNull(req.Site);

                req.Site.GroupSetsEnabled = null;
                Assert.Null(req.Site.GroupSetsEnabledValue);
            }

            [Fact]
            public void SetsBool()
            {
                var req = new UpdateSiteRequest(Create<ISiteSettingsUpdate>());
                Assert.NotNull(req.Site);

                req.Site.GroupSetsEnabled = true;
                Assert.Equal("true", req.Site.GroupSetsEnabledValue);
            }
        }
    }
}
