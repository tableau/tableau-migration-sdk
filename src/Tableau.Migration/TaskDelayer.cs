using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration
{
    internal class TaskDelayer : ITaskDelayer
    {
        public async Task DelayAsync(TimeSpan delay, CancellationToken cancel)
            => await Task.Delay(delay, cancel).ConfigureAwait(false);
    }
}
