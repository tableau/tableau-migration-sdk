## Errors and Warnings

This section provides a list of potential error and warning log messages that you may encounter in the logs. Each entry includes a description to assist you in debugging.

### Warning: `Group [group name] cannot map [user name]`

This warning message indicates that the `GroupUsersTransformer` was unable to add the user, denoted as `[user name]`, to the group, denoted as `[group name]`. 

This situation can occur if a user was excluded by a custom filter, but was not mapped to another user. If a custom filter was implemented based on the `ContentFilterBase<IUser>` class, then debug logging is already available. 

To resolve this issue, enable debug logging to identify which filter is excluding the user. Then, add a mapping to an existing user using the `ContentMappingBase<IUser>` class.
