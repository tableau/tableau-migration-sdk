using System.Diagnostics.CodeAnalysis;
using Tableau.Migration.Engine.Endpoints;

namespace Tableau.Migration.Engine
{
    internal static class IMigrationExtensions
    {
        public static bool TryGetSourceApiEndpoint(this IMigration migration, [NotNullWhen(true)] out ISourceApiEndpoint? apiEndpoint)
        {
            apiEndpoint = null;

            if (migration.Source is ISourceApiEndpoint apiSourceEndpoint)
            {
                apiEndpoint = apiSourceEndpoint;
                return true;
            }

            return false;
        }

        public static bool TryGetDestinationApiEndpoint(this IMigration migration, [NotNullWhen(true)] out IDestinationApiEndpoint? apiEndpoint)
        {
            apiEndpoint = null;

            if (migration.Destination is IDestinationApiEndpoint apiDestinationEndpoint)
            {
                apiEndpoint = apiDestinationEndpoint;
                return true;
            }

            return false;
        }
    }
}
