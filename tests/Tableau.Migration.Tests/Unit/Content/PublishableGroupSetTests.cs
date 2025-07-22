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

using System.Collections.Generic;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public sealed class PublishableGroupSetTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void WithResponse()
            {
                var r = Create<IGroupSetType>();
                var groups = Create<List<IContentReference>>();

                var gs = new PublishableGroupSet(r, groups);

                Assert.Equal(r.Id, gs.Id);
                Assert.Equal(r.Name, gs.Name);
                Assert.Equal(new ContentLocation(r.Name!), gs.Location);
                Assert.Equal(string.Empty, gs.ContentUrl);

                Assert.Same(groups, gs.Groups);
            }

            [Fact]
            public void CopyCtor()
            {
                var copy = Create<IGroupSet>();
                var groups = Create<List<IContentReference>>();

                var gs = new PublishableGroupSet(copy, groups);

                Assert.Equal(copy.Id, gs.Id);
                Assert.Equal(copy.Name, gs.Name);
                Assert.Equal(new ContentLocation(copy.Name), gs.Location);
                Assert.Equal(string.Empty, gs.ContentUrl);

                Assert.Same(groups, gs.Groups);
            }
        }
    }
}
