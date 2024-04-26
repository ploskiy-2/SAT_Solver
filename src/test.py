import sys
import subprocess
import tempfile

sat = "s SATISFIABLE"
unsat = "s UNSATISFIABLE"


def dotnet_test(path):
    res = subprocess.run(
        ["dotnet", "run", "--project", "src", "--configuration", "Release", "--", path],
        capture_output=True,
        text=True,
    )
    with tempfile.NamedTemporaryFile("w+") as tmp_file:
        tmp_file.write(res.stdout)
        tmp_file.seek(0)
        status = tmp_file.readline().strip()
        print(status)
        if status == unsat:
            picosat_test = subprocess.run(
                ["picosat", path], capture_output=True, universal_newlines=True
            )
            if picosat_test.stdout.strip() == unsat:
                print("test for file ", path, " PASSED")
                sys.exit(0)
            else:
                print("test for file ", path, " FAILED")
                sys.exit(1)
        else:
            solution = tmp_file.readline().strip()[2:]
            with open(path, "r") as orig_file:
                with tempfile.NamedTemporaryFile("w+") as tmp_add_file:
                    for line in orig_file:
                        if line[0] == "c":
                            continue
                        elif line[0] == "p":
                            subs = line.strip().split()
                            num_var = int(subs[2])
                            num_clause = int(subs[3])
                            tmp_add_file.write(
                                " ".join(subs[:3])
                                + " "
                                + str(num_var + num_clause)
                                + "\n"
                            )
                        else:
                            tmp_add_file.write(line.strip("\n") + "\n")
                    # add new clauses
                    a = solution.split()
                    for i in range(len(solution.split())):
                        if a[i] == "0":
                            break
                        if a[i + 1] == "0":
                            tmp_add_file.write(a[i] + " 0")
                        else:
                            tmp_add_file.write(a[i] + " 0\n")

                    tmp_add_file.flush()
                    tmp_add_file.seek(0)
                    picosat_test = subprocess.run(
                        ["picosat", tmp_add_file.name],
                        capture_output=True,
                        universal_newlines=True,
                    )
                    if picosat_test.stdout.split("\n")[0].strip() == sat:
                        print("test for file ", path, " PASSED")
                        sys.exit(0)
                    else:
                        print("test for file ", path, " FAILED")
                        sys.exit(1)


if __name__ == "__main__":
    dotnet_test(sys.argv[1])
    sys.exit(0)
