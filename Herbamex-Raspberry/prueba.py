from db.Model import Model
from datetime import datetime, timedelta
from utilities.reporte import generate_plot
import random


db = Model()

#print(db.get_plantas())

#datos_reporte = db.get_reporte(datetime.now()-timedelta(days=7))
#print(datos_reporte)

#generate_plot(datos_reporte)

for i in range(6, -1, -1):
    fecha = datetime.strptime(
        datetime.now().strftime("%d-%m-%Y"), "%d-%m-%Y")-timedelta(days=i)
    
    # Usar IDs de plantas fijos
    planta1 = 1
    planta2 = 2
    planta3 = 3
    
    # Generando valores aleatorios para temperatura, humedad y tamanos
    temperatura = round(random.uniform(15.0, 30.0), 2)  # Temperaturas entre 15.0 y 30.0
    humedad = round(random.uniform(30.0, 90.0), 2)       # Humedad entre 30.0% y 90.0%
    size1 = round(random.uniform(1.0, 5.0), 2)           # Tamanos entre 1.0 y 5.0
    size2 = round(random.uniform(1.0, 5.0), 2)
    size3 = round(random.uniform(1.0, 5.0), 2)
    riego = random.randint(0, 1)  # 0 o 1 para riego (desactivado o activado)

    # Insertando el registro en la base de datos
    db.add_registro(fecha, planta1, planta2, planta3, temperatura, humedad, size1, size2, size3, riego)

# print(db.add_planta("EPAZOTE"))
# print(db.add_planta("HIERBABUENA"))
# print(db.add_planta("ROMERO"))
