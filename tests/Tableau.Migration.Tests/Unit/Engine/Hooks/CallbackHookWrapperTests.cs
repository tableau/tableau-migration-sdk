using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks
{
    public class CallbackHookWrapperTests
    {
        [Fact]
        public async Task CallsCallbackAsync()
        {
            int calls = 0;
            var callbackResult = MigrationActionResult.Succeeded();

            Task<IMigrationActionResult?> callback(IMigrationActionResult ctx, CancellationToken cancel)
            {
                calls++;
                return Task.FromResult<IMigrationActionResult?>(callbackResult);
            }

            var hook = new CallbackHookWrapper<IMigrationActionCompletedHook, IMigrationActionResult>(callback);

            var result = await hook.ExecuteAsync(MigrationActionResult.Succeeded(), default);

            Assert.Same(callbackResult, result);
            Assert.Equal(1, calls);
        }
    }
}
