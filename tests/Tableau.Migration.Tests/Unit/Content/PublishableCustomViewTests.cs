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

using System.Linq;
using AutoFixture;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class PublishableCustomViewTests : AutoFixtureTestBase
    {
        public class Ctor : PublishableCustomViewTests
        {
            [Fact]
            public void Success()
            {
                var customView = AutoFixture.Create<ICustomView>();
                var file = AutoFixture.Create<IContentFileHandle>();
                var defaultUsers = AutoFixture.CreateMany<IContentReference>().ToList();

                var result = new PublishableCustomView(customView, defaultUsers, file);

                Assert.NotNull(result);

                Assert.Equal(customView.Id, result.Id);
                Assert.Equal(customView.Name, result.Name);
                Assert.Equal(customView.CreatedAt, result.CreatedAt);
                Assert.Equal(customView.UpdatedAt, result.UpdatedAt);
                Assert.Equal(customView.LastAccessedAt, result.LastAccessedAt);
                Assert.Equal(customView.Shared, result.Shared);
                Assert.Same(customView.Workbook, result.Workbook);
                Assert.Same(customView.Owner, result.Owner);
                Assert.Equal(customView.BaseViewId, result.BaseViewId);
                Assert.Equal(customView.BaseViewName, result.BaseViewName);


                Assert.Same(file, result.File);
                Assert.Same(defaultUsers, result.DefaultUsers);
            }
        }
    }
}

