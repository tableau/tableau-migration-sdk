import logging
from tableau_migration import (
    BulkPostPublishHookBase,
    BulkPostPublishContext,
    IDataSource)


class BulkLoggingHookForDataSources(BulkPostPublishHookBase[IDataSource]):
    def __init__(self) -> None:
        super().__init__()
        
        # Create a logger for this class
        self._logger = logging.getLogger(__name__)
        
    def execute(self, ctx: BulkPostPublishContext[IDataSource]) -> BulkPostPublishContext[IDataSource]:
        # Log the number of items published in the batch.
        self._logger.info("Published %d IDataSource item(s).", ctx.published_items.count)
        return None
    