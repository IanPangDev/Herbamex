from gpiozero import LED
from time import sleep
import threading

# Funcion para encender el ventilador (simulado con LED)
def Ventila():
    led = LED(13)  # Inicializamos el LED en el pin adecuado
	
    def encender_ventilador():
        led.on()  # Enciende el ventilador (simulado con el LED)
        sleep(240)  # El ventilador estara encendido por 30 segundos
        led.off()  # Apaga el ventilador (simulado con el LED)
	
    ventilador_thread = threading.Thread(target=encender_ventilador)  # Creamos un hilo para la funcion Ventila
    ventilador_thread.start()  # Inicia el hilo
