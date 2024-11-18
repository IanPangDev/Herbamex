import threading
from db.Model import Model
from utilities_hardware.temperatura import DHT11
from utilities_hardware.lcd import LCD
from utilities_hardware.ventilador import Ventila
from utilities.size import get_size
from time import sleep
from datetime import datetime, timedelta
from utilities.firebase.firebase import send_push_notification

class StatusMonitor(threading.Thread):
    def __init__(self, interval=1):
        super().__init__()
        self.db = Model()
        self.sensor = DHT11(16)
        self.lcd = LCD(2, 0x27, True)
        self.interval = interval
        self.running = True
        self.temperatura = None
        self.humedad = None
        self.last_day = self.db.get_last_fecha()
        self.riego = self.db.get_rigos_3days(datetime.now()-timedelta(days=3))
        self.plantas = None
        self.agua = None

    def run(self):
        while self.running:
            self.humedad, self.temperatura = self.sensor.read_data()
            self.lcd.clear()
            self.lcd.message(f'{self.temperatura} C', 1)
            self.lcd.message(f'{self.humedad} %', 2)
            sleep(self.interval)
            self.lcd.clear()
            sleep(0.5)
            
            if self.temperatura >= 30:
                try:
                    Ventila()
                except:
                    pass
            # Verificar si el dia ha cambiado
            current = datetime.now()
            if self.last_day != current.day:
                self.last_day = current.day
                #self.store_data()  # Llamar a la funcion de almacenamiento
            if current.hour == 12 and current.minute == 0:
                if self.agua > 30:
                    # Detalles de la notificación
                    title = 'Riego pendiente!'
                    body = 'Tus plantas necesitan agua. Riega pronto!'
                    # Enviar la notificación push
                    send_push_notification(title, body)
                

    def stop(self):
        self.running = False
        
    def get_temp_hum(self):
        return self.temperatura, self.humedad
        
    def store_data(self):
        # Funcion de almacenamiento de los datos
        if self.riego > 3:
            # Detalles de la notificación
            title = 'Riego pendiente!'
            body = 'Tus plantas necesitan agua. Riega pronto!'

            # Enviar la notificación push
            send_push_notification(title, body)
        plantas = []
        for planta in self.plantas:
            plantas.append(self.db.get_planta_index(planta.upper()))
        fecha = datetime.strptime(datetime.now().strftime("%d-%m-%Y"), "%d-%m-%Y")
        medidas = get_size()
        self.db.add_registro(fecha, plantas[0], plantas[1], plantas[2], 
                             self.temperatura, self.humedad,
                             medidas[0], medidas[1], medidas[2],
                             self.riego)
        self.riego = 0 
