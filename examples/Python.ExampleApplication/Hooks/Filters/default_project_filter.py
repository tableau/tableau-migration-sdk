from tableau_migration import (
    ContentFilterBase,
    ContentFilterContextItem,
    FilterStatus,
    IProject)

class DefaultProjectFilter(ContentFilterBase[IProject]):

    def filter(self, item: ContentFilterContextItem[IProject]) -> None:
        if item.source_item.name.casefold() == 'Default'.casefold():
            item.status = FilterStatus.CASCADE_SKIP