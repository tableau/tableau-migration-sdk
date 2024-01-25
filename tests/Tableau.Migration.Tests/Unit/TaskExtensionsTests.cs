using System;
using System.Threading.Tasks;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class TaskExtensionsTests
    {
        #region - AwaitResult -

        public class AwaitResult
        {
            private static async Task<int> AsyncMethodAsync(bool throwException = false)
            {
                var i = await Task.Run(() => 47);

                if (throwException)
                {
                    throw new Exception("break");
                }

                return i;
            }

            [Fact]
            public void AwaitsResult()
            {
                var i = AwaitResult.AsyncMethodAsync().AwaitResult();

                Assert.Equal(47, i);
            }

            [Fact]
            public void FaultedTaskExceptionPropagated()
            {
                Exception? ex = null;
                try
                {
                    var i = AwaitResult.AsyncMethodAsync(throwException: true).AwaitResult();
                }
                catch (Exception e)
                {
                    ex = e;
                }

                Assert.NotNull(ex);
                Assert.IsNotType<AggregateException>(ex);
            }
        }

        #endregion
    }
}
