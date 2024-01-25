using System.Net.Http;
using Xunit;

namespace Tableau.Migration.Tests
{
    internal static class HttpContentExtensions
    {
        public static void AssertSingleHeaderValue(this HttpContent content, string headerKey, string headerValue)
        {
            content.AssertHeaderExists(headerKey);

            var actualHeaderValue = Assert.Single(content.Headers.GetValues(headerKey));

            Assert.Equal(headerValue, actualHeaderValue);
        }

        public static void AssertHeaderExists(this HttpContent content, string headerKey)
        {
            Assert.True(content.Headers.Contains(headerKey));
        }

        public static void AssertHeaderDoesNotExist(this HttpContent content, string headerKey)
        {
            Assert.False(content.Headers.Contains(headerKey));
        }
    }
}
