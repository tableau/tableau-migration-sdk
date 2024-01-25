using System;
using System.Collections.Immutable;
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
    }
}
