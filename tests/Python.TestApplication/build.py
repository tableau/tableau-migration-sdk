# This is a testing script to check that nothing is really wrong.
import sys
import os

# Build the solution so we have the published binaries
from os.path import abspath
from pathlib import Path

# Build binaries and put them on the path
binPath = abspath(Path(__file__).parent.resolve().__str__() + "/bin")

if os.environ.get('MIG_SDK_PYTHON_BUILD', 'false') == 'true':
    # Not required for GitHub Actions
    import subprocess
    import shutil
    
    migrationProject = abspath("../../tests/Tableau.Migration.Tests/Tableau.Migration.Tests.csproj")
    testComponentProject = abspath("../../tests/Tableau.Migration.TestComponents/Tableau.Migration.TestComponents.csproj")
    shutil.rmtree(binPath, True)
    subprocess.run(["dotnet", "publish", migrationProject, "-o", binPath, "-f", "net7.0"])
    subprocess.run(["dotnet", "publish", testComponentProject, "-o", binPath, "-f", "net7.0"])

sys.path.append(binPath)