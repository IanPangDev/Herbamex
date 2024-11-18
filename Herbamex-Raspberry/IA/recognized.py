import cv2
import tensorflow as tf
import numpy as np
import matplotlib.pyplot as plt

def Recognized():
    # Cargar el modelo
    model = tf.keras.models.load_model("/home/ian/Desktop/Proyecto_Final/IA/79_5_DenseNet169.keras")
    # Densenet169
    optimal_thresholds = {0: 0.67851317, 1: 0.5228325, 2: 0.074599005}

    imagen = cv2.imread("/home/ian/Desktop/Proyecto_Final/images/image.jpg")
    imagen = cv2.cvtColor(imagen, cv2.COLOR_RGB2BGR)
    imagen = imagen / 255.0
    new_imagenes = [np.expand_dims(cv2.resize(imagen[:, 0:250], (250, 250)), axis=0),
                    np.expand_dims(cv2.resize(imagen[:, 250:400], (250, 250)), axis=0), 
                    np.expand_dims(cv2.resize(imagen[:, 400:], (250, 250)), axis=0)]
    
    plantas = []
    for img in new_imagenes:
        y_scores = model.predict(img)

        y_pred_bin = np.zeros((1, 3), dtype=int)
        for i in range(3):
            y_pred_bin[0, i] = (y_scores[0, i] >= optimal_thresholds[i]).astype(int)

        print(y_scores[0])
        if y_pred_bin[0][0] == 1:
            if y_pred_bin[0][2] == 1:
                plantas.append("hierbabuena")
            else:
                plantas.append("epazote")
        elif y_pred_bin[0][1] == 1:
            plantas.append("hierbabuena")
        else:
            plantas.append("romero")

    return plantas
