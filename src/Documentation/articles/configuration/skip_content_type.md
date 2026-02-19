# Skipping Content Types

Migration SDK allows skipping all items of a content type through use of the migration [plan builder](~/articles/configuration/index.md#basic-configuration).
When one or more items of a content type should migrate, [filters](~/articles/hooks/index.md) should be used, instead.

### [Python](#tab/python)

[//]: <> (Adding this as code so we don't change example application default behavior.)
```Python
plan_builder.skip_content_type(IUser)
```

### [C#](#tab/csharp)

[//]: <> (Adding this as code so we don't change example application default behavior.)
```C#
planBuilder.SkipContentType<IUser>();
```

---

## Pre-Caching
The pre-caching option controls the [data loading](~/articles/configuration/data_loading.md) behavior of the content type, which impacts how data is retrieved when items of other content types reference the skipped content type.
For most migrations the default value (enabled) is recommended, as it reduces the total number of API calls to minimize API throttling regardless of how often the skipped content type is referenced.

### Pre-Caching Enabled
When pre-caching is enabled (default), this is equivalent to registering a filter for the content type that skips all items.
The default [data loading](~/articles/configuration/data_loading.md) behavior is used.

### Pre-Caching Disabled
When pre-caching is disabled, a filter is registered to skip all items, but additional changes to [data loading](~/articles/configuration/data_loading.md) are configured for the content type.
Disabling pre-caching is recommended only when the skipped content type has a very large number of items, and only a few of those items are referenced by the rest of the migrating content types.
When pre-caching is disabled:

- A [content loader](~/articles/configuration/data_loading.md#migration-content-loader) is registered for the content type that returns zero items.
- The [content reference cache load strategy](~/articles/configuration/data_loading.md#content-reference-loading) for the content type is configured to search for items individually when possible.