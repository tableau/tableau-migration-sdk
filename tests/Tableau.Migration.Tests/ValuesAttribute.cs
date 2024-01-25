using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace Tableau.Migration.Tests
{
    public class ValuesAttribute<T> : DataAttribute
    {
        private readonly IImmutableList<T?> _values;

        public ValuesAttribute(IEnumerable<T?> values)
            : this(values.ToArray())
        { }

        public ValuesAttribute(params T?[] values)
        {
            _values = values.ToImmutableArray();
        }

        public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
        {
            foreach (var value in GetValues())
                yield return CreateArguments(value);
        }

        protected virtual IEnumerable<T?> GetValues() => _values;

        protected virtual object?[] CreateArguments(T? value)
            => new object?[] { value };
    }
}
