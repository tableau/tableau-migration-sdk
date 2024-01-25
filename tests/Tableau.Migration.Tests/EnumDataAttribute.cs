using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Tableau.Migration.Tests
{
    public class EnumDataAttribute<TEnum> : DataAttribute
        where TEnum : struct, Enum
    {
        public override IEnumerable<object[]?> GetData(MethodInfo testMethod)
        {
            foreach (var value in Enum.GetValues<TEnum>())
            {
                yield return new object[] { value };
            }
        }
    }
}
