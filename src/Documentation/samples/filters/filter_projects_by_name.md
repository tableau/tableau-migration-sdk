# Sample: Filter Projects by Name

In this example, the project named `Default` is filtered out.

# [Python](#tab/Python)

#### Filter Class

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/filters/default_project_filter.py)]

#### Registration

[//]: <> (Adding this as code as regions are not supported in Python snippets)

```Python
plan_builder.filters.add(DefaultProjectFilter)
```

See [hook registration](~/samples/index.md?tabs=Python#hook-registration) for more details.

# [C#](#tab/CSharp)

#### Filter Class

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Filters/DefaultProjectsFilter.cs#class)]

#### Registration

[Learn more.](~/samples/index.md?tabs=CSharp#hook-registration)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#DefaultProjectsFilter-Registration)]

#### Dependency Injection

[Learn more.](~/articles/dependency_injection.md)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#DefaultProjectsFilter-DI)]
