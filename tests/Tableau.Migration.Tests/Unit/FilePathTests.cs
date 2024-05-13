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

using System.Collections.Immutable;
using System.IO;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class FilePathTests
    {
        public abstract class FilePathTest : AutoFixtureTestBase
        {
            protected string CreateFilePath(string? extension)
                => Path.Combine(
                    Path.GetTempPath(), 
                    Create<string>(), 
                    CreateFileName(extension));

            protected string CreateFileName(string? extension)
                => $"{Create<string>()}{(!string.IsNullOrWhiteSpace(extension) ? $".{extension}" : string.Empty)}";

            public class ZipExtensionsData : ValuesAttribute<string>
            {
                private static readonly IImmutableSet<string> _zipExtensions;

                static ZipExtensionsData()
                {
                    var zipExtensions = ImmutableSortedSet.CreateBuilder<string>();

                    foreach (var zipExtension in FilePath.ZipExtensions)
                        zipExtensions.AddRange(new[] { zipExtension.ToLower(), zipExtension.ToUpper() });

                    _zipExtensions = zipExtensions.ToImmutable();
                }

                public ZipExtensionsData()
                    : base(_zipExtensions)
                { }
            }
        }

        public class IsZipFile : FilePathTest
        {
            [Theory]
            [ZipExtensionsData]
            public void True(string extension)
            {
                AssertIsZipFile(extension, true);
            }

            [Theory]
            [Values("abc", "123", "twb", "tds")]
            public void False(string extension)
            {
                AssertIsZipFile(extension, false);
            }

            [Theory]
            [NullEmptyWhiteSpaceData]
            public void Null(string? extension)
            {
                AssertIsZipFile(extension, null);
            }

            private void AssertIsZipFile(string? extension, bool? expected)
            {
                var filePath = CreateFilePath(extension);

                var result = new FilePath(filePath);

                Assert.Equal(expected, result.IsZipFile);
            }
        }
    }
}
