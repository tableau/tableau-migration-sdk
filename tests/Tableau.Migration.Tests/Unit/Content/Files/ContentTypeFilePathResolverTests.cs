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
            [InlineData(typeof(IDataSource), "data-sources", "orig.tdsx", "data-source-", "tdsx")]
            [InlineData(typeof(IDataSource), "data-sources", "orig.tds", "data-source-", "tds")]
            [InlineData(typeof(IWorkbook), "workbooks", "orig.twbx", "workbook-", "twbx")]
            [InlineData(typeof(IWorkbook), "workbooks", "orig.twb", "workbook-", "twb")]
            public void SupportedContentTypesGenerateExpectedPath(Type t, string dir, string originalFileName,
                string expectedPrefix, string expectedExtension)
            {
                var item = (IContentReference)Create(t);

                var r = new ContentTypeFilePathResolver();

                var result = r.ResolveRelativePath(item, originalFileName);

                Assert.Equal(Path.Combine(dir, $"{expectedPrefix}{item.Id:N}.{expectedExtension}"), result);
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
