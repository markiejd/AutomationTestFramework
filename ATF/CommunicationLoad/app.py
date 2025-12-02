import base64
import logging
from flask import Flask, jsonify, request, send_from_directory
from flask_restx import Api, Resource, fields
from flask_cors import CORS
from auth import check_credentials
import json
import time
import subprocess
import os
import sys
import traceback
import requests
from bs4 import BeautifulSoup
from typing import Dict, Any

app = Flask(__name__)
URL = os.getenv('AZURE_DEVOPS_URL')
CORS(app)

# Create Api with swagger_ui enabled
api = Api(app, version='1.0.0', title='Locust Runner API', description='API for running Locust tests and managing configurations')

# Define namespaces
ns_locust = api.namespace('locust', description='Locust operations')
ns_config = api.namespace('config', description='Configuration operations')
ns_version = api.namespace('version', description='Get Version')

# Define models
config_model = api.model('Config', {
    'API': fields.String(required=True, description='API name'),
    'verify_ssl': fields.Boolean(required=True, description='Verify SSL'),
    'request': fields.String(required=True, description='Request type'),
    'api_key': fields.String(required=True, description='API key'),
    'user_number': fields.Integer(required=True, description='Number of users'),
    'increment_rate': fields.Integer(required=True, description='Increment rate'),
    'run_time': fields.String(required=True, description='Run time')
})

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

def checkcredentials(username, password):
     if check_credentials(username, password): return True
     return False

@ns_version.route('/Version')
class VersionResource(Resource):
       def get(self):
        try:
            # Azure DevOps API endpoint
            organization = os.getenv('AZURE_DEVOPS_ORG')
            project = os.getenv('AZURE_DEVOPS_PROJECT')
            repository = os.getenv('AZURE_DEVOPS_REPO')
            
            url = f"https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repository}/commits?api-version=6.0"
            
            # Personal Access Token for authentication
            pat = os.getenv('AZURE_DEVOPS_PAT')
            if not pat:
                return {"error": "AZURE_DEVOPS_PAT environment variable is not set"}, 500
            
            # Encode the PAT
            encoded_pat = base64.b64encode(f":{pat}".encode()).decode()
            
            # Headers for the request
            headers = {
                'Authorization': f'Basic {encoded_pat}',
                'Content-Type': 'application/json'
            }

            logger.info(f"Making request to Azure DevOps API: {url}")
            response = requests.get(url, headers=headers)
            
            logger.info(f"Response status code: {response.status_code}")
            logger.debug(f"Response headers: {response.headers}")
            logger.debug(f"Response content: {response.text[:200]}...")
            
            # Make the API request
            response = requests.get(url, headers=headers)
            response.raise_for_status()
            
            # Parse the response
            commits = response.json()['value']
            
            if commits:
                # Assuming the latest commit represents the current version
                latest_commit = commits[0]
                commit_id = latest_commit['commitId'][:7]  # Short commit hash
                commit_message = latest_commit['comment']
                
                # You might want to parse the commit message to extract a version number
                # For this example, we'll use the short commit hash as the version
                version = f"1.0.0-{commit_id}"
                
                return {
                    "version": version,
                    "commit_id": commit_id,
                    "commit_message": commit_message
                }, 200
            else:
                return {"error": "No commits found"}, 404
        
        except requests.RequestException as e:
            return {"error": f"Failed to fetch data from Azure DevOps: {str(e)}"}, 500
        except KeyError as e:
            return {"error": f"Unexpected response format: {str(e)}"}, 500
        except Exception as e:
            return {"error": f"An unexpected error occurred: {str(e)}"}, 500


@app.route('/static/<path:path>')
def send_static(path):
    return send_from_directory('static', path)

