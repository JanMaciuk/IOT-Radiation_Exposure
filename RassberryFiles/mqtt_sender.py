import paho.mqtt.client as mqtt
import json
from datetime import datetime

broker: str = "localhost"
card_channel_name: str = "iot/entrance_logs"

client = mqtt.Client()


def connect():
    client.connect(broker)

def disconnect():
    client.disconnect()


def sendLog(card_id: str, zone_id: int):
    entrance_log = {
    "CardId": card_id,
    "ZoneId": zone_id,
    "Timestamp": datetime.now().isoformat()
    
    }
    client.publish(card_channel_name, json.dumps(entrance_log))

if __name__ == "__main__":
    connect()
    sendLog("123123", 0)
    disconnect()
