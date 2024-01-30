# Sample: Migration action logging

In the following example, a migration action completed hook logs migration action statuses.

## [Migration Action Completed Hook Class](#tab/class)

[!code-csharp[](../../../../../examples/Csharp.ExampleApplication/Hooks/MigrationActionCompleted/LogMigrationActionsHook.cs#class)]

## [Registration](#tab/registration)

[!code-csharp[](../../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#LogMigrationActionsHook-Registration)]

## [Dependency Injection](#tab/di)

[!code-csharp[](../../../../../examples/Csharp.ExampleApplication/Program.cs#LogMigrationActionsHook-DI)]

---
