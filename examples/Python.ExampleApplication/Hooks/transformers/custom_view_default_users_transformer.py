from tableau_migration import (
    ContentTransformerBase,
    IContentReference,
    IPublishableCustomView
)

class CustomViewDefaultUsersTransformer(ContentTransformerBase[IPublishableCustomView]):
    
    #Pass in list of users retrieved from Users API
    default_users = []
    
    def transform(self, itemToTransform: IPublishableCustomView) -> IPublishableCustomView:
        itemToTransform.default_users = self.default_users
        return itemToTransform