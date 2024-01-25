using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    /// <summary>
    /// Simulation tests for error handling for various components.
    /// </summary>
    public class ErrorHandlingTests
    {
        #region - Network Layer -

        public class NetworkLayerErrorHandling : AutoFixtureTestBase
        {
            private class NetworkExceptionMessageHandler : HttpMessageHandler
            {
                protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                    => throw new HttpRequestException("You shall not pass!");
            }

            [Fact]
            public async Task DoesNotCatchExceptionsAsync()
            {
                using var innerClient = new HttpClient(new NetworkExceptionMessageHandler());
                var client = new DefaultHttpClient(innerClient, Create<IHttpContentSerializer>());

                await Assert.ThrowsAsync<HttpRequestException>(() => client.SendAsync(Create<HttpRequestMessage>(), default));
            }
        }

        #endregion

        #region - API Layer -

        #endregion

        #region - Engine - Entry-level -

        public class EngineEntryLevelErrorHandling
        { }

        #endregion

        #region - Engine - Batch-level -

        public class EngineBatchLevelErrorHandling
        { }

        #endregion

        #region - Engine - Action-level -

        public class EngineActionLevelErrorHandling
        { }

        #endregion

        #region - Engine - Pipeline-level -

        public class EnginePipelineLevelErrorHandling
        { }

        #endregion

        #region - Engine - Migration-level -

        public class EngineMigrationErrorHandling
        { }

        #endregion
    }
}
