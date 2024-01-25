using System.Collections.Immutable;
using System.Linq;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;

namespace Tableau.Migration.Tests.Unit
{
    public static class AutoFixtureExtensions
    {
        /// <summary>
        /// Creates a <see cref="TableauServerResponse"/> error response instance. 
        /// </summary>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="fixture">The <see cref="IFixture"/> used to create instances.</param>
        /// <returns>A new <see cref="TableauServerResponse"/> instance.</returns>
        public static TResponse CreateErrorResponse<TResponse>(this IFixture fixture)
            where TResponse : TableauServerResponse, new()
        {
            return new()
            {
                Error = fixture.Create<Error>()
            };
        }

        /// <summary>
        /// Creates a <see cref="TableauServerResponse"/> error response instance. 
        /// </summary>
        /// <param name="fixture">The <see cref="IFixture"/> used to create instances.</param>
        /// <returns>A new <see cref="TableauServerResponse"/> instance.</returns>
        public static TableauServerResponse CreateErrorResponse(this IFixture fixture)
            => CreateErrorResponse<TestTableauServerResponse>(fixture);

        /// <summary>
        /// Creates a <see cref="TableauServerResponse"/> response instance. 
        /// </summary>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="fixture">The <see cref="IFixture"/> used to create instances.</param>
        /// <returns>A new <see cref="TableauServerResponse"/> instance.</returns>
        public static TResponse CreateResponse<TResponse>(this IFixture fixture)
            where TResponse : TableauServerResponse
            => fixture.Build<TResponse>()
                .Without(r => r.Error)
                .Create();

        /// <summary>
        /// Creates a <see cref="TableauServerResponse"/> response instance. 
        /// </summary>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="fixture">The <see cref="IFixture"/> used to create instances.</param>
        /// <param name="count">The number of responses to create.</param>
        /// <returns>A <see cref="TableauServerResponse"/> collection instance.</returns>
        public static IImmutableList<TResponse> CreateResponses<TResponse>(this IFixture fixture, int count)
            where TResponse : TableauServerResponse
            => Enumerable.Range(0, count).Select(_ => fixture.CreateResponse<TResponse>()).ToImmutableArray();
    }
}