# Sample: Change Projects

In this example, source data sources and workbooks in a project named `Test` are migrated to the destination's `Production` project.

Both the C# and Python mapping classes inherit from a base class that handles most of the work, and then create an `IWorkbook` and `IDataSource` version.

# [Python](#tab/Python)

#### Mapping Class

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/mappings/change_project_mapping.py)]

#### Registration

[//]: <> (Adding this as code as regions are not supported in Python snippets)

```Python
plan_builder.mappings.add(ChangeProjectMappingForWorkbooks)
plan_builder.mappings.add(ChangeProjectMappingForDataSources)
```

See [hook registration](~/samples/index.md?tabs=Python#hook-registration) for more details.

# [C#](#tab/CSharp)

#### Mapping Class

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Mappings/ChangeProjectMapping.cs#class)]

#### Registration

[Learn more.](~/samples/index.md?tabs=CSharp#hook-registration)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#ChangeProjectMapping-Registration)]

#### Dependency Injection

[Learn more.](~/articles/dependency_injection.md)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#ChangeProjectMapping-DI)]
