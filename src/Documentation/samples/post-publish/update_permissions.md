# Sample: Update permissions

In the following example, write permissions for content with a `Production` tag will be set to `Deny` as a separate post-publish step.
To modify permissions that are automatically updated as part of the standard post-publish step, see the [modify permissions transformer sample](~/samples/transformers/modify_permissions.md).

## [Post-Publish Hook Class](#tab/class)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/PostPublish/UpdatePermissionsHook.cs#class)]

## [Registration](#tab/registration)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#UpdatePermissionsHook-Registration)]

## [Dependency Injection](#tab/di)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#UpdatePermissionsHook-DI)]

---
