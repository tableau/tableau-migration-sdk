import logging
from tableau_migration import(
    MigrationActionCompletedHookBase,
    IMigrationActionResult
    )


class LogMigrationActionsHook(MigrationActionCompletedHookBase):
    def __init__(self) -> None:
        super().__init__()
        
        # Create a logger for this class
        self._logger = logging.getLogger(__name__)
        
    def execute(self, ctx: IMigrationActionResult) -> IMigrationActionResult:
        if(ctx.success):
            self._logger.info("Migration action completed successfully.")
        else:
            all_errors = "\n".join(ctx.errors)
            self._logger.warning("Migration action completed with errors:\n%s", all_errors)
            
        return None
