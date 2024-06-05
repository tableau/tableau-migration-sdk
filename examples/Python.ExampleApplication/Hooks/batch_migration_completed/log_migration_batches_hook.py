import logging
from typing import TypeVar
from tableau_migration import(
    ContentBatchMigrationCompletedHookBase,
    IContentBatchMigrationResult,
    IUser
    )

T = TypeVar("T")

class LogMigrationBatchesHook(ContentBatchMigrationCompletedHookBase[T]):
    def __init__(self) -> None:
        super().__init__()
        self._logger = logging.getLogger(__name__)
        
    def execute(self, ctx: IContentBatchMigrationResult[T]) -> IContentBatchMigrationResult[T]:
        
        item_status = ""
        for item in ctx.item_results:
            item_status += "%s: %s".format(item.manifest_entry.source.location, item.manifest_entry.status)
                
        self._logger.info("%s batch of %d item(s) completed:\n%s", ctx._content_type, ctx.item_results.count, item_status)
        
        pass
    
class LogMigrationBatchesHookForUsers(ContentBatchMigrationCompletedHookBase[IUser]):
    def __init__(self) -> None:
        super().__init__()
        self._content_type = "User";

class LogMigrationBatchesHookForGoups(ContentBatchMigrationCompletedHookBase[IUser]):
    def __init__(self) -> None:
        super().__init__()
        self._content_type = "Group";