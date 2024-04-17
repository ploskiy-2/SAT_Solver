import subprocess
import os
import sys

test_dir = 'tests'
returncode = 0

for file in os.listdir(test_dir):
    if file.endswith('.txt'):
        status = subprocess.run(
            ["python", "test.py", os.path.join(test_dir, file)],
        )
        if status.returncode != 0:
            returncode = 1 
sys.exit(returncode)
