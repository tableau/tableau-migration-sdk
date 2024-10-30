# Sample: Set Custom Migration Scoped Context

This example demonstrates how to set custom context in the migration scoped dependency injection container using an initialize migration hook.
This is useful when other hooks like filters are registered with dependency injection and rely on migration scoped services.

# [Python](#tab/Python)

#### Custom Context Service Class

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/InitializeMigration/CustomContext.cs#class)]

#### Custom Context Service Class Dependency Injection

[Learn more.](~/articles/dependency_injection.md)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#SetCustomContext-Service-DI)]

#### Initialize Migration Hook Class

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/initialize_migration/set_custom_context_hook.py)]

#### Registration

[Learn more.](~/samples/index.md?tabs=Python#hook-registration)

[//]: <> (Adding this as code as regions are not supported in Python snippets)
```Python
plan_builder.hooks.add(SetMigrationContextHook)
```

# [C#](#tab/CSharp)

#### Custom Context Service Class

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/InitializeMigration/CustomContext.cs#class)]

#### Custom Context Service Class Dependency Injection

[Learn more.](~/articles/dependency_injection.md)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#SetCustomContext-Service-DI)]

#### Initialize Migration Hook Class

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/InitializeMigration/SetMigrationContextHook.cs#class)]

#### Registration

[Learn more.](~/samples/index.md?tabs=CSharp#hook-registration)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#SetCustomContext-Registration)]

#### Dependency Injection

[Learn more.](~/articles/dependency_injection.md)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#SetCustomContext-Hook-DI)]