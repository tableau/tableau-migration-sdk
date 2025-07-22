# Sample: Modify Permissions

This sample demonstrates how to modify permissions that are automatically updated as part of the standard post-publish step the SDK performs.
To perform a separate permission update, with full control over the update logic, see the [update permissions post-publish hook sample](~/samples/post-publish/update_permissions.md).

Permission transformers are registered for the `IPermissionSet` type and runs for all content types that support permissions, as well as project default permissions for all content types.

## [Python](#tab/Python)

### Transformer Class

To modify permissions in Python, you can use the following transformer class:

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/transformers/modify_permissions_transformer.py)]

### Registration

[//]: <> (Adding this as code as regions are not supported in python snippets)

```Python
plan_builder.transformers.add(ModifyPermissionsTransformer)
```

See [hook registration](~/samples/index.md?tabs=Python#hook-registration) for more details.

## [C#](#tab/CSharp)

### Transformer Class

In C#, the transformer class for modifying permissions is implemented as follows:

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Transformers/ModifyPermissionsTransformer.cs#class)]

### Registration

To register the transformer in C#, follow the guidance provided in the [documentation](~/samples/index.md?tabs=CSharp#hook-registration).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#ModifyPermissionsTransformer-Registration)]

### Dependency Injection

Learn more about dependency injection [here](~/articles/dependency_injection.md).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#ModifyPermissionsTransformer-DI)]