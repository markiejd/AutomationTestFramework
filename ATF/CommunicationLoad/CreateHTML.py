import json
from datetime import datetime
from typing import Dict, Any

def generate_html_report(test_data: Dict[str, Any]) -> str:
    """
    Generate an HTML report from parsed Locust test data.
    
    Args:
        test_data: Dictionary containing the parsed test data
        
    Returns:
        String containing the complete HTML report
    """
    # Convert test data for JavaScript
    js_data = json.dumps(test_data)
    
    html_template = f"""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Locust Test Report</title>
    
    <!-- Include Chart.js -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/3.7.0/chart.min.js"></script>
    
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        
        .container {{
            max-width: 1200px;
            margin: 0 auto;
            background-color: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        
        .header {{
            text-align: center;
            margin-bottom: 30px;
        }}
        
        .grid {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }}
        
        .card {{
            background: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
        }}
        
        .stats-table {{
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 20px;
        }}
        
        .stats-table th, .stats-table td {{
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }}
        
        .stats-table th {{
            background-color: #f8f9fa;
        }}
        
        .chart-container {{
            position: relative;
            height: 300px;
            margin-bottom: 30px;
        }}
        
        .failure-card {{
            background-color: #fff5f5;
            border-left: 4px solid #f56565;
        }}
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>Locust Test Report</h1>
            <p>Test run from {test_data['metadata']['start_time']} to {test_data['metadata']['end_time']}</p>
            <p>Host: {test_data['metadata']['host']}</p>
        </div>
        
        <div class="grid">
            <div class="card">
                <h2>Summary</h2>
                <div id="summaryStats"></div>
            </div>
            
            <div class="card">
                <h2>Response Time Distribution</h2>
                <div class="chart-container">
                    <canvas id="responseTimeChart"></canvas>
                </div>
            </div>
        </div>
        
        <div class="card">
            <h2>Requests Statistics</h2>
            <div id="requestStats"></div>
        </div>
        
        <div class="chart-container">
            <h2>Users and RPS Over Time</h2>
            <canvas id="timelineChart"></canvas>
        </div>
        
        <div id="failureStats"></div>
    </div>

    <script>
        // Load test data
        const testData = {js_data};
        
        // Populate summary statistics
        function populateSummary() {{
            const aggregated = testData.requests_statistics.Aggregated;
            const summary = document.getElementById('summaryStats');
            summary.innerHTML = `
                <table class="stats-table">
                    <tr>
                        <th>Total Requests</th>
                        <td>${{aggregated.num_requests}}</td>
                    </tr>
                    <tr>
                        <th>Failed Requests</th>
                        <td>${{aggregated.num_failures}}</td>
                    </tr>
                    <tr>
                        <th>Average Response Time</th>
                        <td>${{aggregated.avg_response_time.toFixed(2)}} ms</td>
                    </tr>
                    <tr>
                        <th>Current RPS</th>
                        <td>${{aggregated.current_rps.toFixed(2)}}</td>
                    </tr>
                </table>
            `;
        }}
        
        // Create response time distribution chart
        function createResponseTimeChart() {{
            const ctx = document.getElementById('responseTimeChart').getContext('2d');
            const stats = testData.response_time_statistics.Aggregated.percentiles;
            
            new Chart(ctx, {{
                type: 'line',
                data: {{
                    labels: ['50%', '60%', '70%', '80%', '90%', '95%', '99%', '100%'],
                    datasets: [{{
                        label: 'Response Time (ms)',
                        data: Object.values(stats),
                        borderColor: 'rgb(75, 192, 192)',
                        tension: 0.1
                    }}]
                }},
                options: {{
                    responsive: true,
                    maintainAspectRatio: false
                }}
            }});
        }}
        
        // Create timeline chart
        function createTimelineChart() {{
            const ctx = document.getElementById('timelineChart').getContext('2d');
            const history = testData.history;
            
            new Chart(ctx, {{
                type: 'line',
                data: {{
                    labels: history.map(h => new Date(h.time).toLocaleTimeString()),
                    datasets: [
                        {{
                            label: 'Users',
                            data: history.map(h => h.user_count),
                            borderColor: 'rgb(75, 192, 192)',
                            yAxisID: 'y'
                        }},
                        {{
                            label: 'RPS',
                            data: history.map(h => h.current_rps),
                            borderColor: 'rgb(255, 99, 132)',
                            yAxisID: 'y1'
                        }}
                    ]
                }},
                options: {{
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {{
                        y: {{
                            type: 'linear',
                            display: true,
                            position: 'left',
                            title: {{
                                display: true,
                                text: 'Users'
                            }}
                        }},
                        y1: {{
                            type: 'linear',
                            display: true,
                            position: 'right',
                            title: {{
                                display: true,
                                text: 'Requests per Second'
                            }}
                        }}
                    }}
                }}
            }});
        }}
        
        // Populate request statistics
        function populateRequestStats() {{
            const statsDiv = document.getElementById('requestStats');
            let html = '<table class="stats-table"><thead><tr>' +
                '<th>Name</th>' +
                '<th>Requests</th>' +
                '<th>Failures</th>' +
                '<th>Median (ms)</th>' +
                '<th>95%ile (ms)</th>' +
                '<th>RPS</th>' +
                '</tr></thead><tbody>';
                
            for (const [name, stats] of Object.entries(testData.requests_statistics)) {{
                if (name !== 'Aggregated') {{
                    html += `<tr>
                        <td>${{name}}</td>
                        <td>${{stats.num_requests}}</td>
                        <td>${{stats.num_failures}}</td>
                        <td>${{stats.median_response_time}}</td>
                        <td>${{stats.response_time_percentiles['95']}}</td>
                        <td>${{stats.current_rps.toFixed(2)}}</td>
                    </tr>`;
                }}
            }}
            html += '</tbody></table>';
            statsDiv.innerHTML = html;
        }}
        
        // Populate failure statistics
        function populateFailureStats() {{
            const failures = testData.failures_statistics;
            if (failures.length > 0) {{
                const failureDiv = document.getElementById('failureStats');
                let html = '<div class="card failure-card"><h2>Failures</h2>' +
                    '<table class="stats-table"><thead><tr>' +
                    '<th>Endpoint</th>' +
                    '<th>Method</th>' +
                    '<th>Error</th>' +
                    '<th>Occurrences</th>' +
                    '</tr></thead><tbody>';
                    
                for (const failure of failures) {{
                    html += `<tr>
                        <td>${{failure.name}}</td>
                        <td>${{failure.method}}</td>
                        <td>${{failure.error}}</td>
                        <td>${{failure.occurrences}}</td>
                    </tr>`;
                }}
                html += '</tbody></table></div>';
                failureDiv.innerHTML = html;
            }}
        }}
        
        // Initialize all charts and stats
        window.onload = function() {{
            populateSummary();
            createResponseTimeChart();
            createTimelineChart();
            populateRequestStats();
            populateFailureStats();
        }};
    </script>
</body>
</html>
"""
    return html_template

def save_html_report(test_data: Dict[str, Any], output_file: str) -> None:
    """
    Generate and save the HTML report to a file.
    
    Args:
        test_data: Dictionary containing the parsed test data
        output_file: Path where the HTML report should be saved
    """
    html_content = generate_html_report(test_data)
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write(html_content)

if __name__ == "__main__":
    # Example usage
    from GetHTML import parse_locust_report  # Import the parser we created earlier
    
    # Parse the original report
    test_data = parse_locust_report('Results/report.html')
    
    # Generate and save the new report
    save_html_report(test_data, 'generated_report.html')