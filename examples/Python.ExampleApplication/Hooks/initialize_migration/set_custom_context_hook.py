from tableau_migration import(
    InitializeMigrationHookBase,
    IInitailizeMigrationHookResult
)

from Csharp.ExampleApplication.Hooks.InitializeMigration import CustomContext

class SetMigrationContextHook(InitializeMigrationHookBase):
    def execute(self, ctx: IInitailizeMigrationHookResult) -> IInitailizeMigrationHookResult:
        ctx.scoped_services._get_service(CustomContext)