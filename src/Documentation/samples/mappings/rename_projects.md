# Sample: Rename Projects

In this example, the source project named `Test` is renamed to `Production` on the destination.

# [Python](#tab/Python)

#### Mapping Class

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/mappings/project_rename_mapping.py)]

#### Registration

[//]: <> (Adding this as code as regions are not supported in Python snippets)

```Python
plan_builder.mappings.add(ProjectRenameMapping)
```

See [hook registration](~/samples/index.md?tabs=Python#hook-registration) for more details.

# [C#](#tab/CSharp)

#### Mapping Class

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Mappings/ProjectRenameMapping.cs#class)]

#### Registration

[Learn more.](~/samples/index.md?tabs=CSharp#hook-registration)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#ProjectRenameMapping-Registration)]

#### Dependency Injection

[Learn more.](~/articles/dependency_injection.md)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#ProjectRenameMapping-DI)]
