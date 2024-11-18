from gpiozero import Motor
from time import sleep
import threading

def Riega():
    motor = Motor(forward=21, backward=20)
    
    def encender_motor():
        motor.forward()  # El motor empieza a girar hacia adelante
        sleep(10)  # El motor estara encendido durante 10 segundos
        motor.stop()  # Detiene el motor despues de 10 segundos

    # Crear un hilo para ejecutar la funcion encender_motor
    hilo_motor = threading.Thread(target=encender_motor)
    hilo_motor.start()
