import cv2
import matplotlib.pyplot as plt
import numpy as np
from transformers import pipeline
import tempfile
import os

def get_size():
    image_path = "/home/ian/Desktop/Proyecto_Final/images/image.jpg"
    # Cargar el modelo de estimacion de profundidad
    pipe = pipeline(task="depth-estimation", model="LiheYoung/depth-anything-small-hf")
    
    # Cargar la imagen
    image = cv2.imread(image_path)
    if image is None:
        print(f"Error: No se pudo cargar la imagen desde {image_path}.")
        return
    
    # Convertir la imagen de BGR a RGB
    image_rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    
    with tempfile.NamedTemporaryFile(delete=False, suffix='.jpg') as temp_file:
        temp_filename = temp_file.name  # Obten el nombre del archivo temporal
        cv2.imwrite(temp_filename, image[100:, :])
    
        # Procesar la imagen original para obtener la profundidad
        depth = pipe(temp_filename)["depth"]
        depth = np.array(depth)
        
        os.remove(temp_filename)

    # Escalar los valores de profundidad a 8 bits (0-255) para visualizacion
    # Normalizamos los valores de profundidad en el rango 0-255
    depth_min = depth.min()
    depth_max = depth.max()
    depth_normalized = np.uint8(255 * (depth - depth_min) / (depth_max - depth_min))

    # Funcion de interpolacion para convertir valores de profundidad en cm
    def depth_to_cm(depth_value):
        # Ecuacion de interpolacion: y = -0.2041 * x + 73.0
        return -0.2041 * depth_value + 73.0

    # Subplot 1 - 0 - 250
    max_val_1_cm = depth_to_cm(np.max(depth[:, 0:250]))

    # Subplot 2 - 251 - 399
    max_val_2_cm = depth_to_cm(np.max(depth[:, 250:400]))

    # Subplot 3 - 400 - Max
    max_val_3_cm = depth_to_cm(np.max(depth[:, 400:]))
    
    # Redondear los valores de profundidad en cm a dos decimales
    max_val_1_cm = np.round(max_val_1_cm, 2)
    max_val_2_cm = np.round(max_val_2_cm, 2)
    max_val_3_cm = np.round(max_val_3_cm, 2)
    
    # Retornar los valores redondeados
    return max_val_1_cm, max_val_2_cm, max_val_3_cm
