# User and Group authentication

[Tableau Server](https://help.tableau.com/current/server/en-us/security_auth.htm) and [Tableau Cloud](https://help.tableau.com/current/online/en-us/security_auth.htm) support different authentication types. The Migration SDK supports authentication types listed in [AuthenticationTypes](xref:Tableau.Migration.Api.Rest.Models.Types.AuthenticationTypes) out of the box.

## Defaults

The default authentication type for users is [`ServerDefault`](xref:Tableau.Migration.Api.Rest.Models.Types.AuthenticationTypes). It is set by the [`UserAuthenticationTypeTransformer`](xref:Tableau.Migration.Engine.Hooks.Transformers.Default.UserAuthenticationTypeTransformer).

## Server to Cloud

It is possible to set the authentication type on users and groups. The [`ServerToCloudMigrationPlanBuilder`](xref:Tableau.Migration.Engine.ServerToCloudMigrationPlanBuilder) contains methods to support mapping Server users and groups to Cloud authentication types.

### SAML and Tableau ID specific

1. `WithSamlAuthenticationType(string domain)` : Adds mappings for user and group domains based on the SAML authentication type with a domain supplied.
2. `WithTableauIdAuthenticationType(bool mfa = true)`: Adds mappings for user and group domains based on the Tableau ID authentication type with or without multi-factor authentication.

### Tableau Cloud Usernames

The `WithTableauCloudUsernames()` and its overloads allow you to supply an email domain or your own implementation of [`ITableauCloudUsernameMapping`](xref:Tableau.Migration.Engine.Hooks.Mappings.Default.ITableauCloudUsernameMapping) for Tableau Cloud user names.

### General methods

The `WithAuthenticationType()` and its overloads allow you to supply your chosen authentication type with your implementation of [`IAuthenticationTypeDomainMapping`](xref:Tableau.Migration.Engine.Hooks.Mappings.Default.IAuthenticationTypeDomainMapping).

## Custom Mapping

You can also build your own mapping to supply to the appropriate `WithAuthenticationType()` overload. See [Sample EmailDomainMapping](~/samples/mappings/username_email_mapping.md) for example code.
