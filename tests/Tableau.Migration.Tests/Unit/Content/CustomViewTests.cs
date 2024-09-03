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

using AutoFixture;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class CustomViewTests : AutoFixtureTestBase
    {
        public class Ctor : CustomViewTests
        {
            [Fact]
            public void From_interface()
            {
                var customView = AutoFixture.Create<ICustomView>();

                var result = new CustomView(customView);

                Assert.Equal(customView.Id, result.Id);
                Assert.Equal(customView.Name, result.Name);
                Assert.Equal(customView.Shared, result.Shared);
                Assert.Equal(customView.CreatedAt, result.CreatedAt);
                Assert.Equal(customView.UpdatedAt, result.UpdatedAt);
                Assert.Equal(customView.LastAccessedAt, result.LastAccessedAt);
                Assert.Same(customView.Workbook, result.Workbook);
                Assert.Same(customView.Owner, result.Owner);
                Assert.Equal(customView.BaseViewId, result.BaseViewId);
                Assert.Equal(customView.BaseViewName, result.BaseViewName);
            }

            [Fact]
            public void From_response()
            {
                var customViewType = AutoFixture.Create<ICustomViewType>();
                var workbook = AutoFixture.Create<IContentReference>();
                var owner = AutoFixture.Create<IContentReference>();

                var result = new CustomView(customViewType, workbook, owner);

                Assert.Equal(customViewType.Id, result.Id);
                Assert.Equal(customViewType.Name, result.Name);
                Assert.Equal(customViewType.Shared, result.Shared);
                Assert.Equal(customViewType.CreatedAt, result.CreatedAt);
                Assert.Equal(customViewType.UpdatedAt, result.UpdatedAt);
                Assert.Same(workbook, result.Workbook);
                Assert.Same(owner, result.Owner);
                Assert.Equal(customViewType.ViewId, result.BaseViewId);
                Assert.Equal(customViewType.ViewName, result.BaseViewName);
            }

            [Fact]
            public void Non_NullableTimestamps_correct_when_not_null_in_response()
            {
                var customView = AutoFixture.Create<ICustomView>();

                Assert.NotNull(customView.CreatedAt);
                Assert.NotNull(customView.UpdatedAt);

                var result = new CustomView(
                    customView.Id,
                    customView.Name,
                    customView.CreatedAt,
                    customView.UpdatedAt,
                    customView.LastAccessedAt,
                    customView.Shared,
                    customView.BaseViewId,
                    customView.BaseViewName,
                    customView.Workbook,
                    customView.Owner);

                Assert.Equal(customView.Id, result.Id);
                Assert.Equal(customView.Name, result.Name);
                Assert.Equal(customView.Shared, result.Shared);
                Assert.Equal(customView.CreatedAt, result.CreatedAt);
                Assert.Equal(customView.UpdatedAt, result.UpdatedAt);
                Assert.Equal(customView.LastAccessedAt, result.LastAccessedAt);
                Assert.Same(customView.Workbook, result.Workbook);
                Assert.Same(customView.Owner, result.Owner);
                Assert.Equal(customView.BaseViewId, result.BaseViewId);
                Assert.Equal(customView.BaseViewName, result.BaseViewName);
            }

            [Fact]
            public void Non_NullableTimestamps_empty_when_null_in_response()
            {
                var customView = AutoFixture.Create<ICustomView>();

                Assert.NotNull(customView.CreatedAt);
                Assert.NotNull(customView.UpdatedAt);

                var result = new CustomView(
                    customView.Id,
                    customView.Name,
                    null,
                    null,
                    customView.LastAccessedAt,
                    customView.Shared,
                    customView.BaseViewId,
                    customView.BaseViewName,
                    customView.Workbook,
                    customView.Owner);

                Assert.Equal(customView.Id, result.Id);
                Assert.Equal(customView.Name, result.Name);
                Assert.Equal(customView.Shared, result.Shared);
                Assert.Equal(string.Empty, result.CreatedAt);
                Assert.Equal(string.Empty, result.UpdatedAt);
                Assert.Equal(customView.LastAccessedAt, result.LastAccessedAt);
                Assert.Same(customView.Workbook, result.Workbook);
                Assert.Same(customView.Owner, result.Owner);
                Assert.Equal(customView.BaseViewId, result.BaseViewId);
                Assert.Equal(customView.BaseViewName, result.BaseViewName);
            }

            [Fact]
            public void NullableTimestamps_null_when_null_in_response()
            {
                var customView = AutoFixture.Create<ICustomView>();

                Assert.NotNull(customView.CreatedAt);
                Assert.NotNull(customView.UpdatedAt);

                var result = new CustomView(
                    customView.Id,
                    customView.Name,
                    customView.CreatedAt,
                    customView.UpdatedAt,
                    null,
                    customView.Shared,
                    customView.BaseViewId,
                    customView.BaseViewName,
                    customView.Workbook,
                    customView.Owner);

                Assert.Equal(customView.Id, result.Id);
                Assert.Equal(customView.Name, result.Name);
                Assert.Equal(customView.Shared, result.Shared);
                Assert.Equal(customView.CreatedAt, result.CreatedAt);
                Assert.Equal(customView.UpdatedAt, result.UpdatedAt);
                Assert.Null(result.LastAccessedAt);
                Assert.Same(customView.Workbook, result.Workbook);
                Assert.Same(customView.Owner, result.Owner);
                Assert.Equal(customView.BaseViewId, result.BaseViewId);
                Assert.Equal(customView.BaseViewName, result.BaseViewName);
            }
        }
    }
}
