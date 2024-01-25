using System;
using Tableau.Migration.Api.Simulation;

namespace Tableau.Migration.Tests
{
    internal class SimulatedConnectionComparer : ComparerBase<SimulatedConnection>
    {
        public static SimulatedConnectionComparer Instance = new();

        public override int CompareItems(SimulatedConnection x, SimulatedConnection y)
        {
            Guard.AgainstNull(x.Credentials, nameof(x.Credentials));
            Guard.AgainstNull(y.Credentials, nameof(y.Credentials));

            int result = StringComparer.Ordinal.Compare(x.ServerAddress, y.ServerAddress);

            if (result != 0) return result;

            result = StringComparer.Ordinal.Compare(x.ServerPort, y.ServerPort);
            if (result != 0) return result;

            result = StringComparer.Ordinal.Compare(x.Credentials.Name, y.Credentials.Name);
            if (result != 0) return result;

            result = StringComparer.Ordinal.Compare(x.Credentials.Embed, y.Credentials.Embed);
            if (result != 0) return result;

            result = StringComparer.Ordinal.Compare(x.Credentials.OAuth, y.Credentials.OAuth);
            if (result != 0) return result;

            //Do not test password, we don't have it, and because of encryption we can't verify anyway

            return 0;


        }
    }
}
