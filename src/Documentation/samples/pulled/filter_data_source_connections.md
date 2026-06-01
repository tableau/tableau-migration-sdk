# Sample: Filter Data Source Connections

This sample illustrates how to filter published data sources that use a given connection type.
It uses a pulled hook as the connection details are not available until the data source has been fully retrieved.

# [Python](#tab/Python)

#### Filter Class

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/pulled/data_source_connection_pulled.py)]

#### Registration

[//]: <> (Adding this as code as regions are not supported in Python snippets)

```Python
plan_builder.hooks.add(DataSourceConnectionPulled)
```

See [hook registration](~/samples/index.md?tabs=Python#hook-registration) for more details.

# [C#](#tab/CSharp)

#### Filter Class

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Pulled/DataSourceConnectionPulled.cs#class)]

#### Registration

[Learn more.](~/samples/index.md?tabs=CSharp#hook-registration)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#DataSourceConnectionPulled-Registration)]

#### Dependency Injection

[Learn more.](~/articles/dependency_injection/index.md)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#DataSourceConnectionPulled-DI)]

---