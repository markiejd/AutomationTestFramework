import requests

url = "https://atf368-dfhahhbugtcqhhb8.eastus-01.azurewebsites.net/update-config"

payload = {
    "API": "BidGen",
    "verify_ssl": False,
    "request": "AskQuestion",
    "api_key": True,
    "user_number": 2,
    "increment_rate": 1,
    "run_time": "10s",
}

headers = {
    "Content-Type": "application/json"
}

response = requests.post(url, json=payload, headers=headers)

print(response.json())
