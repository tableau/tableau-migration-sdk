#region namespace
namespace MyMigrationApplication.Config
{
    public sealed class MyMigrationApplicationOptions
    {
        public EndpointOptions Source { get; set; } = new();

        public EndpointOptions Destination { get; set; } = new();
    }
}
#endregion