using System;
using System.Runtime.CompilerServices;

namespace DependencyInjection.ExampleApplication
{
    public static class Assert
    {
        public static void SameReferences(
            object? instance1,
            object? instance2,
            [CallerArgumentExpression(nameof(instance1))] string instance1Name = "",
            [CallerArgumentExpression(nameof(instance2))] string instance2Name = "")
        {
            AssertReferences(instance1, instance2, true, $"{instance1Name} reference equals {instance2Name}");
        }

        public static void DifferentReferences(
            object? instance1,
            object? instance2,
            [CallerArgumentExpression(nameof(instance1))] string instance1Name = "",
            [CallerArgumentExpression(nameof(instance2))] string instance2Name = "")
        {
            AssertReferences(instance1, instance2, false, $"{instance1Name} reference does not equal {instance2Name}");
        }

        private static void AssertReferences(object? instance1, object? instance2, bool expected, string assertionMessage)
        {
            var actual = Object.ReferenceEquals(instance1, instance2);
            var passed = expected == actual;

            Console.WriteLine(
                "{0}: Expected = {1}, Actual = {2}, Passed = {3}",
                assertionMessage,
                expected,
                actual,
                passed);
        }
    }
}
