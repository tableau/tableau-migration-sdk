from typing import TypeVar
from tableau_migration import (
    empty_pager,
    MigrationContentLoaderBase
)

TContent = TypeVar("T")

class EmptyMigrationContentLoader(MigrationContentLoaderBase[TContent]):
    def get_migration_content_pager(self, page_size: int):
        return empty_pager(TContent)