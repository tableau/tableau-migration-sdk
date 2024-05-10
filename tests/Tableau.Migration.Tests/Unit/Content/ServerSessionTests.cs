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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class ServerSessionTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void RequiresResponseItem()
            {
                var req = new ServerSessionResponse();

                Assert.Throws<ArgumentNullException>(() => new ServerSession(req));
            }

            [Fact]
            public void RequiresSite()
            {
                var req = new ServerSessionResponse()
                {
                    Item = new()
                };

                Assert.Throws<ArgumentNullException>(() => new ServerSession(req));
            }

            [Fact]
            public void RequiresSiteId()
            {
                var req = Create<ServerSessionResponse>();
                req.Item!.Site!.Id = Guid.Empty;

                Assert.Throws<ArgumentException>(() => new ServerSession(req));
            }

            [Fact]
            public void RequiresSiteContentUrl()
            {
                var req = Create<ServerSessionResponse>();
                req.Item!.Site!.ContentUrl = null;

                Assert.Throws<ArgumentNullException>(() => new ServerSession(req));
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void RequiresSiteName(string? s)
            {
                var req = Create<ServerSessionResponse>();
                req.Item!.Site!.Name = s;

                Assert.Throws<ArgumentException>(() => new ServerSession(req));
            }

            [Fact]
            public void RequiresUserItem()
            {
                var req = Create<ServerSessionResponse>();
                req.Item!.User = null;

                Assert.Throws<ArgumentNullException>(() => new ServerSession(req));
            }

            [Fact]
            public void NonAdministrator()
            {
                var req = Create<ServerSessionResponse>();
                req.Item!.User!.SiteRole = SiteRoles.ExplorerCanPublish;

                var s = new ServerSession(req);

                Assert.False(s.IsAdministrator);
                Assert.Null(s.Settings);
            }

            [Fact]
            public void Administrator()
            {
                var req = Create<ServerSessionResponse>();
                req.Item!.User!.SiteRole = SiteRoles.SiteAdministratorExplorer;

                var s = new ServerSession(req);

                Assert.True(s.IsAdministrator);
                Assert.NotNull(s.Settings);
            }
        }
    }
}
