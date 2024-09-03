# Sample: Changing Default Users for Custom View

This sample illustrates how to change the default users for a custom view

Both the Python and C# transformer classes inherit from a base class that handles the core functionality, then create versions for `IPublishableCustomView`.

## [Python](#tab/Python)

### Transformer Class

To implement the tag addition in Python, you can utilize the following transformer class:

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/transformers/custom_view_default_users_transformer.py)]

### Registration

For detailed instructions on registering the transformer, refer to the [documentation](~/samples/index.md?tabs=Python#hook-registration).

[//]: <> (Adding this as code as regions are not supported in python snippets)
```Python
plan_builder.transformers.add(CustomViewDefaultUsersTransformer)
```

## [C#](#tab/CSharp)

### Transformer Class

In C#, the transformer class for adding tags is implemented as shown below:

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Transformers/CustomViewDefaultUsersTransformer.cs#class)]

### Registration

To register the transformer in C#, follow the guidance provided in the [documentation](~/samples/index.md?tabs=CSharp#hook-registration).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#CustomViewDefaultUsersTransformer-Registration)]

### Dependency Injection

Learn more about dependency injection [here](~/articles/dependency_injection.md).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#CustomViewDefaultUsersTransformer-DI)]

