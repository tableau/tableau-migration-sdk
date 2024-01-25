using Tableau.Migration.Api;

namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Interface for an object describing a source or destination endpoint defined in a <see cref="IMigrationPlan"/>.
    /// </summary>
    /// <param name="SiteConnectionConfiguration"><inheritdoc /></param>
    public record TableauApiEndpointConfiguration(TableauSiteConnectionConfiguration SiteConnectionConfiguration) : ITableauApiEndpointConfiguration
    {
        /// <summary>
        /// A <see cref="TableauApiEndpointConfiguration"/> with empty values, useful to detect if an endpoint has not yet been configured without using null.
        /// </summary>
        public static readonly TableauApiEndpointConfiguration Empty = new(TableauSiteConnectionConfiguration.Empty);

        /// <inheritdoc />
        public IResult Validate() => SiteConnectionConfiguration.Validate();
    }
}
