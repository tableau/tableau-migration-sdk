# Manifest Analyzer

# Manifest Analyzer

The Manifest Analyzer is a tool designed to analyze migration manifests and report on unique errors encountered during the migration process. It is part of the Tableau Migration suite and helps users identify and understand issues that may arise when migrating content from a server to the cloud.

## Key Features

- **Manifest Loading**: The tool loads a migration manifest file specified in the configuration.
- **Error Analysis**: It analyzes the manifest entries for different content types and counts unique errors.
- **Error Reporting**: The tool generates a report of the unique errors and their occurrences, which is saved to a specified error file.

## Installation
From the project folder directly, run `dotnet publish -o <install folder>` to the folder you want ManifestAnalyzer to be in. You'll do this everytime it updates.

Once published, run the `OpenWithManifestAnalyzer.ps1` file to add this tool to the right click menu. You only need to do this once.

## Usage

1. **Configuration**: Ensure that the configuration file includes the paths for the manifest file and the error file.
2. **Execution**: Run the Manifest Analyzer. It will load the manifest, analyze the errors, and generate a report. Easiest is to right click on a manifest.json file and click `Open with Manifest Analyzer`.
3. **Review Report**: Check the error file for a detailed report on the unique errors encountered during the migration.

## Configuration Options

- **ManifestPath** (optional): The path to the migration manifest file. By default this will be passed in.
- **ErrorFilePath** (optional): The path where the error report will be saved. By default this will be next to the manifest file.