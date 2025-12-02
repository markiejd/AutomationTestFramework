def check_credentials(username, password):
    valid_credentials = {
        "mduffy": "hello",
        "alice": "securepass123",
        "bob": "strongpassword456",
        "charlie": "safepass789",
        "test": "123"
    }
    
    return username in valid_credentials and valid_credentials[username] == password