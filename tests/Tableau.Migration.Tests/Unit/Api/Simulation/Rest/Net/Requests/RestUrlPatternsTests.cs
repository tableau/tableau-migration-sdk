using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Simulation.Rest.Net.Requests
{
    public class RestUrlPatternsTests
    {
        [Theory]
        [InlineData("sites", "/api/9.9/sites")]
        [InlineData("sites", "/api/9.9/sites/")]
        [InlineData("sites", "/api/9.99/sites")]
        [InlineData("sites", "/api/9.99/sites/")]
        public void RestApiUrl(string suffix, string input)
        {
            Assert.Matches(RestUrlPatterns.RestApiUrl(suffix), input);
        }

        [Theory]
        [InlineData("sites", "/api/9.9/sites/14904aef-a798-4c7f-b178-2ad224a09b0e")]
        [InlineData("sites", "/api/9.9/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/")]
        [InlineData("sites", "/api/9.99/sites/14904aef-a798-4c7f-b178-2ad224a09b0e")]
        [InlineData("sites", "/api/9.99/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/")]
        public void EntityUrl(string suffix, string input)
        {
            Assert.Matches(RestUrlPatterns.EntityUrl(suffix), input);
        }

        [Theory]
        [InlineData("workbooks", "/api/9.9/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks")]
        [InlineData("workbooks", "/api/9.9/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks/")]
        [InlineData("workbooks", "/api/9.99/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks")]
        [InlineData("workbooks", "/api/9.99/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks/")]
        public void SiteUrl(string suffix, string input)
        {
            Assert.Matches(RestUrlPatterns.SiteUrl(suffix), input);
        }

        [Theory]
        [InlineData("workbooks", "/api/9.9/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks/63c3dca3-6a85-4437-815a-5392b239d5b3")]
        [InlineData("workbooks", "/api/9.9/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks/63c3dca3-6a85-4437-815a-5392b239d5b3/")]
        [InlineData("workbooks", "/api/9.99/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks/63c3dca3-6a85-4437-815a-5392b239d5b3")]
        [InlineData("workbooks", "/api/9.99/sites/14904aef-a798-4c7f-b178-2ad224a09b0e/workbooks/63c3dca3-6a85-4437-815a-5392b239d5b3/")]
        public void SiteEntityUrl(string suffix, string input)
        {
            Assert.Matches(RestUrlPatterns.SiteEntityUrl(suffix), input);
        }
    }
}
