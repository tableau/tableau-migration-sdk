from typing import Optional

from tableau_migration import (
    ContentItemPulledContext,
    ContentItemPulledHookBase,
    FilterStatus,
    IPublishableDataSource)

class DataSourceConnectionPulled(ContentItemPulledHookBase[IPublishableDataSource]):

    def execute(self, ctx: ContentItemPulledContext[IPublishableDataSource]) -> Optional[ContentItemPulledContext[IPublishableDataSource]]:
        if any(c.type.casefold() == "postgres".casefold() for c in ctx.pulled_item.connections):
            ctx.status = FilterStatus.CASCADE_SKIP

        return ctx