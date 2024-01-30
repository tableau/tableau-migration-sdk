# Sample: Filter users by [SiteRole](xref:Tableau.Migration.Api.Rest.Models.SiteRoles)

In the following example, all unlicensed users are excluded from migration.

## [Filter class](#tab/class)

[!code-csharp[](../../../../../examples/Csharp.ExampleApplication/Hooks/Filters/UnlicensedUsersFilter.cs#class)]

## [Registration](#tab/registration)

[!code-csharp[](../../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#UnlicensedUsersFilter-Registration)]

## [Dependency Injection](#tab/di)

[!code-csharp[](../../../../../examples/Csharp.ExampleApplication/Program.cs#UnlicensedUsersFilter-DI)]

---
