import requests
import random
import time

class Sensor:
    def __init__(self, sensor_id, server_url, threshold_url):
        self.sensor_id = sensor_id
        self.server_url = server_url
        self.threshold_url = threshold_url
        self.threshold = self.get_threshold()
        print(f"Initial threshold value: {self.threshold}")

    def get_threshold(self):
        # Retrieve the current threshold value from the server.
        response = requests.get(f"{self.server_url}/{self.sensor_id}", verify=False)
        if response.status_code == 200:
            return response.json().get('threshold', 50)
        else:
            raise Exception(f"Error getting threshold value: {response.status_code}")

    def generate_data(self):
        if random.random() < 0.01:
            return self.threshold + random.uniform(1, 10)
        else:
            return random.uniform(0, self.threshold)

    def send_data(self, data):
        is_above_threshold = data > self.threshold
        payload = {
            "SensorId": str(self.sensor_id),
            "Value": data,
            "IsAboveThreshold": is_above_threshold
        }
        # Send the generated sensor data to the server.
        response = requests.post(f"{self.threshold_url}", json=payload, verify=False)
        if response.status_code != 200:
            print(f"Error sending data: {response.status_code}, Message: {response.json()}")
        else:
            print(f"Sent: {payload}")

    def run(self):
        while True:
            data = self.generate_data()
            print(f"Generated data: {data}")
            self.send_data(data)
            time.sleep(10)

    def update_threshold(self, new_threshold):
        self.threshold = new_threshold
        print(f"New threshold value: {self.threshold}")

    def listen_for_updates(self):
        from flask import Flask, request
        app = Flask(__name__)
        # Updates the threshold if the request contains the correct sensor ID.

        @app.route('/update-threshold', methods=['POST'])
        def update_threshold_route():
            data = request.json
            if data.get('sensor_id') == str(self.sensor_id):
                self.update_threshold(data.get('threshold', self.threshold))
                return "Threshold updated", 200
            return "Invalid sensor ID", 400

        app.run(port=8080)
