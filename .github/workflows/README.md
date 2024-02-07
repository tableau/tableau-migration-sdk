# GitHub Workflows

Here we have defined the workflows used for CI/CD for this SDK.
The files here are:
- dotnetworkflow.yml: This file is a template that is used by the SDK workflow. This template has the following parameters:
  - runs-on-config (mandatory): indicates the runner that will run the workflow. Available runners [here](https://docs.github.com/en/actions/using-github-hosted-runners/about-github-hosted-runners).
  - dotnet-version (mandatory): indicates the dotnet version that will be installed on the runner. The available versions depends on the runners. Check it on the link above.
  - build-config (mandatory): indicates the configuration used to build/test the sdk.
  - run-tests (mandatory): indicates if the workflow will run the unit tests.
  - publish-package (mandatory): indicates if the workflow will publish the artifacts (packages).
- sdk-workflow.yml: This is the real workflow that will run on every PR or every push on the defined branches.

For any changes, you can use the following documentation ([GitHub Actions Syntax](https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions)) as a reference.