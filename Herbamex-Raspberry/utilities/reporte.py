import matplotlib.pyplot as plt
import numpy as np

def generate_plot(df):
    
    # Crear la figura y los ejes para los gráficos y la tabla
    fig, axs = plt.subplots(nrows=3, ncols=2, figsize=(14, 20))  # 3 filas, 2 columnas

    # Ajustar el tamaño de los ejes
    axs[0, 0].set_position([0.1, 0.7, 0.35, 0.15])
    axs[0, 1].set_position([0.55, 0.7, 0.35, 0.15])
    axs[1, 0].set_position([0.1, 0.4, 0.35, 0.15])
    axs[1, 1].set_position([0.55, 0.4, 0.35, 0.15])

    # Eliminar el segundo subplot de la tercera fila
    fig.delaxes(axs[2, 0])
    fig.delaxes(axs[2, 1])

    # Graficar Temperatura
    df.filter(like='temperatura').plot(ax=axs[0, 0], marker='o')
    axs[0, 0].set_title('Temperatura')
    axs[0, 0].set_ylabel('C°', rotation=0, labelpad=20)
    axs[0, 0].set_xlabel('')
    axs[0, 0].tick_params(axis='x', labelrotation=15, labelsize='small')
    axs[0, 0].legend().remove()

    # Graficar Humedad
    df.filter(like='humedad').plot(ax=axs[0, 1], marker='o')
    axs[0, 1].set_title('Humedad')
    axs[0, 1].set_ylabel('%', rotation=0, labelpad=20)
    axs[0, 1].set_xlabel('')
    axs[0, 1].tick_params(axis='x', labelrotation=15, labelsize='small')
    axs[0, 1].legend().remove()

    # Graficar Tamaño
    df.filter(like='size').plot(ax=axs[1, 0], marker='o')
    axs[1, 0].set_title('Tamaño')
    axs[1, 0].set_ylabel('cm', rotation=0, labelpad=20)
    axs[1, 0].set_xlabel('')
    axs[1, 0].tick_params(axis='x', labelrotation=15, labelsize='small')
    axs[1, 0].legend()

    # Graficar Riego
    df.filter(like='riego').plot(ax=axs[1, 1], marker='o')
    axs[1, 1].set_title('Riego')
    axs[1, 1].set_ylabel('Veces', rotation=0, labelpad=20)
    axs[1, 1].set_xlabel('')
    axs[1, 1].tick_params(axis='x', labelrotation=15, labelsize='small')
    axs[1, 1].legend().remove()

    # Crear la tabla en la tercera fila
    table_ax = fig.add_subplot(313)
    table_ax.axis('off')
    table = table_ax.table(
        cellText=df.values,
        colLabels=df.columns,
        rowLabels=df.index,
        cellLoc='center',
        loc='center',
        rowLoc='left'
    )

    # Ajustar el ancho de las columnas
    table.auto_set_column_width([i for i in range(len(df.columns))])

    # Ajustar la altura de las filas
    for key, cell in table.get_celld().items():
        cell.set_height(0.15)  # Ajustar la altura de las filas

    # Ajustar el tamaño del texto en las celdas y centrar
    for (i, j), val in np.ndenumerate(df.values):
        cell_text = table[(i+1, j)].get_text()
        cell_text.set_fontsize(10)  # Ajustar el tamaño del texto
        cell_text.set_horizontalalignment('center')  # Centrar horizontalmente
        cell_text.set_verticalalignment('center')  # Centrar verticalmente

    # Cambiar el color de fondo de la columna 'Humedad'
    for (i, j), cell in table.get_celld().items():
        if i == 0:
            cell.set_facecolor('lightblue')  # Color de fondo para el encabezado

    # Cambiar el color de fondo de las celdas de los rowLabels a verde
    for (i, j), cell in table.get_celld().items():
        if j == -1:  # Celdas de los rowLabels
            cell.set_facecolor('lightgreen')

    # Cambiar el color de fondo de las celdas según umbral específico
    umbral_temp = 30  # Umbral para temperatura
    umbral_humedad = 70  # Umbral para humedad
    umbral_tamano = 25  # Umbral para tamaño
    umbral_riego = 1  # Umbral para riego

    for (i, j), cell in table.get_celld().items():
        if i > 0 and j >= 0:  # Excluir encabezados y rowLabels
            valor = df.iloc[i-1, j]
            if 'Temperatura' in df.columns[j]:
                if valor >= umbral_temp:
                    cell.set_facecolor('red')
            elif 'Humedad' in df.columns[j]:
                if valor >= umbral_humedad:
                    cell.set_facecolor('red')
            elif 'Tamaño' in df.columns[j]:
                if valor >= umbral_tamano:
                    cell.set_facecolor('red')
            elif 'Riego' in df.columns[j]:
                if valor >= umbral_riego:
                    cell.set_facecolor('red')

    # Ajustar el diseño
    fig.tight_layout(pad=10.0)
    plt.subplots_adjust(hspace=0.6)
    plt.savefig("images/reporte.jpg")
