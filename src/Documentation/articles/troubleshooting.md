# Troubleshooting

## Common issues

### Migration fails due to invalid credentials

1. Make sure credentials are correct in Migration SDK configuration.
2. If they are incorrect/absent, [Create a Personal Access Token (PAT)](https://help.tableau.com/current/server/en-us/security_personal_access_tokens.htm#:~:text=Create%20personal%20access%20tokens,-Users%20must%20create&text=Users%20with%20accounts%20on%20Tableau,have%20up%20to%2010%20PATs) and use them in the Migration SDK configuration.

### The migration has finished but I do not see the expected content on the destination

When the migration finishes you get a [MigrationResult](xref:Tableau.Migration.MigrationResult). The [MigrationCompletionStatus](xref:Tableau.Migration.MigrationCompletionStatus) should be `Canceled` or `FatalError`. In the [Manifest](xref:Tableau.Migration.IMigrationManifest), check

- Errors: The top level errors not related to any manifest entries.
- [Entries](xref:Tableau.Migration.Engine.Manifest.IMigrationManifestEntryCollection): This is a collection of manifest entries that can be grouped by content type. Here are code snippets that log those errors. You can use this as general reference to process migration errors in your application.

C#

```c#
foreach (var type in ServerToCloudMigrationPipeline.ContentTypes)
{
    var contentType = type.ContentType;

    _logger.LogInformation($"## {contentType.Name} ##");

    // Manifest entries can be grouped based on content type.
    foreach (var entry in result.Manifest.Entries.ForContentType(contentType))
    {
        _logger.LogInformation($"{contentType.Name} {entry.Source.Location} Migration Status: {entry.Status}");

        if (entry.Errors.Any())
        {
            _logger.LogError($"## {contentType.Name} Errors detected! ##");
            foreach (var error in entry.Errors)
            {
                _logger.LogError(error, "Processing Error.");
            }
        }

        if (entry.Destination is not null)
        {
            _logger.LogInformation($"{contentType.Name} {entry.Source.Location} migrated to {entry.Destination.Location}");
        }
    }
}
```

Python

```python
    for type in ServerToCloudMigrationPipeline.ContentTypes:
        content_type = type.ContentType
        _logger.LogInformation(f"## {content_type.Name} ##")
        for entry in result.manifest.entries.ForContentType(content_type):
            _logger.LogInformation(f"{content_type.Name} {entry.Source.Location} Migration Status: {entry.Status}")
            if entry.Errors:
                _logger.LogError(f"## {content_type.Name} Errors detected! ##")
                for error in entry.Errors:
                    _logger.LogError(error, "Processing Error.")
            if entry.Destination is not None:
                _logger.LogInformation(f"{content_type.Name} {entry.Source.Location} migrated to {entry.Destination.Location}")
```

### Python - The SDK isn't loading the *.env* configuration

Environment variables must be set in the system the Python application runs in. This can be done through the OS itself, or by 3rd party libraries. The SDK will load the environment configuration on its **\_\_init\_\_** process.

For the case of the library [dotenv](https://pypi.org/project/python-dotenv/), it is required to execute the command **load_dotenv()** before referring to any **tableau_migration** code. 

```
# Used to load environment variables
from dotenv import load_dotenv  

# Load the environment variables before importing tableau_migration
load_dotenv()

# first tableau_migration reference
import tableau_migration

# The SDK will not recognize the .env file values
# Don't load the values here
# load_dotenv()
```

## Errors and Warnings

This section provides a list of potential error and warning log messages that you may encounter in the logs. Each entry includes a description to assist you in debugging.

### Warning: `Could not add a user to the destination Group [group name]. Reason: Could not find the destination user for [user name].`

This warning message indicates that the `GroupUsersTransformer` was unable to add the user, denoted as `[user name]`, to the group, denoted as `[group name]`.

This situation can occur if a user was excluded by a custom filter, but was not mapped to another user. If a custom filter was implemented based on the `ContentFilterBase<IUser>` class, then debug logging is already available.

To resolve this issue, enable debug logging to identify which filter is excluding the user. Then, add a mapping to an existing user using the `ContentMappingBase<IUser>` class.
