from picamera2 import Picamera2
from time import sleep

def Camara():	
	picam2 = Picamera2()
	picam2.start()
	picam2.capture_file("images/image.jpg")
	picam2.stop()
	picam2.close()
