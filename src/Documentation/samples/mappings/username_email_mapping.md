# Sample EmailDomainMapping

This example is for a hypothetical scenario where the Tableau Server usernames are the same as the user part of the email. It uses a configuration class to supply the email domain.

# [Python](#tab/Python)

#### Mapping Class

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/mappings/email_domain_mapping.py)]

#### Registration

[Learn more.](~/samples/index.md?tabs=Python#hook-registration)

See the line with `with_tableau_cloud_usernames`.

[//]: <> (Adding this as code as regions are not supported in Python snippets)
```Python
plan_builder = plan_builder \
                .from_source_tableau_server(
                    server_url=config['SOURCE']['URL'], 
                    site_content_url=config['SOURCE']['SITE_CONTENT_URL'], 
                    access_token_name=config['SOURCE']['ACCESS_TOKEN_NAME'], 
                    access_token=os.environ.get('TABLEAU_MIGRATION_SOURCE_TOKEN', config['SOURCE']['ACCESS_TOKEN'])) \
                .to_destination_tableau_cloud(
                    pod_url=config['DESTINATION']['URL'], 
                    site_content_url=config['DESTINATION']['SITE_CONTENT_URL'], 
                    access_token_name=config['DESTINATION']['ACCESS_TOKEN_NAME'], 
                    access_token=os.environ.get('TABLEAU_MIGRATION_DESTINATION_TOKEN', config['DESTINATION']['ACCESS_TOKEN'])) \
                .for_server_to_cloud() \
                .with_tableau_id_authentication_type() \
                # You can add authentication type mappings here      
                .with_tableau_cloud_usernames(EmailDomainMapping)
```

# [C#](#tab/CSharp)

This uses a configuration class to supply the email domain.

#### Mapping Class

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Mappings/EmailDomainMapping.cs#namespace)]

#### Configuration Class

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Mappings/EmailDomainMappingOptions.cs)]

#### Registration

[Learn more.](~/samples/index.md?tabs=CSharp#hook-registration)

See the line with `WithTableauCloudUsernames`.

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#EmailDomainMapping-Registration)]

#### Dependency Injection

[Learn more.](~/articles/dependency_injection.md)

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#EmailDomainMapping-DI)]
