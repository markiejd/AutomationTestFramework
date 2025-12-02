import json
import os
import logging
import time
from locust import HttpUser, task, between, events

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

TARGET_CONFIG_FILE = 'config/target_config.json'

class APIUser(HttpUser):
    wait_time = between(1, 3)

    def on_start(self):
        logger.info("Starting user...")
        self.load_target_config()
        api_name = self.config.get("API")
        if not api_name:
            logger.error("API not specified in the target config.")
            return
        self.load_api_config(api_name)

    def load_target_config(self):
        logger.info(f"Loading target config from {TARGET_CONFIG_FILE}")
        with open(TARGET_CONFIG_FILE, 'r') as f:
            self.config = json.load(f)
        self.client.verify = self.config.get('verify_ssl', True)
        logger.info(f"Loaded target config: {self.config}")

    def load_api_config(self, api_name):
        api_name_lower = api_name.lower()
        config_path = os.path.join('config/Apps', f"{api_name_lower}_config.json")

        if not os.path.exists(config_path):
            logger.error(f"Config file {config_path} does not exist.")
            return

        logger.info(f"Loading {api_name} config from {config_path}")
        with open(config_path, 'r') as f:
            self.requests_config = json.load(f)
        logger.info(f"Loaded {api_name} config: {self.requests_config}")

    @task
    def perform_request(self):
        request_name = self.config["request"]
        request_data = self.requests_config.get(request_name)
        logger.info(f"Performing request: {request_name}")
        self.process_request(request_data)

    def process_request(self, request_data):
        method = request_data['method']
        endpoint = request_data['endpoint']
        data = request_data.get('data', {})

        headers = request_data.get('headers', {})
        if self.config.get('api_key', False):
            api_key = os.getenv('API_KEY')
            if api_key:
                headers['api-key'] = f"{api_key}"
            else:
                logger.warning("API key is required but not found in environment variables.")

        logger.info(f"Sending {method} request to {endpoint} with headers: {headers} and data: {data}")

        try:
            if method == "GET":
                response = self.client.get(endpoint, headers=headers, params=data)
            elif method == "POST":
                response = self.client.post(endpoint, json=data, headers=headers)
            elif method == "PUT":
                response = self.client.put(endpoint, json=data, headers=headers)
            elif method == "DELETE":
                response = self.client.delete(endpoint, headers=headers)
            else:
                raise ValueError(f"Unsupported HTTP method: {method}")

            response.raise_for_status()
            logger.info(f"Request successful: {response.status_code}")
        except Exception as e:
            logger.error(f"Request failed: {str(e)}")

    def save_result(self, result):
        RESULTS_FILE = self.config["results"]
        if not os.path.exists(os.path.dirname(RESULTS_FILE)):
            os.makedirs(os.path.dirname(RESULTS_FILE))
        with open(RESULTS_FILE, 'a') as f:
            logger.info(f"Results {result}")
            f.write(json.dumps(result) + '\n')
        logger.info(f"Result saved to {RESULTS_FILE}")

def load_target_config():
    logger.info(f"Loading target config from {TARGET_CONFIG_FILE}")
    with open(TARGET_CONFIG_FILE, 'r') as f:
        return json.load(f)

def load_api_config(api_name):
    api_name_lower = api_name.lower()
    config_path = os.path.join('config/Apps', f"{api_name_lower}_config.json")

    if not os.path.exists(config_path):
        logger.error(f"Config file {config_path} does not exist.")
        return None

    logger.info(f"Loading {api_name} config from {config_path}")
    with open(config_path, 'r') as f:
        requests_config = json.load(f)
    logger.info(f"Loaded {api_name} config: {requests_config}")
    return requests_config

def on_locust_init(environment, **kwargs):
    logger.info("Initializing Locust environment...")
    target_config = load_target_config()
    
    api_name = target_config.get("API")
    if not api_name:
        logger.error("API not specified in the target config.")
        return

    requests_config = load_api_config(api_name)
    if requests_config is None:
        return

    environment.host = requests_config['Host']
    logger.info(f"Host set to: {environment.host}")

events.init.add_listener(on_locust_init)

# def on_locust_quit(environment, **kwargs):
#     stats = environment.stats.total
#     report = {
#         "num_requests": stats.num_requests,
#         "num_failures": stats.num_failures,
#         "avg_response_time": stats.avg_response_time,
#         "min_response_time": stats.min_response_time,
#         "max_response_time": stats.max_response_time,
#         "median_response_time": stats.median_response_time,
#         "percentile_95_response_time": stats.get_response_time_percentile(0.95),
#         "percentile_99_response_time": stats.get_response_time_percentile(0.99)
#     }
#     results_file = "Results/locust_report.json"
#     current_dir = os.getcwd()
#     results_path = os.path.join(current_dir, results_file)
#     os.makedirs(os.path.dirname(results_path), exist_ok=True)
#     with open(results_path, "w") as f:
#         json.dump(report, f)

#     logger.info(f"Locust report saved to {results_path}")

# events.quitting.add_listener(on_locust_quit)

if __name__ == "__main__":
    config = load_target_config()
    user_number = config["user_number"]
    increment_rate = config["increment_rate"]
    run_time = config["run_time"]

    html_file = os.getenv('LOCUST_REPORT_PATH', 'Results/report.html')
    current_dir = os.getcwd()
    html_path = os.path.join(current_dir, html_file)
    os.makedirs(os.path.dirname(html_path), exist_ok=True)
    print(f"HTML file created at: {html_path}")

    locust_command = f"python -m locust -f locustfile.py --headless -u {user_number} -r {increment_rate} --run-time {run_time} --html {html_path}"
    os.system(f"{locust_command} > {html_path}")