from tableau_migration import (
    ICustomView,
    ContentMigrationItem,
    ContentFilterBase)
    

class SharedCustomViewFilter(ContentFilterBase[ICustomView]):
    def should_migrate(self, item: ContentMigrationItem[ICustomView]) -> bool:
        if item.source_item.shared == True:
            return False
        return True