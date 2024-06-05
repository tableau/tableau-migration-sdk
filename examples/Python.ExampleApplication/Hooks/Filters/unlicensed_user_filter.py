from tableau_migration import (
    IUser,
    ContentMigrationItem,
    ContentFilterBase,
    SiteRoles)
    

class UnlicensedUserFilter(ContentFilterBase[IUser]):
    def should_migrate(self, item: ContentMigrationItem[IUser]) -> bool:
        if item.source_item.license_level.casefold() == SiteRoles.UNLICENSED.casefold():
            return False
        return True