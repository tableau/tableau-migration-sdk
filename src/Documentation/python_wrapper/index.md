# Python Wrapper

The Migration SDK is written in .NET. It has a Python wrapper package that provides access to most of this functionality.

## Capabilities

With the Python Wrapper, you can:

- Provide basic [configuration](~/articles/configuration.md) values to the Migration SDK via the PlanBuilder.
- Set configuration options as described in [Configuration](~/articles/configuration.md) with environment variables.
- Configure Python [logging](~/articles/logging.md#python-support).
- Run a migration using the wrapper.
- Write [Python hooks](~/samples/python_hooks.md) (See [Hooks](~/articles/hooks/index.md) for an overview).

## Current limitations

There are advanced features of the Migration SDK that the Python Wrapper cannot currently access:

- Override `C#` classes and methods to change how the SDK works.

## Examples to get started

The following code samples are for writing a simple migration app using the Migration SDK. For details on configuring and customizing the Migration SDK to your specific needs, see [Articles](~/articles/intro.md) and [Code Samples](~/samples/intro.md).

### [Startup Script](#tab/startup)

[!code-python[](../../../examples/Python.ExampleApplication/Python.ExampleApplication.py)]

### [config.ini](#tab/config)

> [!Important]
> The values below should not be quoted. So ***no*** `'` or `"`.

[!code-ini[](../../../examples/Python.ExampleApplication/config.ini)]

### [requirements.txt](#tab/reqs)

[!code-text[](../../../examples/Python.ExampleApplication/requirements.txt#L3-)]

---
