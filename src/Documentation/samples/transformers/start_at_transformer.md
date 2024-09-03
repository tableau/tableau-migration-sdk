# Sample: Adjust 'Start At' to Scheduled Tasks

This sample demonstrates how to adjust the 'Start At' of a Scheduled Task.

Both the Python and C# transformer classes inherit from a base class responsible for the core functionality.

## [Python](#tab/Python)

### Transformer Class

To adjust the 'Start At' in Python, you can use the following transformer class:

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/transformers/schedule_startat_transformer.py)]

### Registration

[//]: <> (Adding this as code as regions are not supported in python snippets)

```Python
plan_builder.transformers.add(SimpleScheduleStartAtTransformer)
```

See [hook registration](~/samples/index.md?tabs=Python#hook-registration) for more details.

## [C#](#tab/CSharp)

### Transformer Class

In C#, the transformer class for adjusting the 'Start At' of a given Scheduled Task is implemented as follows:

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Transformers/SimpleScheduleStartAtTransformer.cs#class)]

### Registration

To register the transformer in C#, follow the guidance provided in the [documentation](~/samples/index.md?tabs=CSharp#hook-registration).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#StartAtTransformer-Registration)]

### Dependency Injection

Learn more about dependency injection [here](~/articles/dependency_injection.md).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#StartAtTransformer-DI)]
