using System;
using System.Threading.Tasks;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class ExceptionExtensionsTests
    {
        public class IsCancellationException
        {
            [Fact]
            public void OperationCanceledException()
            {
                Assert.True(new OperationCanceledException().IsCancellationException());
            }

            [Fact]
            public void TaskCanceledException()
            {
                Assert.True(new TaskCanceledException().IsCancellationException());
            }

            [Fact]
            public void WrappedTaskCanceledException()
            {
                Assert.True(new AggregateException(new TaskCanceledException()).IsCancellationException());
            }

            [Fact]
            public void MixedAggregateException()
            {
                Assert.False(new AggregateException(new TaskCanceledException(), new Exception()).IsCancellationException());
            }

            [Fact]
            public void NonCanceledException()
            {
                Assert.False(new Exception().IsCancellationException());
            }
        }
    }
}
