import time
from datetime import datetime
import RPi.GPIO as GPIO
from config import *  # pylint: disable=unused-wildcard-import
from mfrc522 import MFRC522
import neopixel
import board
import os
import lib.oled.SSD1331 as SSD1331
from PIL import Image, ImageDraw, ImageFont

import mqtt_client


notReadDelay = 2  # Number of reads of no card before re-arming the reader
screenFontSize = 12
zonesRadiation = [
    ("A", 0.1),
    ("B", 0.2),
    ("C", 0.3),
    ("D", 0.4),
]
fontPath = "./lib/oled/Font.ttf"
currentZoneIndex = 0
last_state_left = GPIO.input(encoderLeft)
last_state_right = GPIO.input(encoderRight)
MIFAREReader = MFRC522()
pixels = neopixel.NeoPixel(board.D18, 8, brightness=1.0 / 32, auto_write=False)
disp = SSD1331.SSD1331()


def encoderEvent(channel):
    """Handles any encoder movement and changes the current zone accordingly."""
    global last_state_left, last_state_right, currentZoneIndex

    current_state_left = GPIO.input(encoderLeft)
    current_state_right = GPIO.input(encoderRight)

    if (last_state_left == 1 and current_state_left == 0 and currentZoneIndex < len(zonesRadiation) - 1):
        currentZoneIndex += 1  # go to next zone
    if (last_state_right == 1 and current_state_right == 0 and currentZoneIndex > 0):
        currentZoneIndex -= 1  # go to previous zone
    displayCurrentZone()
    last_state_left = current_state_left
    last_state_right = current_state_right

def displayCurrentZone():
    """Displays the current zone on the OLED screen."""
    if fontPath is None:
        print("Zone: ", zonesRadiation[currentZoneIndex][0], " Radiation: ", zonesRadiation[currentZoneIndex][1])
        return
    disp.clear()
    image = Image.new("RGB", (disp.width, disp.height), "WHITE")
    draw = ImageDraw.Draw(image)
    font = ImageFont.truetype(fontPath, screenFontSize)

    zone, radiation = zonesRadiation[currentZoneIndex]
    line1 = f"Zone: {zone}"
    line2 = f"Radiation: {radiation} mSv/h"

    draw.text((0, 0), line1, font=font, fill="BLACK")
    draw.text((0, screenFontSize + 1), line2, font=font, fill="BLACK")
    disp.ShowImage(image, 0, 0)


def rfidRead():
    """Reads the UID from an RFID card. Returns the UID or None if no card is detected."""
    try:
        (status, TagType) = MIFAREReader.MFRC522_Request(MIFAREReader.PICC_REQIDL)
        if status == MIFAREReader.MI_OK:
            (status, uid) = MIFAREReader.MFRC522_Anticoll()
            if status == MIFAREReader.MI_OK:
                return uid
        else:
            return None
    except Exception as e:
        print(f"Error reading card: {e}")
        return None

def sendUID(uid, timestamp):
    """Sends the UID and timestamp to the MQTT broker"""
    print(f"UID: {uid}, Timestamp: {timestamp}, Zone ID: {currentZoneIndex}")
    
    mqtt_client.sendLog(uid, currentZoneIndex)

def receiveResponse(uid):
    """Receives a response, if the user has not exceeded the radiation limit."""
    
    return mqtt_client.wait_for_access_granted(uid)

def buzz(duration=0.1):
    """Triggers the buzzer for the specified duration."""
    GPIO.output(buzzerPin, 0)
    time.sleep(duration)
    GPIO.output(buzzerPin, 1)

def ledColor(duration=1, color=(255, 0, 0)):
    """Lights the green LED for the specified duration."""
    pixels.fill(color)
    pixels.show()
    time.sleep(duration)
    pixels.fill((0, 0, 0))
    pixels.show()

def cardWasRead(uid):
    """Handles actions when a card is successfully read."""
    sendUID(uid)
    buzz()
    if receiveResponse(uid):
        ledColor(color=(0, 255, 0))
    else:
        ledColor(color=(255, 0, 0))

def mainLoop():
    """Main loop for continuously reading RFID cards."""
    global fontPath
    GPIO.add_event_detect(encoderLeft, GPIO.FALLING, callback=encoderEvent, bouncetime=200)
    GPIO.add_event_detect(encoderRight, GPIO.FALLING, callback=encoderEvent, bouncetime=200)
    #Try to find a font file:
    if not os.path.exists(fontPath):
        print("Font file not found at: " + fontPath + ". Checking another location.")
        fontPath = os.path.join(os.path.dirname(__file__), "lib/oled/Font.ttf")
        if not os.path.exists(fontPath):
            print("Font file not found at: " + fontPath + ". Checking another location.")
            fontPath = os.path.join(os.path.dirname(__file__), "Font.ttf")
            if not os.path.exists(fontPath):
                print("Font file not found at: " + fontPath + ". Unable to use screen.")
                fontPath = None
        
    delay = 0  # If 0, the reader is armed; otherwise, each no-card read decrements the delay
    disp.Init()
    disp.clear()
    displayCurrentZone()
    while True:
        uid = rfidRead()
        if uid is not None:
            if delay == 0:
                print("Card read: ", uid)
                cardWasRead(uid)
            delay = notReadDelay
        else:
            if delay > 0:
                delay -= 1
        time.sleep(0.5)

if __name__ == "__main__":
    mqtt_client.connect()
    try:
        mainLoop()
    except KeyboardInterrupt:
        print("Exiting...")
    finally:
        GPIO.cleanup()
        mqtt_client.disconnect()
