from tableau_migration import (
    ContentFilterBase,
    ContentFilterContextItem,
    FilterStatus,
    IUser,
    SiteRoles)    

class UnlicensedUserFilter(ContentFilterBase[IUser]):

    def filter(self, item: ContentFilterContextItem[IUser]) -> None:
        if item.source_item.license_level.casefold() == SiteRoles.UNLICENSED.casefold():
            item.status = FilterStatus.SKIP