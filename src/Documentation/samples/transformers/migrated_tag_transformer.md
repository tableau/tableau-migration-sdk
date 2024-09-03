# Sample: Adding Tags to Content

This sample illustrates how to add a `Migrated` tag to both data sources and workbooks.

Both the Python and C# transformer classes inherit from a base class that handles the core functionality, then create versions for `IPublishableWorkbook` and `IPublishableDataSource`.

## [Python](#tab/Python)

### Transformer Class

To implement the tag addition in Python, you can utilize the following transformer class:

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/transformers/migrated_tag_transformer.py)]

### Registration

[//]: <> (Adding this as code as regions are not supported in python snippets)

```Python
plan_builder.transformers.add(MigratedTagTransformerForDataSources)
plan_builder.transformers.add(MigratedTagTransformerForWorkbooks)
```

See [hook registration](~/samples/index.md?tabs=Python#hook-registration) for more details.

## [C#](#tab/CSharp)

### Transformer Class

In C#, the transformer class for adding tags is implemented as shown below:

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Transformers/MigratedTagTransformer.cs#class)]

### Registration

To register the transformer in C#, follow the guidance provided in the [documentation](~/samples/index.md?tabs=CSharp#hook-registration).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#MigratedTagTransformer-Registration)]

### Dependency Injection

Learn more about dependency injection [here](~/articles/dependency_injection.md).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#MigratedTagTransformer-DI)]
