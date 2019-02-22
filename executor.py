from sys import argv, path
import importlib


def get_control_pipe(tenant_id, execution_id):
    return open(f"\\\\.\\pipe\\{tenant_id}_{execution_id}_control", "rt")

def get_results_pipe(tenant_id, execution_id):
    return open(f"\\\\.\\pipe\\{tenant_id}_{execution_id}_results", "wt")

def load_module(deployment_path, algorithm_name):
    # sys path is to be modified in order to resolve import of common lib within
    # the algorithm
    path.append(deployment_path)
    return importlib.import_module(f'{algorithm_name}.main')


if __name__ == "__main__":
    tenant_id = argv[1]
    execution_id = argv[2]
    with get_control_pipe(tenant_id, execution_id) as control_pipe:
        deployment_path = control_pipe.readline().rstrip()
        algorithm_name = control_pipe.readline().rstrip()
        execution_parameters = control_pipe.readline().rstrip()

    algorithm_module = load_module(deployment_path, algorithm_name)
    results = algorithm_module.main(execution_parameters)
    with get_results_pipe(tenant_id, execution_id) as results_pipe:
        for result in results:
            results_pipe.write(result + "\n")
