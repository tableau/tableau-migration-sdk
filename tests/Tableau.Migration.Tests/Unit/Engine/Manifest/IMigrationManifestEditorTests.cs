using System;
using Moq;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Manifest
{
    public class IMigrationManifestEditorTests
    {
        public class AddErrors
        {
            [Fact]
            public void AddsAllErrorsFromResult()
            {
                var result = Result.Failed(new[] { new Exception(), new Exception(), new Exception() });

                var mockEditor = new Mock<IMigrationManifestEditor> { CallBase = true };
                mockEditor.Object.AddErrors(result);

                mockEditor.Verify(x => x.AddErrors(result.Errors), Times.Once);
            }

            [Fact]
            public void AddsAllErrorsFromMultipleResult()
            {
                var result1 = Result.Failed(new[] { new Exception(), new Exception(), new Exception() });
                var result2 = Result.Failed(new[] { new Exception(), new Exception(), new Exception() });

                var mockEditor = new Mock<IMigrationManifestEditor> { CallBase = true };
                mockEditor.Object.AddErrors(new[] { result1, result2 });

                mockEditor.Verify(x => x.AddErrors(It.IsAny<IResult>()), Times.Exactly(2));
                mockEditor.Verify(x => x.AddErrors(result1.Errors), Times.Once);
                mockEditor.Verify(x => x.AddErrors(result2.Errors), Times.Once);
            }
        }
    }
}
