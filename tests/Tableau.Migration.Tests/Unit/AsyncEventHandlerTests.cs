using System.Threading.Tasks;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class AsyncEventHandlerTests
    {
        private class TestClass
        {
            public event AsyncEventHandler? EventTriggered;

            public async void TriggerEvent()
            {
                if (EventTriggered is not null)
                    await EventTriggered.Invoke(default);
            }
        }

        [Fact]
        public void Calls_handler()
        {
            var obj = new TestClass();

            var count = 0;

            obj.EventTriggered += async (c) =>
            {
                count++;
                await Task.CompletedTask;
            };

            obj.TriggerEvent();

            Assert.Equal(1, count);
        }
    }
}
