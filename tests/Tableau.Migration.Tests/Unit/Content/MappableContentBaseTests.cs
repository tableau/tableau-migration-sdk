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
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public sealed class MappableContentBaseTests
    {
        public sealed class TestMappableContentBase : MappableContentBase
        { }

        public sealed class SetLocation
        {
            [Fact]
            public void SetsLocationAndName()
            {
                var c = new TestMappableContentBase();

                var parent = new ContentReferenceStub(Guid.NewGuid(), string.Empty, new("parent", "project"));
                var loc = parent.Location.Append("Name");

                ((IMappableContent)c).SetLocation(loc);

                Assert.Equal(loc, c.Location);
                Assert.Equal("Name", c.Name);
            }
        }
    }
}
