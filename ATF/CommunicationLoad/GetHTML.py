from bs4 import BeautifulSoup
import json
import datetime
from typing import Dict, List, Any

def parse_locust_report(file_path: str) -> Dict[str, Any]:
    """
    Parse a Locust HTML report and extract all available test statistics and data.
    
    Args:
        file_path: Path to the Locust HTML report file
        
    Returns:
        Dictionary containing the complete parsed test data and statistics
    """
    # Read the HTML file
    with open(file_path, 'r', encoding='utf-8') as file:
        html_content = file.read()

    # Parse HTML
    soup = BeautifulSoup(html_content, 'html.parser')

    # Find the script tag containing the template args
    script_tags = soup.find_all('script')
    template_args = None
    theme = None
    
    for script in script_tags:
        if script.string:
            if 'window.templateArgs = ' in script.string:
                # Extract the JSON data
                json_str = script.string.split('window.templateArgs = ')[1].split('\n')[0].strip()
                if json_str.endswith(';'):
                    json_str = json_str[:-1]
                template_args = json.loads(json_str)
            elif 'window.theme = ' in script.string:
                theme = script.string.split('window.theme = ')[1].strip().strip('"').strip("'").strip(';')

    if not template_args:
        raise ValueError("Could not find template args in the HTML report")

    # Extract ALL available information
    test_data = {
        'metadata': {
            'start_time': template_args['start_time'],
            'end_time': template_args['end_time'],
            'host': template_args['host'],
            'locustfile': template_args['locustfile'],
            'theme': theme,
            'show_download_link': template_args['show_download_link'],
            'is_report': template_args['is_report']
        },
        
        # Task distribution data
        'tasks': {
            'per_class': template_args['tasks']['per_class'],
            'total': template_args['tasks']['total']
        },
        
        # Complete request statistics
        'requests_statistics': {
            req['name']: {
                'method': req['method'],
                'num_requests': req['num_requests'],
                'num_failures': req['num_failures'],
                'avg_response_time': req['avg_response_time'],
                'min_response_time': req['min_response_time'],
                'max_response_time': req['max_response_time'],
                'median_response_time': req['median_response_time'],
                'avg_content_length': req['avg_content_length'],
                'current_rps': req['current_rps'],
                'current_fail_per_sec': req['current_fail_per_sec'],
                'response_time_percentiles': {
                    '95': req['response_time_percentile_0.95'],
                    '99': req['response_time_percentile_0.99']
                },
                'safe_name': req['safe_name']
            }
            for req in template_args['requests_statistics']
        },
        
        # Detailed response time distribution
        'response_time_statistics': {
            stat['name']: {
                'method': stat['method'],
                'percentiles': {
                    '50': stat['0.5'],
                    '60': stat['0.6'],
                    '70': stat['0.7'],
                    '80': stat['0.8'],
                    '90': stat['0.9'],
                    '95': stat['0.95'],
                    '99': stat['0.99'],
                    '100': stat['1.0']
                }
            }
            for stat in template_args['response_time_statistics']
        },
        
        # Failure statistics
        'failures_statistics': [
            {
                'name': failure['name'],
                'method': failure['method'],
                'error': failure['error'],
                'occurrences': failure['occurrences']
            }
            for failure in template_args['failures_statistics']
        ],
        
        # Exception statistics (if any)
        'exceptions_statistics': template_args['exceptions_statistics'],
        
        # Time-series history data
        'history': [
            {
                'time': entry['time'],
                'user_count': entry['user_count'],
                'current_rps': entry['current_rps'],
                'current_fail_per_sec': entry['current_fail_per_sec'],
                'total_avg_response_time': entry['total_avg_response_time'],
                'response_time_percentile_0.95': entry['response_time_percentile_0.95']
            }
            for entry in template_args['history']
        ],
        
        # Percentiles configured for charting
        'percentiles_to_chart': template_args['percentiles_to_chart']
    }

    return test_data

def print_detailed_summary(test_data: Dict[str, Any]) -> None:
    """
    Print a comprehensive summary of the test results including all available metrics.
    
    Args:
        test_data: Dictionary containing the parsed test data
    """
    print("Locust Load Test Detailed Summary")
    print("=================================")
    
    # Metadata
    print("\nTest Metadata:")
    print(f"Start Time: {test_data['metadata']['start_time']}")
    print(f"End Time: {test_data['metadata']['end_time']}")
    print(f"Host: {test_data['metadata']['host']}")
    print(f"Locust File: {test_data['metadata']['locustfile']}")
    
    # Task Distribution
    print("\nTask Distribution:")
    for user_class, data in test_data['tasks']['per_class'].items():
        print(f"\nUser Class: {user_class}")
        print(f"Ratio: {data['ratio']}")
        for task_set, task_data in data['tasks'].items():
            print(f"  Task Set: {task_set}")
            print(f"  Tasks:")
            for task, task_info in task_data['tasks'].items():
                print(f"    - {task}: {task_info['ratio']}")
    
    # Request Statistics
    print("\nRequest Statistics:")
    for endpoint, stats in test_data['requests_statistics'].items():
        if endpoint != "Aggregated":
            print(f"\n{stats['method']} {endpoint}")
            print(f"  Total Requests: {stats['num_requests']}")
            print(f"  Failures: {stats['num_failures']}")
            print(f"  Current RPS: {stats['current_rps']}")
            print(f"  Response Times (ms):")
            print(f"    Min: {stats['min_response_time']}")
            print(f"    Max: {stats['max_response_time']}")
            print(f"    Avg: {stats['avg_response_time']:.2f}")
            print(f"    Median: {stats['median_response_time']}")
            print(f"    P95: {stats['response_time_percentiles']['95']}")
            print(f"    P99: {stats['response_time_percentiles']['99']}")
            print(f"  Avg Content Length: {stats['avg_content_length']:.2f}")
    
    # Failure Details
    if test_data['failures_statistics']:
        print("\nFailure Details:")
        for failure in test_data['failures_statistics']:
            print(f"\n{failure['method']} {failure['name']}")
            print(f"  Error: {failure['error']}")
            print(f"  Occurrences: {failure['occurrences']}")
    
    # Exception Details
    if test_data['exceptions_statistics']:
        print("\nException Details:")
        for exception in test_data['exceptions_statistics']:
            print(f"\nException: {exception}")

if __name__ == "__main__":
    # Example usage
    test_data = parse_locust_report('report.html')
    print_detailed_summary(test_data)