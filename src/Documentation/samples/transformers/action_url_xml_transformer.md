# Sample: Action URL XML Transformer

This sample demonstrates how to read and write XML to update the workbook files.
XML transformers should use the `XmlContentTransformerBase` base class that handles parsing the file XML and saving the modified XML back to the file to be published.

XML transformers require additional resource overhead to execute, so care should be taken when developing them.

- Due to encryption, XML transformers require the file to be loaded into memory to modify. For large files, such as those with large extracts, this can require significant memory.
- Python XML transformers each require extra processing as the parsed XML is converted from a .NET representation to a Python representation. For files with large XML content this can require significant time and memory. Combining multiple Python XML transformers can minimize this impact.

In general, resource overhead can be mimized by implementing the `NeedsXmlTransforming`/`needs_xml_transforming` method, which allows the transformer to use the metadata of the content item to determine whether the XML file needs to be loaded.

XML transformers are provided with "raw" XML and no file format validation is performed by the SDK.
Care should be taken when modifying workbook or other files that the changes do not result in content that is valid XML but invalid by the file format.
File format errors can lead to migration errors during publishing, and can also cause errors that are only apparent after the migration is complete and reported success.

## [Python](#tab/Python)

### Transformer Class

To update action URLs in Python, you can use the following transformer class:

[!code-python[](../../../../examples/Python.ExampleApplication/hooks/transformers/action_url_xml_transformer.py)]

### Registration

[//]: <> (Adding this as code as regions are not supported in python snippets)

```Python
plan_builder.transformers.add(ActionUrlXmlTransformer)
```

See [hook registration](~/samples/index.md?tabs=Python#hook-registration) for more details.

## [C#](#tab/CSharp)

### Transformer Class

In C#, the transformer class for adjusting action URLs is implemented as follows:

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Hooks/Transformers/ActionUrlXmlTransformer.cs#class)]

### Registration

To register the transformer in C#, follow the guidance provided in the [documentation](~/samples/index.md?tabs=CSharp#hook-registration).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/MyMigrationApplication.cs#ActionUrlXmlTransformer-Registration)]

### Dependency Injection

Learn more about dependency injection [here](~/articles/dependency_injection.md).

[!code-csharp[](../../../../examples/Csharp.ExampleApplication/Program.cs#ActionUrlXmlTransformer-DI)]