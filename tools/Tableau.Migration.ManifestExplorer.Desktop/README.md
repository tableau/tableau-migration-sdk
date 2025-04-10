# Manifest Explorer

The Manifest Analyzer is a tool designed to let users view the manifest. It has the ability to filter the entries and to only view entries with errors. 

## Key Features

- **Manifest Loading**: Loading of the manifest is either done by pressing the `Load Manifest` button or by starting the application with the first command line parameter being the path to the manifest.

## Installation
From the project folder directly, run `dotnet publish -o <install folder> -f net9.0` to the folder you want ManifestExplorer to be in. You'll do this everytime it updates.

Once published, run the `OpenWithManifestExplorer.ps1` file to add this tool to the right click menu. You only need to do this once.
