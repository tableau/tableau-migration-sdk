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
using Xunit;

using FileOptions = Tableau.Migration.Config.FileOptions;

namespace Tableau.Migration.Tests.Unit.Config
{
    public class FileOptionsTests
    {
        public class Disabled
        {
            [Fact]
            public void DefaultsToFalse()
            {
                Assert.False(FileOptions.Defaults.DISABLE_FILE_ENCRYPTION);
            }

            [Fact]
            public void FallsBackToDefault()
            {
                var opts = new FileOptions();
                Assert.Equal(FileOptions.Defaults.DISABLE_FILE_ENCRYPTION, opts.DisableFileEncryption);
            }

            [Fact]
            public void CustomizedValue()
            {
                var opts = new FileOptions
                {
                    DisableFileEncryption = true
                };
                Assert.True(opts.DisableFileEncryption);
            }
        }

        public class RootPath
        {
            [Fact]
            public void DefaultsToTempDir()
            {
                Assert.Equal(Path.GetTempPath(), FileOptions.Defaults.ROOT_PATH);
            }

            [Fact]
            public void FallsBackToDefault()
            {
                var opts = new FileOptions();
                Assert.Equal(FileOptions.Defaults.ROOT_PATH, opts.RootPath);
            }

            [Fact]
            public void CustomizedValue()
            {
                const string testPath = @"\\test";
                var opts = new FileOptions
                {
                    RootPath = testPath
                };
                Assert.Equal(testPath, opts.RootPath);
            }
        }

    }
}
