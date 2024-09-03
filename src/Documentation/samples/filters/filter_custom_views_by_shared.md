# Sample: Filter Custom Views by Shared Status

In this example, the custom views currently shared to all users are excluded from migration.

# [Python](#tab/Python)

#### Filter Class

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/filters/shared_custom_view_filter.py)]

#### Registration

[Learn more.](~/samples/index.md?tabs=Python#hook-registration)

[//]: <> (Adding this as code as regions are not supported in Python snippets)
```Python
plan_builder.filters.add(SharedCustomViewFilter)
```

# [C#](#tab/CSharp)

#### Filter Class

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Filters/SharedCustomViewFilter.cs#class)]

#### Registration

[Learn more.](~/samples/index.md?tabs=Python#hook-registration)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#SharedCustomViewFilter-Registration)]

#### Dependency Injection

[Learn more.](~/articles/dependency_injection.md)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#SharedCustomViewFilter-DI)]
