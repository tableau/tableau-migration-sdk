import sys
import os
from os.path import abspath
from pathlib import Path

_module_path = abspath(Path(__file__).parent.resolve().__str__() + "/../src/tableau_migration")
sys.path.append(_module_path)

if os.environ.get('MIG_SDK_PYTHON_BUILD', 'false') == 'true':
    # Not required for GitHub Actions
    import subprocess
    import shutil
    
    _bin_path = abspath(Path(__file__).parent.resolve().__str__() + "/../src/tableau_migration/bin")
    shutil.rmtree(_bin_path, True)
    
    _build_script = abspath(Path(__file__).parent.resolve().__str__() + "/../scripts/build-package.ps1")
    subprocess.run(["pwsh", "-c", _build_script, "-Fast", "-IncludeTests"])

from tableau_migration import clr
clr.AddReference("Moq")
clr.AddReference("Tableau.Migration.Tests")