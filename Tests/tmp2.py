import time
import json
import random
from datetime import datetime
import paho.mqtt.client as mqtt

def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print("[MQTT] Połączono z brokerem MQTT")
        client.subscribe(response_channel_name)
    else:
        print(f"[MQTT] Błąd połączenia, kod: {rc}")

def on_message(client, userdata, message):
    response = json.loads(message.payload.decode())
    print(f"[MQTT] Otrzymano odpowiedź: {response}")
    
    if response.get("AccessGranted"):
        print("[SYSTEM] Dostęp przyznany! Zmiana koloru LED na zielony.\n")
    else:
        print("[SYSTEM] Dostęp ODRZUCONY! Zmiana koloru LED na czerwony i aktywacja buzzera.\n")

broker = "127.0.0.1"  # Możesz podać adres swojego brokera
port = 1883
card_channel_name = "iot/entrance_logs"
response_channel_name = "iot/entrance_response/0"

def send_log(client, card_id, zone_id):
    log = {
        "CardId": card_id,
        "ZoneId": zone_id,
        "Timestamp": datetime.now().isoformat(),
        "RaspberryId": 0
    }
    print(f"[SYSTEM] Wysyłanie logu: {log}")
    client.publish(card_channel_name, json.dumps(log))

def main():
    client = mqtt.Client()
    client.on_connect = on_connect
    client.on_message = on_message
    
    try:
        client.connect(broker, port)
        client.loop_start()
    except Exception as e:
        print(f"[MQTT] Błąd połączenia: {e}")
        return
    
    try:
        while True:
            send_log(client, "433598404", 1)
            time.sleep(5)
    except KeyboardInterrupt:
        print("[SYSTEM] Zamykanie symulatora...")
    finally:
        client.loop_stop()
        client.disconnect()

if __name__ == "__main__":
    main()
