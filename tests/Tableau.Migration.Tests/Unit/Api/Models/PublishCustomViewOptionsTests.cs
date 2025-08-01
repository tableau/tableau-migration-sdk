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

using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class PublishCustomViewOptionsTests : AutoFixtureTestBase
    {
        private static void AssertContentTypeFields(IPublishableCustomView customView, PublishCustomViewOptions result)
        {
            Assert.Same(customView.File, result.File);
            Assert.Equal(customView.Name, result.Name);
            Assert.Equal(customView.Workbook.Id, result.WorkbookId);
            Assert.Equal(customView.Owner.Id, result.OwnerId);
            Assert.Equal(customView.Shared, result.Shared);
        }

        [Fact]
        public void Default()
        {
            var customView = Create<IPublishableCustomView>();
            var result = new PublishCustomViewOptions(customView);

            Assert.NotNull(result);
            AssertContentTypeFields(customView, result);
            Assert.Equal(customView.File.OriginalFileName, result.FileName);
            Assert.Equal(CustomViewFileTypes.Json, result.FileType);
        }

        [Fact]
        public void Different_FileType()
        {
            var customView = Create<IPublishableCustomView>();
            var testFileType = Create<string>();
            var result = new PublishCustomViewOptions(customView, testFileType);

            Assert.NotNull(result);
            AssertContentTypeFields(customView, result);
            Assert.Equal(customView.File.OriginalFileName, result.FileName);
            Assert.Equal(testFileType, result.FileType);
        }
    }
}
