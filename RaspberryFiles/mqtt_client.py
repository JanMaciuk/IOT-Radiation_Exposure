import paho.mqtt.client as mqtt
import json
import threading
from datetime import datetime

broker: str = "10.108.33.101"
# broker: str = "10.108.33.122"
port = 1883
rasberry_id: int = 0
card_channel_name: str = "iot/entrance_logs"
response_channel_name: str = f"iot/entrance_response/{rasberry_id}"


BLANK_RESPONSE  = {
    "CardId": "",
    "ZoneId": -1,
    "AccessGranted": False
}
entrance_response = BLANK_RESPONSE


lock = threading.Lock()
condition = threading.Condition(lock)

client = mqtt.Client()

client.username_pw_set("admin", "admin")

def process_message(client, userdata, message):
    global entrance_response
    with condition:
        entrance_response = json.loads(message.payload.decode())
        condition.notify_all() 


def wait_for_access_granted(cardId: str) -> bool:
    def response_received():
        return cardId == entrance_response.get("CardId")

    with condition:
        if condition.wait_for(response_received, timeout=2):
            return entrance_response.get("AccessGranted")
        return True

def connect():
    client.connect(broker,port=port)
    print("connected")
    client.on_message = process_message
    client.loop_start()
    client.subscribe(response_channel_name)
    
    
def disconnect():
    client.disconnect()


def sendLog(card_id: str, zone_id: int):
    entrance_log = {
        "CardId": card_id,
        "ZoneId": zone_id+1,
        "Timestamp": datetime.now().isoformat(),
        "RasberryId": rasberry_id
    }
    client.publish(card_channel_name, json.dumps(entrance_log))
    print(json.dumps(entrance_log))
    global entrance_response
    entrance_response = BLANK_RESPONSE

if __name__ == "__main__":
    connect()
    sendLog("123123", 0)
    disconnect()
