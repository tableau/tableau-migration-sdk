from tableau_migration import (
    ContentTransformerBase,
    GranteeType,
    IPermissionSet
)

class ModifyPermissionsTransformer(ContentTransformerBase[IPermissionSet]):
    def transform(self, item_to_transform: IPermissionSet) -> IPermissionSet:
        filtered_grantees = [g for g in item_to_transform.grantee_capabilities if g.grantee_type != GranteeType.GROUP]
        return item_to_transform
