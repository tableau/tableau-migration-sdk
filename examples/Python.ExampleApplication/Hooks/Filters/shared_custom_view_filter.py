from tableau_migration import (
    ICustomView,
    ContentFilterBase,
    ContentFilterContextItem,
    FilterStatus)


class SharedCustomViewFilter(ContentFilterBase[ICustomView]):
    def filter(self, item: ContentFilterContextItem[ICustomView]) -> None:
        if item.source_item.shared == True:
            item.status = FilterStatus.SKIP