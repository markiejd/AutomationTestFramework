# Flask Locust Integration

This project contains how to update a JSON configuration file via a Flask application and run a Locust file using the updated configuration.

## Prerequisites

- Python
- Flask
- Locust 

## Steps

python app.py

For Running Locust:

Remove-item alias:curl

curl -X GET http://127.0.0.1:8000/locust/run?username=test"&"password=123"&"keyword=RunLocust

<!-- [EXPERIMENTAL] To export data:
python ExportHTML.py -->

For Updating the Config File:
python testjson.py


## Steps 2.0

1. Upload <your_app_here>_config.json

2. Update target_config.json with relevant API and Request values.

3. run:
    locust -f locustfile.py --headless -u 2 -r 1 -t 2m --csv Results/locust 

    -u: Users Count
    -r: Run Up
    -t: runtime
    --headless: runs in terminal only
    --csv: Will use Locust's built-in capabilities to 
           generate csv reports of completed locust tests in [Results folder]
           Note: these will be overwritten after every run.
