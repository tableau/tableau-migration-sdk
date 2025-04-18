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

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Tests.Unit.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class IContentFileStoreExtensionsTests
    {
        public class CreateAsync : AutoFixtureTestBase
        {
            [Fact]
            public async Task CreatesFromFileDownloadAsync()
            {
                var cancel = new CancellationToken();
                var fs = new MemoryContentFileStore();

                var fileText = "text";
                await using (var fileDownload = new FileDownload("fileName", new MemoryStream(Constants.DefaultEncoding.GetBytes(fileText)), true))
                {
                    var file = await fs.CreateAsync(new object(), fileDownload, cancel);

                    Assert.Equal(fileDownload.Filename, file.OriginalFileName);
                    Assert.Equal(fileDownload.IsZipFile, file.IsZipFile);

                    await using (var readStream = await file.OpenReadAsync(cancel))
                    using (var reader = new StreamReader(readStream.Content, Constants.DefaultEncoding))
                    {
                        Assert.Equal(fileText, reader.ReadToEnd());
                    }
                }
            }
        }
    }
}
