namespace Tableau.Migration.Tests
{
    public class StringEnumDataAttribute<T> : ValuesAttribute<string>
        where T : StringEnum<T>
    {
        public StringEnumDataAttribute(params string[] exclude)
            : base(StringEnum<T>.GetAll(exclude))
        { }
    }
}
