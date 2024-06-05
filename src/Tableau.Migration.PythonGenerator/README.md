# Migration SDK Python Generator

## About

The Migration SDK Python generator creates wrapper code for a selection of C# types.
A custom generator is used due to the bespoke nature of the Python wrappers.
The generated code is designed to work with a subset of types and allow for manual wrapping of types that are difficult to wrap or pre-date the generator.

## Usage

The generator automatically runs when built, and is designed to output as part of local and scripted builds.
To add a new type to generate wrappers for, update the list in [`PythonGenerationList.cs`](PythonGenerationList.cs)

### Hints

The [`appsettings.json`](appsettings.json) file contains hints for the generator on a per-namespace/per-type basis:

- `excludedMembers` type hint: An array of strings for member names (methods or properties) not not generate wrappers for.

## Limitations

The current generator has the following limitations that would need to be addressed to generate code for types making use of these features:

- Overloaded methods
