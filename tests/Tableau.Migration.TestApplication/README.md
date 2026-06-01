# Server to Cloud Migration SDK - Test Application

Console application used to validate Migration SDK server-to-cloud scenarios.

## Running non-interactively

From repository root, run with required flag:

```powershell
dotnet run --project "tests/Tableau.Migration.TestApplication/Tableau.Migration.TestApplication.csproj" -- credential-migration-enabled
```

To also force reuse of an existing manifest:

```powershell
dotnet run --project "tests/Tableau.Migration.TestApplication/Tableau.Migration.TestApplication.csproj" -- credential-migration-enabled use-existing-manifest
```

### Command-line flags

- `credential-migration-enabled`  
  Required for non-interactive runs to proceed past the TSM credential gate.
- `use-existing-manifest`  
  Automatically uses a previously saved manifest if present.

After the run, check generated logs and manifest files in your configured output locations.

## Logging

[Serilog](https://github.com/serilog/serilog) is used as the logging provider. Console and file logging are enabled by default.

### Logging configuration

Logging configuration is wired in `Program.cs` through `services.ConfigureLogging(...)`.
