namespace Tableau.Migration.TestApplication.Config
{
    public sealed class TestApplicationOptions
    {
        public EndpointOptions Source { get; set; } = new();

        public EndpointOptions Destination { get; set; } = new();
    }
}