def parse_locust_html(html_content: str) -> Dict[str, Any]:
    """
    Parse Locust HTML report content and extract JSON data.
    Handles both dictionary and list formats of requests_statistics.
    
    Args:
        html_content: String containing the HTML report
        
    Returns:
        Dictionary containing the parsed test data
    """
    # Parse HTML
    soup = BeautifulSoup(html_content, 'html.parser')

    # Find the script tag containing the template args
    script_tags = soup.find_all('script')
    template_args = None
    
    for script in script_tags:
        if script.string and 'window.templateArgs = ' in script.string:
            # Extract the JSON data
            json_str = script.string.split('window.templateArgs = ')[1].split('\n')[0].strip()
            if json_str.endswith(';'):
                json_str = json_str[:-1]
            template_args = json.loads(json_str)
            break

    if not template_args:
        raise ValueError("Could not find template args in the HTML report")

    # Convert list format to dictionary if necessary
    requests_statistics = template_args.get('requests_statistics', {})
    if isinstance(requests_statistics, list):
        requests_statistics = {
            stat['name']: {
                'method': stat.get('method', ''),
                'num_requests': stat.get('num_requests', 0),
                'num_failures': stat.get('num_failures', 0),
                'min_response_time': stat.get('min_response_time', 0),
                'max_response_time': stat.get('max_response_time', 0),
                'avg_response_time': stat.get('avg_response_time', 0),
                'median_response_time': stat.get('median_response_time', 0),
                'response_time_percentile_0.95': stat.get('p95', 0),
                'response_time_percentile_0.99': stat.get('p99', 0),
                'current_rps': stat.get('current_rps', 0),
                'current_fail_per_sec': stat.get('current_fail_per_sec', 0),
                'avg_content_length': stat.get('avg_content_length', 0)
            }
            for stat in requests_statistics
            if stat.get('name') != 'Aggregated'
        }

    # Structure the data for API response
    return {
        'metadata': {
            'start_time': template_args.get('start_time', ''),
            'end_time': template_args.get('end_time', ''),
            'host': template_args.get('host', ''),
            'locustfile': template_args.get('locustfile', '')
        },
        'statistics': {
            'requests': {
                name: {
                    'method': stats.get('method', ''),
                    'num_requests': stats.get('num_requests', 0),
                    'num_failures': stats.get('num_failures', 0),
                    'response_times': {
                        'min': stats.get('min_response_time', 0),
                        'max': stats.get('max_response_time', 0),
                        'average': stats.get('avg_response_time', 0),
                        'median': stats.get('median_response_time', 0),
                        'p95': stats.get('response_time_percentile_0.95', 0),
                        'p99': stats.get('response_time_percentile_0.99', 0)
                    },
                    'performance': {
                        'current_rps': stats.get('current_rps', 0),
                        'current_fail_per_sec': stats.get('current_fail_per_sec', 0),
                        'avg_content_length': stats.get('avg_content_length', 0)
                    }
                }
                for name, stats in requests_statistics.items()
                if name != 'Aggregated'
            },
            'failures': [
                {
                    'endpoint': failure.get('name', ''),
                    'method': failure.get('method', ''),
                    'error': failure.get('error', ''),
                    'occurrences': failure.get('occurrences', 0)
                }
                for failure in template_args.get('failures_statistics', [])
            ],
            'exceptions': template_args.get('exceptions_statistics', [])
        },
        'history': [
            {
                'timestamp': entry.get('time', ''),
                'metrics': {
                    'users': entry.get('user_count', 0),
                    'rps': entry.get('current_rps', 0),
                    'failures_per_sec': entry.get('current_fail_per_sec', 0),
                    'avg_response_time': entry.get('total_avg_response_time', 0),
                    'p95_response_time': entry.get('response_time_percentile_0.95', 0)
                }
            }
            for entry in template_args.get('history', [])
        ]
    }

