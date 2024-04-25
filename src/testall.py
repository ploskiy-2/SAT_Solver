import subprocess
import os
import sys

test_dir = "src/tests"
returncode = 0

print("Starting tests...")

for file in os.listdir(test_dir):
    if file.endswith(".txt"):
        file_path = os.path.join(test_dir, file)
        print(f"starting: {file}")
        status = subprocess.run(
            ["python3", "src/test.py", file_path], capture_output=True, text=True
        )
        print(status.stdout.strip("\n"))
        if status.returncode != 0:
            returncode = 1

sys.exit(returncode)
