using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Tableau.Migration.Config;
using Tableau.Migration.Net.Policies;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Policies
{
    public class RetryPolicyBuilderTests
    {
        private readonly Mock<IConfigReader> _mockedConfigReader;
        private readonly MigrationSdkOptions _sdkOptions;
        private readonly RetryPolicyBuilder _builder;

        #region GetRetryCasesForResponseStatusCodes

        public static IEnumerable<object[]> GetRetryCasesForResponseStatusCodes()
        {
            yield return new object[]
            {
                // returnedStatusCode
                HttpStatusCode.RequestTimeout
            };

            // Test default configuration for response status codes 5XX
            for (var statusCode = (int)HttpStatusCode.InternalServerError; statusCode <= (int)HttpStatusCode.NetworkAuthenticationRequired; statusCode++)
            {
                yield return new object[]
                {
                    // returnedStatusCode
                    (HttpStatusCode)statusCode
                };
            }

            // OK Status
            yield return new object[]
            {
                // returnedStatusCode
                HttpStatusCode.OK,
                // expectRetry
                false
            };

            // OK Status and with Override Status Code configuration
            yield return new object[]
            {
                // returnedStatusCode
                HttpStatusCode.OK,
                // expectRetry
                false,
                // overrideStatusCodes
                new int[]
                {
                    (int)HttpStatusCode.ServiceUnavailable
                }
            };

            // With Override Status Code configuration
            yield return new object[]
            {
                // returnedStatusCode
                HttpStatusCode.Conflict,
                // expectRetry
                true,
                // overrideStatusCodes
                new int[]
                {
                    (int)HttpStatusCode.Conflict
                }
            };

            // Too Many Requests not retried
            yield return new object[]
            {
                // returnedStatusCode
                HttpStatusCode.TooManyRequests,
                // expectRetry
                false
            };
        }

        #endregion GetRetryCasesForResponseStatusCodes

        #region GetRetryCasesForExceptions

        public static IEnumerable<object[]> GetRetryCasesForExceptions()
        {
            yield return new object[]
            {
                // exceptionType
                typeof(HttpRequestException),
                // expectedException
                new HttpRequestException()
            };

            yield return new object[]
            {
                // exceptionType
                typeof(TimeoutRejectedException),
                // expectedException
                new TimeoutRejectedException()
            };

            yield return new object[]
            {
                // exceptionType
                typeof(Exception),
                // expectedException
                new Exception(),
                // expectRetry
                false
            };

            yield return new object[]
            {
                // exceptionType
                typeof(TaskCanceledException),
                // expectedException
                new TaskCanceledException(),
                // expectRetry
                false
            };
        }

        #endregion GetRetryCasesForExceptions

        public RetryPolicyBuilderTests()
        {
            _mockedConfigReader = new Mock<IConfigReader>();
            _sdkOptions = new MigrationSdkOptions();
            _mockedConfigReader
                .Setup(x => x.Get())
                .Returns(_sdkOptions);
            _builder = new RetryPolicyBuilder(
                _mockedConfigReader.Object);
        }

        [Fact]
        public void BuildPolicy_ReturnsDefaultPolicy()
        {
            // Act
            var policy = _builder.Build(new HttpRequestMessage());

            // Assert
            Assert.NotNull(policy);
            Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(policy);
            var retryPolicy = (AsyncRetryPolicy<HttpResponseMessage>)policy;

            var retriesObject = retryPolicy.GetFieldValue("_sleepDurationsEnumerable");
            Assert.NotNull(retriesObject);
            Assert.IsAssignableFrom<IEnumerable<TimeSpan>>(retriesObject);
            Assert.Equal(
                (IEnumerable<TimeSpan>)retriesObject,
                _sdkOptions.Network.Resilience.RetryIntervals);
        }

        [Fact]
        public void BuildPolicy_DisableRetry_ReturnsNull()
        {
            // Arrange
            _sdkOptions.Network.Resilience.RetryEnabled = false;

            // Act
            var policy = _builder.Build(new HttpRequestMessage());

            // Assert
            Assert.Null(policy);
        }

        [Fact]
        public void BuildPolicy_EmptyRetryIntervals_ReturnsNull()
        {
            // Arrange
            _sdkOptions.Network.Resilience.RetryEnabled = true;
            _sdkOptions.Network.Resilience.RetryIntervals = Array.Empty<TimeSpan>();

            // Act
            var policy = _builder.Build(new HttpRequestMessage());

            // Assert
            Assert.Null(policy);
        }

        [Theory]
        [MemberData(nameof(GetRetryCasesForResponseStatusCodes))]
        public async Task ExecuteBuiltPolicy_TestRetryCasesForResponseStatusCodes(
            HttpStatusCode returnedStatusCode,
            bool expectRetry = true,
            int[]? overrideStatusCodes = null)
        {
            // Arrange
            if (overrideStatusCodes is not null)
            {
                _sdkOptions.Network.Resilience.RetryOverrideResponseCodes = overrideStatusCodes;
            }
            _sdkOptions.Network.Resilience.RetryIntervals = new TimeSpan[]
            {
                TimeSpan.FromMilliseconds(5)
            };
            var expectedTries = expectRetry ? 2 : 1;
            var retryKey = "retryCount";
            var context = new Context
            {
                { retryKey, 0 }
            };
            var policy = _builder.Build(new HttpRequestMessage());
            var retryPolicy = (AsyncRetryPolicy<HttpResponseMessage>)policy!;

            // Act
            var response = await retryPolicy.ExecuteAsync(
                (ctx) =>
                {
                    var tries = (int)ctx[retryKey];
                    ctx[retryKey] = ++tries;
                    return Task.FromResult(
                        new HttpResponseMessage(
                            returnedStatusCode));
                },
                context);

            // Assert
            Assert.Equal(
                expectedTries,
                (int)context[retryKey]);

            Assert.Equal(
                returnedStatusCode,
                response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(GetRetryCasesForExceptions))]
        public async Task ExecuteBuiltPolicy_TestRetryCasesForExceptions(
            Type exceptionType,
            Exception expectedException,
            bool expectRetry = true)
        {
            // Arrange
            _sdkOptions.Network.Resilience.RetryIntervals = new TimeSpan[]
            {
                TimeSpan.FromMilliseconds(5)
            };
            var expectedTries = expectRetry ? 2 : 1;
            var retryKey = "retryCount";
            var context = new Context
            {
                { retryKey, 0 }
            };
            var policy = _builder.Build(new HttpRequestMessage());
            var retryPolicy = (AsyncRetryPolicy<HttpResponseMessage>)policy!;

            // Act
            _ = await Assert.ThrowsAsync(
                exceptionType,
                async () =>
                    await retryPolicy.ExecuteAsync(
                    (ctx) =>
                    {
                        var tries = (int)ctx[retryKey];
                        ctx[retryKey] = ++tries;
                        throw expectedException;
                    },
                    context));

            // Assert
            Assert.Equal(
                expectedTries,
                (int)context[retryKey]);
        }
    }
}