@ns_locust.route('/run')
class RunLocust(Resource):
    @api.doc(description='Run Locust test')
    @ns_locust.doc(params={'username': 'User name for authentication','password': 'Password for authentication', 'keyword': 'Word used to access APIs'})
    def get(self):

        username = request.args.get('username')
        password = request.args.get('password')
        keyword = request.args.get('keyword')
        keywordPass = "RunLocust"

        if not check_credentials(username, password):
            return {'message': 'Authentication failed'}, 401
            
        if keyword != keywordPass:
            return {'message': 'Authentication failed'}, 401
        
        try:
            python_executable = sys.executable
            current_directory = os.getcwd()
            locust_file_path = os.path.join(current_directory, "locustfile.py")
            results_file = "Results/report.html"
            results_path = os.path.join(current_directory, results_file)
            
            if not os.path.isfile(locust_file_path):
                api.abort(500, f"Locust file '{locust_file_path}' not found in the directory '{current_directory}'.")

            result = subprocess.run(
                [python_executable, locust_file_path],
                stdout=subprocess.PIPE,
                stderr=subprocess.PIPE,
                text=True
            )

            if result.returncode == 0:
                time.sleep(2)
                if os.path.exists(results_path):
                    with open(results_path, 'r', encoding='utf-8') as html_file:
                        html_content = html_file.read()
                    
                    # Parse the HTML and extract JSON data
                    try:
                        locust_results = parse_locust_html(html_content)
                        return {
                            "status": "success",
                            "message": "Locust test completed successfully",
                            "results": locust_results
                        }
                    except Exception as e:
                        api.abort(500, f"Failed to parse Locust results: {str(e)}")
                else:
                    api.abort(500, f"HTML report not found at {results_path}")
            else:
                api.abort(500, f"Locust command failed with return code {result.returncode}. Error: {result.stderr}")


        except Exception as e:
            traceback_message = traceback.format_exc()
            api.abort(500, f"An unexpected error occurred: {str(e)}\nTraceback: {traceback_message}")

@ns_config.route('/update')
class UpdateConfig(Resource):
    @api.expect(config_model)
    @api.doc(description='Update configuration')
    @ns_config.doc(params={'username': 'User name for authentication','password': 'Password for authentication', 'keyword': 'Word used to access APIs'})
    def post(self):
        CONFIG_PATH = 'wwwroot/Config/target_config.json'
        username = request.args.get('username')
        password = request.args.get('password')

        keyword = request.args.get('keyword')
        keywordPass = "UpdateConfig"

        if not check_credentials(username, password):
            return {'message': 'Authentication failed'}, 401
            
        if keyword != keywordPass:
            return {'message': 'Authentication failed'}, 401

        if not os.path.exists(CONFIG_PATH):
            os.makedirs(os.path.dirname(CONFIG_PATH), exist_ok=True)
            default_config = {
                "API": "PetStore",
                "verify_ssl": False,
                "request": "AddPet",
                "user_number": 1,
                "increment_rate": 1,
                "run_time": "10s",
            }
            with open(CONFIG_PATH, "w") as config_file:
                json.dump(default_config, config_file)

        try:
            new_config = request.json
            
            with open(CONFIG_PATH, 'w') as config_file:
                json.dump(new_config, config_file, indent=4)

            return {
                "status": "success",
                "message": "Configuration updated successfully"
            }
        except Exception as e:
            api.abort(500, f"An unexpected error occurred: {str(e)}")

@ns_config.route('/get')
class GetConfig(Resource):
    @api.doc(description='Get configuration')
    def get(self):
        CONFIG_PATH = 'wwwroot/Config/target_config.json'
        
        if not os.path.exists(CONFIG_PATH):
            return {'message': 'Configuration file not found'}, 404
        
        try:
            with open(CONFIG_PATH, 'r') as config_file:
                config_data = json.load(config_file)
                
            return {
                "status": "success",
                "data": config_data
            }
        except Exception as e:
            api.abort(500, f"An unexpected error occurred: {str(e)}")

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 8000))
    app.run(host='0.0.0.0', port=port, debug=True)