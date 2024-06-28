from tableau_migration import(
    IUser,
    TableauCloudUsernameMappingBase,
    ContentMappingContext)


class EmailDomainMapping(TableauCloudUsernameMappingBase):
    def map(self, ctx: ContentMappingContext[IUser]) -> ContentMappingContext[IUser]:
        _email_domain: str = "@mycompany.com"
        
        _tableau_user_domain = ctx.mapped_location.parent()

        # Re-use an existing email if it already exists. 
        if ctx.content_item.email:
            return ctx.map_to(_tableau_user_domain.append(ctx.content_item.email))
        
        # Takes the existing username and appends the domain to build the email
        new_email = ctx.content_item.name + _email_domain
        return ctx.map_to(_tableau_user_domain.append(new_email))