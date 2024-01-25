using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tableau.Migration.Tests
{
    public static class MockExtensions
    {
        /// <summary>
        /// Sets up a fluent object with one or more calls that return the original instance.
        /// </summary>
        /// <typeparam name="T">The fluent object.</typeparam>
        /// <param name="mock">The mocked fluent object.</param>
        /// <param name="expressions">The expressions to set up.</param>
        /// <returns>The <see cref="Mock{T}"/> instance.</returns>
        public static Mock<T> SetupFluent<T>(this Mock<T> mock, params Expression<Func<T, T>>[] expressions)
            where T : class
        {
            if (!expressions.IsNullOrEmpty())
            {
                foreach (var expression in expressions)
                {
                    mock.Setup(expression).Returns(mock.Object);
                }
            }

            return mock;
        }

        public static void VerifyErrors<T>(this Mock<T> mock, Func<Times> times)
            where T : class, ILogger
            => VerifyLogging(mock, LogLevel.Error, times);

        public static void VerifyErrors<T>(this Mock<T> mock, Times times)
            where T : class, ILogger
            => VerifyLogging(mock, LogLevel.Error, times);

        public static void VerifyWarnings<T>(this Mock<T> mock, Func<Times> times)
            where T : class, ILogger
            => VerifyLogging(mock, LogLevel.Warning, times);

        public static void VerifyWarnings<T>(this Mock<T> mock, Times times)
            where T : class, ILogger
            => VerifyLogging(mock, LogLevel.Warning, times);

        public static void VerifyLogging<T>(this Mock<T> mock, LogLevel logLevel, Func<Times> times)
            where T : class, ILogger
            => VerifyLogging(mock, logLevel, times());

        public static void VerifyLogging<T>(this Mock<T> mock, LogLevel logLevel, Times times)
            where T : class, ILogger
        {
            mock.Verify(x => x.Log(logLevel, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), times);
        }
    }
}
