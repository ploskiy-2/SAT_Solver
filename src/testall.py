import subprocess
import os
import sys

test_dir = 'tests'
returncode = 0

print('Starting tests...')

for file in os.listdir(test_dir):
    if file.endswith('.txt'):
        print(f'starting: {file}')
        if (file=="9.txt"):
            status = subprocess.run(
                ['python3', 'test.py', os.path.join(test_dir, file)],
                capture_output=True,
                text=True
                )
            print(status.stdout.strip('\n'))
            if (status.returncode != 0):
                returncode = 1

sys.exit(returncode)