# Sample: Filter Users by SiteRole

In this example, all unlicensed users are excluded from migration.

# [Python](#tab/Python)

#### Filter Class

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/filters/unlicensed_user_filter.py)]

#### Registration

[Learn more.](~/samples/intro.md?tabs=Python#hook-registration)

[//]: <> (Adding this as code as regions are not supported in Python snippets)
```Python
plan_builder.filters.add(UnlicensedUserFilter)
```

# [C#](#tab/CSharp)

#### Filter Class

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Filters/UnlicensedUsersFilter.cs#class)]

#### Registration

[Learn more.](~/samples/intro.md?tabs=Python#hook-registration)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#UnlicensedUsersFilter-Registration)]

#### Dependency Injection

[Learn more.](~/articles/dependency_injection.md)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#UnlicensedUsersFilter-DI)]
