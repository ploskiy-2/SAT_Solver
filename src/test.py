import subprocess
import tempfile
import sys

sat_msg = 's SATISFIABLE'
unsat_msg = 's UNSATISFIABLE'

in_file_name = sys.argv[1]
result = subprocess.run(
    ['dotnet', 'run', in_file_name],
    capture_output=True,
    universal_newlines=True
)

tmp = tempfile.NamedTemporaryFile()
with open(tmp.name, 'w') as f:
    f.write(result.stdout)
tmp.flush()
with open(tmp.name, 'r') as f:
    status = f.readline().strip() #/tmp/tmpj5db4cmy:20: expected number
    if status == unsat_msg: 
        picosat_status = subprocess.run(
            ['picosat', in_file_name],
            capture_output=True,
            universal_newlines=True
        )
        if picosat_status.stdout.strip() == unsat_msg:
            print(f'{in_file_name}: PASSED')
            sys.exit(0)
        else:
            print(f'{in_file_name}: FAILED')
            sys.exit(1)
    else:
        solution = f.readline().strip()[2:]
        tmp_in_file = tempfile.NamedTemporaryFile()
        with open(tmp_in_file.name, 'w') as f_in_tmp:
            with open(in_file_name, 'r') as f_in:
                for line in f_in:
                    if line.startswith('c'):
                        continue
                    elif line.startswith('p'):
                        splitted = line.split()
                        new_line = ' '.join(splitted[:3]) + ' ' + \
                            str(int(splitted[2]) + int(splitted[3]))
                    else:
                        new_line = line.strip()
                    f_in_tmp.write(new_line + '\n')
            for literal in solution.split():
                if literal == '0':
                    break
                f_in_tmp.write(literal + ' 0\n')
        tmp_in_file.flush()
        
        picosat_status = subprocess.run(
            ['picosat', tmp_in_file.name],
            capture_output=True,
            universal_newlines=True
        )
        if picosat_status.stdout.strip().split('\n')[0] == sat_msg:
            print(f'{in_file_name}: PASSED')
            sys.exit(0)
        else:
            print(f'{in_file_name}: FAILED')
            print(picosat_status.stdout.strip().split('\n')[0])
            sys.exit(1)
