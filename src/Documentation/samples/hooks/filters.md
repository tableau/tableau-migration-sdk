# Sample filters

Filters allow you to skip migrating certain content items.

> [!Note]
> Filters do not have a cascading effect. You will need to write similar filters or mappings for the related content items as well.

The following are samples that cover some common scenarios.

## Sample 1:  Filter projects by name

In the following example, the project named `Default` is filtered out.

### [Filter class](#tab/sample1-class)

[!code-csharp[CS](../../../../examples/Csharp.ExampleApplication/Hooks/Filters/DefaultProjectsFilter.cs#class)]

### [Registration](#tab/sample1-registration)

[!code-csharp[CS](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#DefaultProjectsFilter-Registration)]

### [Dependency Injection](#tab/sample1-di)

[!code-csharp[CS](../../../../examples/Csharp.ExampleApplication/Program.cs#DefaultProjectsFilter-DI)]

---

## Sample 2:  Filter users by [SiteRole](xref:Tableau.Migration.Api.Rest.Models.SiteRoles)

In the following example, all unlicensed users are excluded from migration.

### [Filter class](#tab/sample2-class)

[!code-csharp[CS](../../../../examples/Csharp.ExampleApplication/Hooks/Filters/UnlicensedUsersFilter.cs#class)]

### [Registration](#tab/sample2-registration)

[!code-csharp[CS](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#UnlicensedUsersFilter-Registration)]

### [Dependency Injection](#tab/sample2-di)

[!code-csharp[CS](../../../../examples/Csharp.ExampleApplication/Program.cs#UnlicensedUsersFilter-DI)]

---
