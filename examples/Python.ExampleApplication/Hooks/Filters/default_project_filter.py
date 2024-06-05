from tableau_migration import (
    IProject,
    ContentMigrationItem,
    ContentFilterBase)
    

class DefaultProjectFilter(ContentFilterBase[IProject]):
    def should_migrate(self, item: ContentMigrationItem[IProject]) -> bool:
        if item.source_item.name.casefold() == 'Default'.casefold():
            return False
        return True