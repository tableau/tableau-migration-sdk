# Introduction

Welcome to the Python API Reference for the Migration SDK.

The Migration SDK is written in .NET. It has a Python wrapper package that provides access to most of this functionality.

## Capabilities

With the Python API, you can:

- Provide basic [configuration](~/articles/configuration.md) values to the Migration SDK via the PlanBuilder.
- Set configuration options as described in [Configuration](~/articles/configuration.md) with environment variables.
- Configure Python [logging](~/articles/logging.md#python-support).
- Run a migration using python.
- Write Python hooks. See [Custom Hooks](~/articles/hooks/index.md) for an overview.

## Current limitations

There are advanced features of the Migration SDK that the Python Wrapper cannot currently access:

- Override `C#` classes and methods to change how the SDK works.

## Examples to get started

The following code samples are for writing a simple migration app using the Migration SDK. For details on configuring and customizing the Migration SDK to your specific needs, see [Articles](~/articles/index.md) and [Code Samples](~/samples/index.md).

[!include[](~/includes/python-getting-started.md)]
