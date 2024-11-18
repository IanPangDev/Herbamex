import cv2
from picamera2 import Picamera2
import numpy as np

# Inicializamos la camara
picam2 = Picamera2()
picam2.start()

# Configuramos la camara para capturar imagenes en formato de numpy array
#picam2.configure(picam2.ccreate_still_configuration())

# Creamos una ventana para mostrar la imagen
cv2.namedWindow("Camera Feed", cv2.WINDOW_NORMAL)

try:
    while True:
        # Captura una imagen
        frame = picam2.capture_array()
        # Convertir la imagen a formato BGR (deberia ser RGB por defecto)
        frame_bgr = cv2.cvtColor(frame, cv2.COLOR_RGB2BGR)

        # Mostrar la imagen en la ventana
        cv2.imshow("Camera Feed", frame_bgr)

        # Salir si se presiona la tecla 'q'
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
finally:
    # Liberar recursos al final
    picam2.stop()
    cv2.destroyAllWindows()
