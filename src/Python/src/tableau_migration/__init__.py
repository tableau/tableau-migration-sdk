"""Tableau Migration SDK package that wraps the dotnet implementation."""
import sys
from pathlib import Path
from os.path import abspath

# Append current file path to PATH
sys.path.append(Path(__file__).parent.resolve().__str__())

_bin_path = abspath(Path(__file__).parent.resolve().__str__() + "/bin")
sys.path.append(_bin_path)

# noqu: E402 stops the linter from flagging global imports that are not at the top of the file
# The python net and dotnet import must happen in this order else they won't load correctly
# See: https://pythonnet.github.io/pythonnet/python.html#loading-a-runtime

# Load python net and the clr
from pythonnet import load  # noqa: E402 
# load must be called before import clr
load("coreclr")
import clr  # noqa: E402

clr.AddReference("Tableau.Migration")
clr.AddReference("Microsoft.Extensions.DependencyInjection")

import tableau_migration.migration # noqa: E402 
_services = None
_service_collection = None
_logger_names = [] # List of logger names

tableau_migration.migration._initialize()

# Create out global default cancellation token
from System.Threading import CancellationTokenSource # noqa: E402 
_cancellation_token_source = CancellationTokenSource()
cancellation_token = _cancellation_token_source.Token

# Friendly renames for common top-level imports
from tableau_migration.migration import PyMigrationCompletionStatus as MigrationCompletionStatus # noqa: E402,F401
from tableau_migration.migration_engine import PyMigrationPlanBuilder as MigrationPlanBuilder # noqa: E402,F401
from tableau_migration.migration_engine_migrators import PyMigrator as Migrator # noqa: E402,F401
