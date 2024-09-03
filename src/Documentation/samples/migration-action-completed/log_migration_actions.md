# Sample: Migration Action Logging

This sample illustrates how to implement migration action logging, capturing the statuses of migration actions upon completion.

# [Python](#tab/Python)

### Migration Action Completed Hook Class

To log migration action statuses in Python, you can utilize the following hook class:

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/migration_action_completed/log_migration_actions_hook.py)]

### Registration

[//]: <> (Adding this as code as regions are not supported in python snippets)

```Python
plan_builder.hooks.add(LogMigrationActionsHookForUsers)
plan_builder.hooks.add(LogMigrationActionsHookForGroups)
```

See [hook registration](~/samples/index.md?tabs=Python#hook-registration) for more details.

# [C#](#tab/CSharp)

### Migration Action Completed Hook Class

In C#, you can implement the migration action completed hook as demonstrated below:

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/MigrationActionCompleted/LogMigrationActionsHook.cs#class)]

### Registration

To register the hook in C#, follow the instructions provided in the [documentation](~/samples/index.md?tabs=CSharp#hook-registration).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#LogMigrationActionsHook-Registration)]

### Dependency Injection

Learn more about dependency injection [here](~/articles/dependency_injection.md).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#LogMigrationActionsHook-DI)]
