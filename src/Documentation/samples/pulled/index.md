# Pulled Hooks

Pulled hooks allow you to inspect items after they have been fully retrieved and downloaded, but before they have been potentially converted or prepared for publishing.
This allows for a second round of [filtering](~/articles/hooks/filter_cascading.md) that requires information such as file content not available to the primary filter hook.

The following samples cover some common scenarios:

- [Sample: Filter Data Source Connections](~/samples/pulled/filter_data_source_connections.md)