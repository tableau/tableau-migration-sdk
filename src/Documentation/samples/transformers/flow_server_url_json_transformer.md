# Sample: Flow Connection Server URL JSON Transformer

This sample demonstrates how to read and write JSON to update server and serverUrl in the flow JSON file before publishing.
JSON transformers for flows should use the `JsonContentTransformerBase<IPublishableFlow>` base class, which handles opening the flow file, parsing the JSON, and persisting the modified JSON back to the file.

JSON transformers require additional resource overhead to execute, so care should be taken when developing them.

- Due to encryption, JSON transformers require the file to be loaded into memory to modify. For large flow files, this can require significant memory.
- Implementing the `NeedsJsonTransforming`/`needs_json_transforming` method can reduce overhead by letting the transformer skip loading flow files that do not require changes.

JSON transformers are provided with "raw" JSON and no file format validation is performed by the SDK.
Care should be taken when modifying flow files that the changes do not result in content that is valid JSON but invalid by the file format.
File format errors can lead to migration errors during publishing, and can also cause errors that are only apparent after the migration is complete and reported success.

## [Python](#tab/Python)

### Transformer Class

To update flow connection server values in Python, you can use the following transformer class:

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/transformers/flow_connection_server_json_transformer.py)]

### Registration

[//]: <> (Adding this as code as regions are not supported in python snippets)

```Python
plan_builder.transformers.add(FlowConnectionServerJsonTransformer)
```

See [hook registration](~/samples/index.md?tabs=Python#hook-registration) for more details.

## [C#](#tab/CSharp)

### Transformer Class

To update placeholder server values in flow connection attributes and flow nodes in C#, you can use the following transformer class:

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Transformers/FlowConnectionServerJsonTransformer.cs#class)]

### Registration

To register the transformer in C#, follow the guidance provided in the [documentation](~/samples/index.md?tabs=CSharp#hook-registration).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#FlowConnectionServerJsonTransformer-Registration)]

### Dependency Injection

Learn more about dependency injection [here](~/articles/dependency_injection/index.md).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#FlowConnectionServerJsonTransformer-DI)]

---
