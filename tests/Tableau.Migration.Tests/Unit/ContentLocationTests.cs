//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Collections.Immutable;
using System.Linq;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class ContentLocationTests
    {
        public class Path
        {
            [Theory]
            [InlineData(new[] { "workbook" }, "/", "workbook")]
            [InlineData(new[] { "project", "workbook" }, "/", "project/workbook")]
            [InlineData(new[] { "project", "child", "workbook" }, "/", "project/child/workbook")]
            [InlineData(new[] { "project", "", "workbook" }, "/", "project//workbook")]
            [InlineData(new[] { "domain", "username" }, "\\", "domain\\username")]
            public void BuildsPath(string[] segments, string separator, string expected)
            {
                var loc = new ContentLocation(segments.ToImmutableArray(), separator);
                Assert.Equal(expected, loc.Path);
            }
        }

        public class Name
        {
            [Theory]
            [InlineData(new string[0], "")]
            [InlineData(new[] { "workbook" }, "workbook")]
            [InlineData(new[] { "project", "workbook" }, "workbook")]
            [InlineData(new[] { "project", "child", "workbook" }, "workbook")]
            [InlineData(new[] { "project", "", "workbook" }, "workbook")]
            public void GetsFinalSegment(string[] segments, string expected)
            {
                var loc = new ContentLocation(segments.ToImmutableArray());
                Assert.Equal(expected, loc.Name);
            }
        }

        public class IsEmpty
        {
            [Fact]
            public void Empty()
            {
                var loc = new ContentLocation(ImmutableArray<string>.Empty);
                Assert.True(loc.IsEmpty);
            }

            [Fact]
            public void NotEmpty()
            {
                var loc = new ContentLocation("name");
                Assert.False(loc.IsEmpty);
            }
        }

        public class Ctor
        {
            [Fact]
            public void SegmentParams()
            {
                var loc = new ContentLocation("project", "workbook");
                Assert.Equal("project/workbook", loc.Path);
            }

            [Fact]
            public void SegmentEnumerable()
            {
                var loc = new ContentLocation(new[] { "project", "workbook" }.ToImmutableArray());
                Assert.Equal("project/workbook", loc.Path);
            }

            [Fact]
            public void NonDefaultSeparator()
            {
                var loc = new ContentLocation("|", new[] { "project", "workbook" });
                Assert.Equal("project|workbook", loc.Path);
            }

            [Fact]
            public void AppendWithParent()
            {
                var parent = new ContentLocation("|", new[] { "project", "child" });

                var loc = new ContentLocation(parent, "workbook");
                Assert.Equal("project|child|workbook", loc.Path);
            }
        }

        public class Equality : AutoFixtureTestBase
        {
            [Fact]
            public void LogicallyEqual()
            {
                var path = Create<string>();

                var a = new ContentLocation(path);
                var b = new ContentLocation(path);

                Assert.True(a.Equals(b));
            }

            [Fact]
            public void DifferentPath()
            {
                var a = new ContentLocation(CreateMany<string>());
                var b = new ContentLocation(CreateMany<string>());

                Assert.False(a.Equals(b));
            }
        }

        public class GetHashCodeTests : AutoFixtureTestBase
        {
            [Fact]
            public void UsesPathHashCode()
            {
                var loc = new ContentLocation(CreateMany<string>());
                Assert.Equal(loc.Path.GetHashCode(StringComparison.OrdinalIgnoreCase), loc.GetHashCode());
            }
        }

        public class ToStringTests : AutoFixtureTestBase
        {
            [Fact]
            public void UsesPath()
            {
                var loc = new ContentLocation(CreateMany<string>());
                Assert.Equal(loc.Path, loc.ToString());
            }
        }

        public class ForUsername : AutoFixtureTestBase
        {
            [Fact]
            public void UsesDomainSeparator()
            {
                var domain = Create<string>();
                var username = Create<string>();
                var loc = ContentLocation.ForUsername(domain, username);

                Assert.Equal($"{domain}{Constants.DomainNameSeparator}{username}", loc.Path);
            }
        }

        public class CompareTo
        {
            [Fact]
            public void OrdersByPathIgnoringCase()
            {
                var sorted = new[]
                {
                    new ContentLocation("Project", "Child", "subWorkbookA"),
                    new ContentLocation("Project", "child", "subWorkbookB"),
                    new ContentLocation("project", "workbookA"),
                    new ContentLocation("Project", "WorkbookB")
                }.ToImmutableList();

                var result = sorted.Reverse().Sort();

                Assert.Equal(sorted, result);
            }
        }

        public class Append
        {
            [Theory]
            [InlineData(new string[0], "name", "name")]
            [InlineData(new[] { "project" }, "workbook", "project/workbook")]
            [InlineData(new[] { "project", "child" }, "workbook", "project/child/workbook")]
            [InlineData(new[] { "project", "" }, "workbook", "project//workbook")]
            public void AppendsSegment(string[] startSegments, string append, string expectedPath)
            {
                var loc = new ContentLocation(startSegments.ToImmutableArray());
                Assert.Equal(expectedPath, loc.Append(append).Path);
            }
        }

        public class Parent
        {
            [Theory]
            [InlineData(new string[0], "")]
            [InlineData(new[] { "project" }, "")]
            [InlineData(new[] { "project", "workbook" }, "project")]
            [InlineData(new[] { "project", "child", "workbook" }, "project/child")]
            public void GetsParentPath(string[] segments, string expectedPath)
            {
                var loc = new ContentLocation(segments.ToImmutableArray());
                Assert.Equal(expectedPath, loc.Parent().Path);
            }
        }

        public class Rename
        {
            [Theory]
            [InlineData(new string[0], "rename")]
            [InlineData(new[] { "project" }, "rename")]
            [InlineData(new[] { "project", "workbook" }, "rename")]
            [InlineData(new[] { "project", "child", "workbook" }, "rename")]
            public void RenamesLastSegment(string[] segments, string rename)
            {
                var loc = new ContentLocation(segments.ToImmutableArray());
                var renamed = loc.Rename(rename);

                Assert.Equal(new ContentLocation(loc.Parent(), rename).Path, renamed.Path);
            }
        }

        public class FromPath
        {
            [Theory]
            [InlineData("workbook", "/", new[] { "workbook" })]
            [InlineData("project/workbook", "/", new[] { "project", "workbook" })]
            [InlineData("project/child/workbook", "/", new[] { "project", "child", "workbook" })]
            [InlineData("domain\\username", "\\", new[] { "domain", "username" })]
            public void ParsesSimplePaths(string path, string separator, string[] expectedSegments)
            {
                var loc = ContentLocation.FromPath(path, separator);
                Assert.Equal(expectedSegments, loc.PathSegments);
                Assert.Equal(separator, loc.PathSeparator);
            }

            [Theory]
            [InlineData("Test 4\\/25", "/", new[] { "Test 4/25" })]
            [InlineData("project/Test 4\\/25", "/", new[] { "project", "Test 4/25" })]
            [InlineData("Test 4\\/25/workbook", "/", new[] { "Test 4/25", "workbook" })]
            [InlineData("domain\\\\with\\\\backslash\\username", "\\", new[] { "domain\\with\\backslash", "username" })]
            public void ParsesEscapedPaths(string path, string separator, string[] expectedSegments)
            {
                var loc = ContentLocation.FromPath(path, separator);
                Assert.Equal(expectedSegments, loc.PathSegments);
                Assert.Equal(separator, loc.PathSeparator);
            }

            [Theory]
            [InlineData("Test 4\\/25/Another\\/Project", "/", new[] { "Test 4/25", "Another/Project" })]
            [InlineData("domain\\\\with\\\\slash\\user\\\\name", "\\", new[] { "domain\\with\\slash", "user\\name" })]
            public void ParsesMultipleEscapedSegments(string path, string separator, string[] expectedSegments)
            {
                var loc = ContentLocation.FromPath(path, separator);
                Assert.Equal(expectedSegments, loc.PathSegments);
                Assert.Equal(separator, loc.PathSeparator);
            }

            [Fact]
            public void HandlesEmptyPath()
            {
                var loc = ContentLocation.FromPath("", "/");
                Assert.True(loc.PathSegments.IsEmpty);
                Assert.Equal("/", loc.PathSeparator);
            }

            [Fact]
            public void HandlesNullPath()
            {
                var loc = ContentLocation.FromPath(null!, "/");
                Assert.True(loc.PathSegments.IsEmpty);
                Assert.Equal("/", loc.PathSeparator);
            }

            [Theory]
            [InlineData("custom|separator", "|", new[] { "custom", "separator" })]
            [InlineData("no|escaping|for|custom", "|", new[] { "no", "escaping", "for", "custom" })]
            public void HandlesCustomSeparators(string path, string separator, string[] expectedSegments)
            {
                var loc = ContentLocation.FromPath(path, separator);
                Assert.Equal(expectedSegments, loc.PathSegments);
                Assert.Equal(separator, loc.PathSeparator);
            }
        }

        public class PathEscaping
        {
            [Theory]
            [InlineData(new[] { "Test 4/25" }, "/", "Test 4\\/25")]
            [InlineData(new[] { "Test 4\\/25" }, "/", @"Test 4\\\/25")]
            [InlineData(new[] { "project", "Test 4/25" }, "/", "project/Test 4\\/25")]
            [InlineData(new[] { "Test 4/25", "workbook" }, "/", "Test 4\\/25/workbook")]
            [InlineData(new[] { "domain\\with\\backslash", "username" }, "\\", "domain\\\\with\\\\backslash\\username")]
            public void EscapesSeparatorsInSegments(string[] segments, string separator, string expectedPath)
            {
                var loc = new ContentLocation(segments.ToImmutableArray(), separator);
                Assert.Equal(expectedPath, loc.Path);
            }

            [Theory]
            [InlineData(new[] { "Test 4/25", "Another/Project" }, "/", "Test 4\\/25/Another\\/Project")]
            [InlineData(new[] { "domain\\with\\slash", "user\\name" }, "\\", "domain\\\\with\\\\slash\\user\\\\name")]
            public void EscapesMultipleSegments(string[] segments, string separator, string expectedPath)
            {
                var loc = new ContentLocation(segments.ToImmutableArray(), separator);
                Assert.Equal(expectedPath, loc.Path);
            }

            [Theory]
            [InlineData(new[] { "normal", "segments" }, "/", "normal/segments")]
            [InlineData(new[] { "domain", "username" }, "\\", "domain\\username")]
            public void DoesNotEscapeNormalSegments(string[] segments, string separator, string expectedPath)
            {
                var loc = new ContentLocation(segments.ToImmutableArray(), separator);
                Assert.Equal(expectedPath, loc.Path);
            }

            [Theory]
            [InlineData(new[] { "custom|separator" }, "|", "custom|separator")]
            public void DoesNotEscapeCustomSeparators(string[] segments, string separator, string expectedPath)
            {
                var loc = new ContentLocation(segments.ToImmutableArray(), separator);
                Assert.Equal(expectedPath, loc.Path);
            }
        }

        public class RoundTripTests
        {
            [Theory]
            [InlineData(new[] { "Test 4/25" }, "/")]
            [InlineData(new[] { "Test 4\\/25" }, "/")]
            [InlineData(new[] { "project", "Test 4/25" }, "/")]
            [InlineData(new[] { "Test 4/25", "Another/Project", "workbook" }, "/")]
            [InlineData(new[] { "domain\\with\\backslash", "username" }, "\\")]
            [InlineData(new[] { "domain\\with\\slash", "user\\name" }, "\\")]
            public void PathsRoundTripProperly(string[] originalSegments, string separator)
            {
                // Create location from segments
                var originalLoc = new ContentLocation(originalSegments.ToImmutableArray(), separator);
                
                // Convert to path string
                var pathString = originalLoc.Path;
                
                // Parse back from path string
                var parsedLoc = ContentLocation.FromPath(pathString, separator);
                
                // Verify segments match
                Assert.Equal(originalSegments, parsedLoc.PathSegments);
                Assert.Equal(separator, parsedLoc.PathSeparator);
                
                // Verify paths match
                Assert.Equal(pathString, parsedLoc.Path);
            }

            [Theory]
            [InlineData("Test 4\\/25", "/", new[] { "Test 4/25" })]
            [InlineData("project/Test 4\\/25", "/", new[] { "project", "Test 4/25" })]
            [InlineData("domain\\\\with\\\\backslash\\username", "\\", new[] { "domain\\with\\backslash", "username" })]
            public void EscapedPathsRoundTripProperly(string escapedPath, string separator, string[] expectedSegments)
            {
                // Parse from escaped path
                var parsedLoc = ContentLocation.FromPath(escapedPath, separator);
                
                // Verify segments
                Assert.Equal(expectedSegments, parsedLoc.PathSegments);
                
                // Convert back to path
                var regeneratedPath = parsedLoc.Path;
                
                // Verify path matches original
                Assert.Equal(escapedPath, regeneratedPath);
                
                // Parse again to ensure consistency
                var reparsedLoc = ContentLocation.FromPath(regeneratedPath, separator);
                Assert.Equal(expectedSegments, reparsedLoc.PathSegments);
            }
        }

        public class ForContentType : AutoFixtureTestBase
        {
            [Theory]
            [InlineData(typeof(IUser), Constants.DomainNameSeparator)]
            [InlineData(typeof(IGroup), Constants.DomainNameSeparator)]
            [InlineData(typeof(IProject), Constants.PathSeparator)]
            public void DetectsSeparatorByContentType(Type contentType, string expectedSeparator)
            {
                var segments = CreateMany<string>();

                var loc = ContentLocation.ForContentType(contentType, segments);

                Assert.Equal(expectedSeparator, loc.PathSeparator);
                Assert.Equal(segments, loc.PathSegments);
            }

            [Theory]
            [InlineData(typeof(IUser), Constants.DomainNameSeparator)]
            [InlineData(typeof(IGroup), Constants.DomainNameSeparator)]
            [InlineData(typeof(IProject), Constants.PathSeparator)]
            public void DetectsSeparatorByContentTypeParams(Type contentType, string expectedSeparator)
            {
                var segments = CreateMany<string>().ToArray();

                var loc = ContentLocation.ForContentType(contentType, segments);

                Assert.Equal(expectedSeparator, loc.PathSeparator);
                Assert.Equal(segments, loc.PathSegments);
            }

            [Fact]
            public void DetectsSeparatorByContentTypeGeneric()
            {
                var segments = CreateMany<string>();

                var loc = ContentLocation.ForContentType<IUser>(segments);

                Assert.Equal(Constants.DomainNameSeparator, loc.PathSeparator);
                Assert.Equal(segments, loc.PathSegments);

                loc = ContentLocation.ForContentType<IGroup>(segments);

                Assert.Equal(Constants.DomainNameSeparator, loc.PathSeparator);
                Assert.Equal(segments, loc.PathSegments);

                loc = ContentLocation.ForContentType<IProject>(segments);

                Assert.Equal(Constants.PathSeparator, loc.PathSeparator);
                Assert.Equal(segments, loc.PathSegments);
            }

            [Fact]
            public void DetectsSeparatorByContentTypeGenericParams()
            {
                var segments = CreateMany<string>().ToArray();

                var loc = ContentLocation.ForContentType<IUser>(segments);

                Assert.Equal(Constants.DomainNameSeparator, loc.PathSeparator);
                Assert.Equal(segments, loc.PathSegments);

                loc = ContentLocation.ForContentType<IGroup>(segments);

                Assert.Equal(Constants.DomainNameSeparator, loc.PathSeparator);
                Assert.Equal(segments, loc.PathSegments);

                loc = ContentLocation.ForContentType<IProject>(segments);

                Assert.Equal(Constants.PathSeparator, loc.PathSeparator);
                Assert.Equal(segments, loc.PathSegments);
            }
        }
    }
}
