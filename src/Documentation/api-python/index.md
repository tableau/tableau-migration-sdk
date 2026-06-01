# Introduction

This section is the **Python API Reference** for the Migration SDK. It documents the types and functions in the Python wrapper package you use when building migration applications in Python.

Below you’ll find requirements, installation, capabilities, and a minimal getting-started example. For concepts, configuration, and more samples, see [Articles](~/articles/index.md) and [Code Samples](~/samples/index.md).

## Prerequisites

[!include[](~/includes/python-prerequisites.html)]

## Hardware requirements

The Migration SDK downloads copies of content items onto the machine that the Migration SDK is installed on. Make sure there's enough disk space on the machine to sequentially download content during the migration process.

## Installation

To use the Migration SDK for Python, download the [Python package](https://pypi.org/project/tableau-migration/). For information about installing Python packages, see [Installing Packages](https://packaging.python.org/en/latest/tutorials/installing-packages/) in the Python documentation.

## Capabilities

With the Python API, you can:

- Provide basic [configuration](~/articles/configuration/index.md) values to the Migration SDK via the PlanBuilder.
- Set configuration options as described in [Configuration](~/articles/configuration/index.md) with environment variables.
- Configure Python [logging](~/articles/logging.md?tabs=Python).
- Run a migration using python.
- Write Python hooks. See [Custom Hooks](~/articles/hooks/index.md) for an overview.

## Current limitations

There are advanced features of the Migration SDK that the Python Wrapper cannot currently access:

- Override `C#` classes and methods to change how the SDK works.

## Examples to get started

The following code samples are for writing a simple migration app using the Migration SDK. For details on configuring and customizing the Migration SDK to your specific needs, see [Articles](~/articles/index.md) and [Code Samples](~/samples/index.md).

[!include[](~/includes/python-getting-started.md)]
