using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace Tableau.Migration.Tests
{
    /// <summary>
    /// Provides culture name test data to theory tests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CultureNameTestDataAttribute : DataAttribute
    {
        private static readonly string[] _cultureNames = new string[2]
        {
            "en-US",
            "fi-FI" // has an alternate decimal number format
        };

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            return _cultureNames
                .Select(x => new object[] { x });
        }
    }
}
