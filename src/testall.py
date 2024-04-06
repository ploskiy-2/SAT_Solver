import subprocess
import os
import sys

test_dir = 'tests'
returncode = 0

print("Starting tests...")

for file in os.listdir(test_dir):
    if file.endswith('.txt'):
        #print(f"Running test: {file}")
        status = subprocess.run(
            ["python", "test.py", os.path.join(test_dir, file)],
            capture_output=True,
            text=True
        )
        if status.returncode != 0:
            returncode = 1 
print("Finish tests")
sys.exit(returncode)
