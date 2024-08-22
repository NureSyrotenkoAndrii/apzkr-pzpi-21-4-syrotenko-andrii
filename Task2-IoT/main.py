from config import SENSOR_ID, SERVER_URL, THRESHOLD_URL
from sensor import Sensor
import threading
import urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)


def main():
    sensor = Sensor(SENSOR_ID, SERVER_URL, THRESHOLD_URL)

    sensor_thread = threading.Thread(target=sensor.run)
    sensor_thread.start()

    sensor.listen_for_updates()


if __name__ == "__main__":
    main()
