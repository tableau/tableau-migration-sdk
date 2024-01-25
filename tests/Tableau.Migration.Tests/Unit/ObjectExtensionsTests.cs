using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class ObjectExtensionsTests
    {
        #region - DisposeOnThrowAsync -

        public class DisposeOnThrowAsync
        {
            [Fact]
            public async Task ReturnsAsync()
            {
                var mockResource = new Mock<IAsyncDisposable>();

                var result = await mockResource.Object.DisposeOnThrowAsync(
                    async () => await Task.FromResult(5));

                Assert.Equal(5, result);

                mockResource.Verify(x => x.DisposeAsync(), Times.Never);
            }

            [Fact]
            public async Task DisposesOnThrowAsync()
            {
                var mockResource = new Mock<IAsyncDisposable>();

                var ex = new Exception();
                var thrown = await Assert.ThrowsAsync<Exception>(async () => await mockResource.Object.DisposeOnThrowAsync<int>(() => throw ex));

                Assert.Same(ex, thrown);

                mockResource.Verify(x => x.DisposeAsync(), Times.Once);
            }
        }

        #endregion

        #region - DisposeOnThrowOrFailureAsync -

        public class DisposeOnThrowOrFailureAsync
        {
            [Fact]
            public async Task ReturnsSuccessAsync()
            {
                var mockResource = new Mock<IAsyncDisposable>();
                var innerResult = Result.Succeeded();

                var result = await mockResource.Object.DisposeOnThrowOrFailureAsync(
                    async () => await Task.FromResult(innerResult));

                Assert.Same(innerResult, result);

                mockResource.Verify(x => x.DisposeAsync(), Times.Never);
            }

            [Fact]
            public async Task DisposesOnFailureAsync()
            {
                var mockResource = new Mock<IAsyncDisposable>();
                var innerResult = Result.Failed(new Exception());

                var result = await mockResource.Object.DisposeOnThrowOrFailureAsync(
                    async () => await Task.FromResult(innerResult));

                Assert.Same(innerResult, result);

                mockResource.Verify(x => x.DisposeAsync(), Times.Once);
            }

            [Fact]
            public async Task DisposesOnThrowAsync()
            {
                var mockResource = new Mock<IAsyncDisposable>();

                var ex = new Exception();
                var thrown = await Assert.ThrowsAsync<Exception>(async () => await mockResource.Object.DisposeOnThrowOrFailureAsync<Result>(() => throw ex));

                Assert.Same(ex, thrown);

                mockResource.Verify(x => x.DisposeAsync(), Times.Once);
            }
        }

        #endregion

        #region - DisposeIfNeededAsync -

        public class DisposeIfNeededAsync
        {
            [Fact]
            public async Task AsynchronousDisposalAsync()
            {
                var mock = new Mock<IAsyncDisposable>();

                await mock.Object.DisposeIfNeededAsync();

                mock.Verify(x => x.DisposeAsync(), Times.Once);
            }

            [Fact]
            public async Task SynchronousDisposalAsync()
            {
                var mock = new Mock<IDisposable>();

                await mock.Object.DisposeIfNeededAsync();

                mock.Verify(x => x.Dispose(), Times.Once);
            }

            public class NonDisposable
            {
                public virtual void Dispose() { }
            }

            [Fact]
            public async Task NotDisposableAsync()
            {
                var mock = new Mock<NonDisposable>();

                await mock.Object.DisposeIfNeededAsync();

                mock.Verify(x => x.Dispose(), Times.Never);
            }
        }

        #endregion
    }
}
