from gpiozero import DistanceSensor
from time import sleep

def Ultrasonico():
    # Define el valor maximo y minimo
    MAX = 4  # en centimetros (distancia maxima del agua)
    MIN = 2  # en centimetros (distancia minima del agua)
    
    # Inicializar el sensor ultrasonico
    sensor = DistanceSensor(echo=26, trigger=19)
    
    # Medir la distancia en centimetros
    distancia_cm = sensor.distance * 100  # El valor por defecto es en metros, se multiplica por 100 para obtener centimetros
    
    # Calcular el porcentaje de agua en el sensor basado en el rango
    if distancia_cm >= MAX:
        porcentaje = 0  # Si la distancia es mayor o igual a MAX, el porcentaje de agua es 0%
    elif distancia_cm <= MIN:
        porcentaje = 100  # Si la distancia es menor o igual a MIN, el porcentaje de agua es 100%
    else:
        # Calcula el porcentaje de agua entre el rango
        porcentaje = ((MAX - distancia_cm) / (MAX - MIN)) * 100
    
    # Redondear el porcentaje a un valor entero
    return round(porcentaje)
