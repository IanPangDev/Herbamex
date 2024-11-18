import firebase_admin
from firebase_admin import credentials
from firebase_admin import messaging

def send_push_notification( title, body, server_key="/home/ian/Desktop/Proyecto_Final/utilities/firebase/herbamex-7e6d0-firebase-adminsdk-ms7w7-26332f98f7.json", topic='all', data=None):
    # Inicializar Firebase Admin si no se ha inicializado
    if not firebase_admin._apps:
        cred = credentials.Certificate(server_key)
        firebase_admin.initialize_app(cred)

    # Crear el mensaje
    message = messaging.Message(
        notification=messaging.Notification(
            title=title,
            body=body
        ),
        topic=topic
    )
    
    if data:
        message.data = data

    try:
        # Enviar el mensaje al topic especificado
        response = messaging.send(message)
        print("Notificación enviada exitosamente:", response)
    except Exception as e:
        print("Error al enviar la notificación:", e)
