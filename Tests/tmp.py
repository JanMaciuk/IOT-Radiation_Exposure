import paho.mqtt.client as mqtt
import json
import threading
from datetime import datetime

broker = "localhost"
port = 1883
raspberry_id = "T0"
card_channel_name = "iot/entrance_logs"
response_channel_name = f"iot/entrance_response/{raspberry_id}"

BLANK_RESPONSE = {
    "CardId": "",
    "ZoneId": -1,
    "AccessGranted": False
}
entrance_response = BLANK_RESPONSE

lock = threading.Lock()
condition = threading.Condition(lock)

client = mqtt.Client()

def process_message(client, userdata, message):
    global entrance_response
    with condition:
        print(f"Otrzymano wiadomość: {message.payload.decode()} na temat: {message.topic}")
        entrance_response = json.loads(message.payload.decode())
        condition.notify_all()

def wait_for_access_granted(cardId: str) -> bool:
    def response_received():
        return cardId == entrance_response.get("CardId")
    
    with condition:
        if condition.wait_for(response_received, timeout=3):
            return entrance_response.get("AccessGranted")
        return False

def connect():
    try:
        client.connect(broker, port=port)
        print("Połączono z brokerem MQTT")
        client.on_message = process_message
        client.subscribe(response_channel_name)
        client.loop_start()
    except Exception as e:
        print(f"Błąd połączenia: {e}")

def disconnect():
    client.loop_stop()
    client.disconnect()
    print("Rozłączono z brokerem MQTT")

def sendLog(card_id: str, zone_id: int):
    global entrance_response
    entrance_log = {
        "CardId": card_id,
        "ZoneId": zone_id,
        "Timestamp": datetime.now().isoformat(),
        "RaspberryId": raspberry_id
    }
    print(f"Wysyłanie logu: {entrance_log}")
    client.publish(card_channel_name, json.dumps(entrance_log))
    entrance_response = BLANK_RESPONSE

if __name__ == "__main__":
    from time import sleep
    while True:
        sleep(2)
        connect()
        sendLog("123123", 0)
        granted = wait_for_access_granted("123123")
        print(f"Dostęp przyznany: {granted}")
        disconnect()
