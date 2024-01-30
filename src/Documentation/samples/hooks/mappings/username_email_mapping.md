# Sample EmailDomainMapping

This example is for a hypothetical scenario where the Tableau Server usernames are the same as the user part of the email. It uses a configuration class to supply the email domain.

## `C#`

### [Mapping class](#tab/class)

[!code-csharp[CS](../../../../../examples/Csharp.ExampleApplication/Hooks/Mappings/EmailDomainMapping.cs#namespace)]

### [Configuration class](#tab/config)

[!code-csharp[CS](../../../../../examples/Csharp.ExampleApplication/Hooks/Mappings/EmailDomainMappingOptions.cs)]

### [Registration](#tab/registration)

See the line with `WithTableauCloudUsernames`

[!code-csharp[CS](../../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#EmailDomainMapping-Registration)]

### [Dependency Injection](#tab/di)

[!code-csharp[CS](../../../../../examples/Csharp.ExampleApplication/Program.cs#EmailDomainMapping-DI)]

---
