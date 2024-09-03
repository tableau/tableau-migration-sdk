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

using System.IO;
using System.Linq;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class PublishWorkbookOptionsTests : AutoFixtureTestBase
    {
        private static void AssertContentTypeFields(IPublishableWorkbook workbook, PublishWorkbookOptions result)
        {
            Assert.Equal(workbook.Name, result.Name);
            Assert.Equal(workbook.Description, result.Description);
            Assert.Equal(workbook.ShowTabs, result.ShowTabs);
            Assert.Equal(workbook.EncryptExtracts, result.EncryptExtracts);
            Assert.Equal(workbook.ThumbnailsUserId, result.ThumbnailsUserId);
            Assert.Equal(((IContainerContent)workbook).Container.Id, result.ProjectId);
            Assert.Equal(workbook.HiddenViewNames.ToList(), result.HiddenViewNames.ToList());
        }

        [Fact]
        public void Default()
        {
            var workbook = Create<IPublishableWorkbook>();
            var testFile = Create<Stream>();
            var result = new PublishWorkbookOptions(workbook, testFile);

            Assert.NotNull(result);
            AssertContentTypeFields(workbook, result);
            Assert.Equal(testFile, result.File);
            Assert.Equal(workbook.File.OriginalFileName, result.FileName);
            Assert.Equal(WorkbookFileTypes.Twbx, result.FileType);
        }

        [Fact]
        public void Different_FileType()
        {
            var workbook = Create<IPublishableWorkbook>();
            var testFile = Create<Stream>();
            var testFileType = Create<string>();
            var result = new PublishWorkbookOptions(workbook, testFile, testFileType);

            Assert.NotNull(result);
            AssertContentTypeFields(workbook, result);
            Assert.Equal(testFile, result.File);
            Assert.Equal(workbook.File.OriginalFileName, result.FileName);
            Assert.Equal(testFileType, result.FileType);
        }
    }
}
