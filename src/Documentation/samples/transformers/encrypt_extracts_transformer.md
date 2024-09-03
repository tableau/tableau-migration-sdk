# Sample: Encrypt Extracts

This sample demonstrates how to encrypt workbook and data source extracts, irrespective of their original state.

Both the Python and C# transformer classes inherit from a base class responsible for the core functionality, then generate versions for `IPublishableWorkbook` and `IPublishableDataSource`.

## [Python](#tab/Python)

### Transformer Class

To encrypt extracts in Python, you can use the following transformer class:

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/transformers/encrypt_extracts_transformer.py)]

### Registration

[//]: <> (Adding this as code as regions are not supported in python snippets)

```Python
plan_builder.transformers.add(EncryptExtractTransformerForDataSources)
plan_builder.transformers.add(EncryptExtractTransformerForWorkbooks)
```

See [hook registration](~/samples/index.md?tabs=Python#hook-registration) for more details.

## [C#](#tab/CSharp)

### Transformer Class

In C#, the transformer class for encrypting extracts is implemented as follows:

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Transformers/EncryptExtractTransformer.cs#class)]

### Registration

To register the transformer in C#, follow the guidance provided in the [documentation](~/samples/index.md?tabs=CSharp#hook-registration).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#EncryptExtractTransformer-Registration)]

### Dependency Injection

Learn more about dependency injection [here](~/articles/dependency_injection.md).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#EncryptExtractTransformer-DI)]
