# GitHub Workflows

This directory contains GitHub Actions workflows that automate the build, test, and deployment processes for the Migration SDK.

More info can be found in [Migration SDK Release Process for Package Repositories (Binaries)](https://docs.google.com/document/d/1XD68jhh0WnIOeLbxS6-xrhaYCNe18fGZakrfOz886cM/edit?tab=t.0).

## TL;DR

For production releases:

- Packages are built, tested, and deployed to **staging** environment
- Deployment to production requires approval through GitHub Environments
- When manual approval is provided, deployment to **production** environment happens

When code is merged to main/feature/staging/release branches:

- Packages are built and versioned
- Tests and linting are run
- Packages are automatically deployed to the **development** environment

When a PR is opened:

- Packages are built and versioned
- Tests and linting are run
- No deployment occurs

When sdk-workflow is started manually:

- Packages are built and versioned
- Tests and linting are run
- Packages are automatically deployed to the **staging** environment

## Overview

The workflows are organized hierarchically with a main workflow orchestrating various specialized workflows:

- **sdk-workflow.yml**: The main orchestrator workflow that coordinates all other workflows
- **build-*.yml**: Workflows for building different components (.NET, Python, docs)
- **test-*.yml**: Workflows for running tests for different components
- **lint-*.yml**: Workflows for linting code
- **deploy-*.yml**: Workflows for deploying to different environments
- **create-release.yml**: Workflow for creating GitHub releases

## Main Workflow

The `sdk-workflow.yml` workflow is the entry point that coordinates all other workflows. It runs on:

- Push to main, release, staging, and feature branches
- Pull requests (opened, synchronized, reopened, or ready for review)
- Manual workflow dispatch

The workflow includes:

1. **Define Versions**: Generates version numbers for packages
2. **Build**: Builds .NET, Python, and documentation
3. **Lint**: Runs code style checks
4. **Test**: Runs tests for all components
5. **Deploy**: Deploys to development, staging, or production environments based on triggers

## Deployment Environments

The SDK supports the following deployment environments:

- **Development**: Automatic deployment on pushes or beta manual triggers
- **Staging**: Manual deployment with Production release type or beta-to-production intent
- **Production**: Manual deployment after approval from staging with proper release flags

## .NET Version Support and Updates

### Current Requirements

The Migration SDK workflows support and test multiple .NET versions:

- **Build/Deployment**: Requires .NET 9.0.x (as specified in `global.json`)
- **Runtime Compatibility**: Supports .NET 8.0+ and .NET 9.0+ for end users
- **Testing**: Runs compatibility tests on both .NET 8.0.x and .NET 9.0.x via matrix strategy

### GitHub Actions Configuration

The workflows use centralized version management:

- **Build Version**: Set via `DOTNET_BUILD_VERSION: '9.0.x'` in `sdk-workflow.yml`
- **Test Matrix**: Set via `BUILD_DOTNET_VERSIONS` variable (e.g., `['8.0.x', '9.0.x']`) used by both `test-dotnet.yml` and `test-python.yml`
- **Version Mismatch Handling**: Uses `DOTNET_ROLL_FORWARD: LatestMajor` during package restore

### Updating for New .NET Versions

When a new major version of .NET is released, update these workflow files:

#### Required Updates

1. **`global.json`** - Update the SDK version to the latest LTS or Current version
2. **`sdk-workflow.yml`** - Update `DOTNET_BUILD_VERSION` environment variable
3. **Workflow Variables** - Update `BUILD_DOTNET_VERSIONS` variable to include new version (e.g., `['8.0.x', '9.0.x', '10.0.x']`), in test-dotnet and test-python.
4. **Project files** - Add new target framework monikers where appropriate

#### Example for .NET 10 (when released)

```yaml
# In sdk-workflow.yml
env:
  DOTNET_BUILD_VERSION: '10.0.x'  # Update build version
```

```json
// In global.json
{
  "sdk": {
    "version": "10.0.100",  // Update to latest SDK
    "rollForward": "latestMajor"
  }
}
```

Workflow Variables:

- Update `BUILD_DOTNET_VERSIONS` to `['8.0.x', '9.0.x', '10.0.x']`

### Compatibility Testing Strategy

The workflows use several strategies to ensure .NET version compatibility:

- **Matrix Testing**: Tests run on multiple .NET versions simultaneously using framework targeting:
  - **.NET 8 Matrix**: Installs both .NET 8 and .NET 9 SDKs, tests solution with `--framework net8.0` targeting
  - **.NET 9 Matrix**: Installs .NET 9 SDK, tests solution with `--framework net9.0` targeting
- **Cross-Platform**: Tests run on Windows, macOS, and Linux runners
- **Version Rollforward**: Uses `DOTNET_ROLL_FORWARD: LatestMajor` to handle version mismatches during package restore
- **Artifact Compatibility**: Build artifacts use the build version but are tested against all matrix versions
- **Clean Environment**: Each test run starts with a completely clean .NET environment using two steps: first the `clean-dotnet` action removes all existing .NET installations, then `setup-dotnet` installs the target version. This eliminates conflicts with global.json or other version requirements across all platforms.

#### .NET Version-Specific Testing

**.NET 8.0.x Matrix Behavior:**

- ✅ Clean .NET installation and install .NET 8 runtime
- ✅ Install .NET 9 SDK for build compatibility (global.json requirements)
- ✅ Restore dependencies and tools (using .NET 9 SDK)
- ✅ Test solution with `--framework net8.0` targeting (tests .NET 8 assemblies)
- ✅ Collect code coverage with XPlat Code Coverage
- ✅ Generate coverage reports with reportgenerator
- ✅ Upload test results and coverage artifacts

**.NET 9.0.x Matrix Behavior:**

- ✅ Clean .NET installation and install .NET 9 SDK
- ✅ Restore dependencies and tools
- ✅ Test solution with `--framework net9.0` targeting (tests .NET 9 assemblies)
- ✅ Collect code coverage with XPlat Code Coverage
- ✅ Generate coverage reports with reportgenerator
- ✅ Upload test results and coverage artifacts

This approach ensures both versions get **full testing capabilities** while verifying true **runtime compatibility** through framework targeting.

## Custom Actions

The workflows use several custom actions located in `.github/actions/`:

### setup-dotnet

Standard .NET SDK setup with verification and caching.

### clean-dotnet

Removes all existing .NET installations from the system across all platforms:

- **Linux**: Removes via `apt-get purge` and deletes installation directories (`/usr/share/dotnet`, `/etc/dotnet`, `~/.dotnet`)
- **macOS**: Removes installation directories (`/usr/local/share/dotnet`, `~/.dotnet`) and brew packages
- **Windows**: Uses PowerShell to uninstall packages and remove directories (`C:\Program Files\dotnet`, etc.)

This action only performs cleanup and does not install any .NET versions. Use in combination with `setup-dotnet` for a clean installation environment.

## Testing with Act

[Act](https://github.com/nektos/act) allows you to run GitHub Actions workflows locally for testing. To use Act with these workflows:

### Running Workflows Locally

#### Testing the Main Workflow

```bash
# Test the main workflow on push event
act push -W .github/workflows/sdk-workflow.yml

# Test with workflow_dispatch event (manual trigger with inputs)
act workflow_dispatch -W .github/workflows/sdk-workflow.yml --input release-type=Beta --input intent-beta-to-prod=false
```

#### Testing Individual Workflows

```bash
# Test building .NET
act workflow_call -W .github/workflows/build-dotnet.yml --input beta-version=0.1.0-beta123.post1 --input runs-on-config=ubuntu-latest

# Test running tests
act workflow_call -W .github/workflows/test-dotnet.yml
```

### Act Configuration

For best results, create an `.actrc` file in your repository root:

```bash
-P ubuntu-latest=ghcr.io/catthehacker/ubuntu:full-22.04
--artifact-server-path=<Local Path to a temp folder for storing artifacts>
```

`ubuntu:full` is required for building the docs.

### Environment and Secrets Configuration

When testing with Act, you'll need to provide environment variables and secrets that would normally be available in GitHub. The repository includes template files for this purpose:

1. **Environment Variables**: Copy `.github/.vars.default` to `.github/.vars`

   ```bash
   cp .github/.vars.default .github/.vars
   ```

2. **Secrets**: Copy `.github/.secrets.default` to `.github/.secrets`

   ```bash
   cp .github/.secrets.default .github/.secrets
   ```

3. Edit `.github/.secrets` to add your actual secret values:

   ```bash
   NUGET_PUBLISH_API_KEY=your-nuget-api-key
   PYPI_PUBLISH_USER_PASS=your-pypi-password
   ```

4. Run Act with these files:

   ```bash
   act push -W .github/workflows/sdk-workflow.yml --secret-file .github/.secrets --var-file .github/.vars
   ```

The `.vars.default` file includes settings for different environments (development, staging, and production). Uncomment the appropriate section based on which environment you want to test.

### Using the VS Code Extension

For a more user-friendly experience, you can use the [GitHub Local Actions VS Code extension](https://marketplace.visualstudio.com/items?itemName=github-actions.github-actions-localhost):

1. Install the extension from the VS Code marketplace
2. Open the command palette (Ctrl+Shift+P) and search for "GitHub Actions: Run Locally"
3. Select the workflow you want to run
4. Configure your environment variables and secrets in the extension interface
   - From the steps above, after creating local .vars and .secrets file, Use "Locate Variable File" and "Locate Secret File" to find them. More details here: https://sanjulaganepola.github.io/github-local-actions-docs/usage/settings/
   - Once you've added your local .vars and .secrets file, remember to CHECK THE BOX or else it won't pick it up.
5. You need to add in a artifacts-server-path field under Options (This is for the upload parts. Go to  `Settings` -> `Options` -> `