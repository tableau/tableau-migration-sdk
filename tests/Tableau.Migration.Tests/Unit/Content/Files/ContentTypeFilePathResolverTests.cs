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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Files
{
    public class ContentTypeFilePathResolverTests
    {
        public class ResolveRelativePath : AutoFixtureTestBase
        {
            [Theory]
            [InlineData(typeof(IDataSource), ContentTypeDirectoryNames.DataSources, "orig.tdsx", ContentTypeFilePrefixes.DataSource, "tdsx")]
            [InlineData(typeof(IDataSource), ContentTypeDirectoryNames.DataSources, "orig.tds", ContentTypeFilePrefixes.DataSource, "tds")]
            [InlineData(typeof(IWorkbook), ContentTypeDirectoryNames.Workbooks, "orig.twbx", ContentTypeFilePrefixes.Workbook, "twbx")]
            [InlineData(typeof(IWorkbook), ContentTypeDirectoryNames.Workbooks, "orig.twb", ContentTypeFilePrefixes.Workbook, "twb")]
            [InlineData(typeof(IFlow), ContentTypeDirectoryNames.Flows, "orig.tflx", ContentTypeFilePrefixes.Flow, "tflx")]
            [InlineData(typeof(IFlow), ContentTypeDirectoryNames.Flows, "orig.tfl", ContentTypeFilePrefixes.Flow, "tfl")]
            public void SupportedContentTypesGenerateExpectedPath(Type t, string dir, string originalFileName,
                string expectedPrefix, string expectedExtension)
            {
                var item = (IContentReference)Create(t);

                var r = new ContentTypeFilePathResolver();

                var result = r.ResolveRelativePath(item, originalFileName);

                Assert.Equal(Path.Combine(dir, $"{expectedPrefix}-{item.Id:N}.{expectedExtension}"), result);
            }

            private static bool IsFileContentInterface(Type t, object? criteria)
                => t == typeof(IFileContent);

            [Fact]
            public void AllFileContentTypesSupported()
            {
                var fileContentTypes = typeof(IFileContent).Assembly
                    .GetTypes()
                    .Where(t => t.FindInterfaces(IsFileContentInterface, null).Any());

                var r = new ContentTypeFilePathResolver();

                var unsupportedTypes = new List<Type>();
                foreach (var fileContentType in fileContentTypes)
                {
                    try
                    {
                        var item = Create(fileContentType);
                        r.ResolveRelativePath(item, Create<string>());
                    }
                    catch (ArgumentException)
                    {
                        unsupportedTypes.Add(fileContentType);
                    }
                }

                if (unsupportedTypes.Any())
                {
                    Assert.Fail($"One or content types implements {nameof(IFileContent)} but is not supported by {nameof(ContentTypeFilePathResolver)}: {string.Join(", ", unsupportedTypes)}.");
                }
            }
        }
    }
}
