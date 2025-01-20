import paho.mqtt.client as mqtt
import json
import threading
from datetime import datetime

broker: str = "localhost"
card_channel_name: str = "iot/entrance_logs"
alert_channel_name: str = "iot/radiation_alert"

card_access_responses = []
# {
#     "CardId": str,
#     "ZoneId": int,
#     "AccessGranted": bool
# }

lock = threading.Lock()
condition = threading.Condition(lock)

client = mqtt.Client()

def process_message(client, userdata, message):
    global card_access_responses
    with condition:
        print(f"Otrzymano wiadomość: {message.payload.decode()} na temat: {message.topic}")
        card_access_responses.append(json.loads(message.payload.decode()))
        condition.notify_all() 


def wait_for_access_granted(cardId: str) -> bool:
    def message_in_list():
        return cardId in [item.get("CardId") for item in card_access_responses]

    with condition:
        condition.wait_for(message_in_list) 
        for index, item in enumerate(card_access_responses):
            if item.get('CardId') == cardId:
                access_granted_value = item.get('AccessGranted')
                del card_access_responses[index]  # Usuwanie słownika z listy
                return access_granted_value
        return False

def connect():
    client.connect(broker)
    client.on_message = process_message
    client.subscribe(alert_channel_name)
    client.loop_forever()

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
