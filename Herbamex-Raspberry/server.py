from http.server import BaseHTTPRequestHandler, HTTPServer
import json
from db.Model import Model
import base64
from utilities.reporte import generate_plot
from utilities_hardware.camera import Camara
from utilities_hardware.ultrasonico import Ultrasonico
from utilities_hardware.bomba_agua import Riega
from datetime import datetime, timedelta
from paralelo.STATUS import StatusMonitor 
from IA.recognized import Recognized
from utilities.firebase.firebase import send_push_notification
import os
from gpiozero import LED

luz = LED(12)
nivel_luz = 0

os.environ["QT_QPA_PLATFORM"] = "offscreen"

class RequestHandler(BaseHTTPRequestHandler):

    db = Model()
    monitor = None

    def do_POST(self):
        global nivel_luz, luz

        content_length = int(self.headers['Content-Length'])
        post_data = self.rfile.read(content_length)
        data = json.loads(post_data.decode('utf-8'))
        print(data)
        response_dict = {
            'method': None,
            'status': None
        }
        #Revisa si hay metodo 
        if 'method' in data.keys():
            # Metodo para objetener el listado de plantas actual
            if data['method'] == 'plantas':
                response_dict['method'] = data['method']
                print(self.monitor.plantas)
                response_dict['plantas'] = self.monitor.plantas
                response_dict['status'] = 200
            # Metodo para obtener las medidas del sistema
            elif data['method'] == 'medidas':
                response_dict['method'] = data['method']
                temperatura, humedad = self.monitor.get_temp_hum()
                response_dict['medidas'] = [temperatura, humedad]
                response_dict['status'] = 200
            # Metodo para obtener una imagen actual del sistema
            elif data['method'] == 'foto':
                response_dict['method'] = data['method']
                luz.on()
                Camara()
                luz.off()
                with open('images/image.jpg', 'rb') as imagen:
                    foto_base64 = base64.b64encode(imagen.read()).decode('utf-8')
                response_dict['foto'] = [foto_base64]
                response_dict['status'] = 200
            # Metodo para obtener el reporte de 7 dias
            elif data['method'] == 'reporte':
                response_dict['method'] = data['method']
                datos_reporte = self.db.get_reporte(datetime.now()-timedelta(days=7))
                generate_plot(datos_reporte)
                with open('images/reporte.jpg', 'rb') as imagen:
                    foto_base64 = base64.b64encode(imagen.read()).decode('utf-8')
                response_dict['reporte'] = [foto_base64]
                response_dict['status'] = 200
            # Metodo para modificar la luminosidad del sistema
            elif data['method'] == 'luz':
                response_dict['method'] = data['method']
                if "nivel" in data.keys():
                    if data['nivel'] in [0, 1, 1.5, 2]:
                        match data['nivel']:
                            case 0:
                                response_dict['nivel'] = 1
                                nivel_luz = 1
                                luz.on()
                            case 1:
                                response_dict['nivel'] = 1.5
                                nivel_luz = 1.5
                                luz.on()
                            case 1.5:
                                response_dict['nivel'] = 2
                                nivel_luz = 2
                                luz.on()
                            case 2:
                                response_dict['nivel'] = 0
                                nivel_luz = 0
                                luz.off()
                        response_dict['status'] = 200
                    else:
                        response_dict['status'] = 400
                else:
                    response_dict['nivel'] = nivel_luz
                    response_dict['status'] = 200
            # Metodo para realizar el riego
            elif data['method'] == 'riega':
                if self.monitor.agua >= 20:
                    Riega()
                self.monitor.riego += 1
                response_dict['method'] = data['method']
                response_dict['status'] = 200
            # Metodo para actualizar las plantas en caso de fallo
            elif data['method'] == 'uplantas':
                response_dict['method'] = data['method']
                self.monitor.plantas = [planta.lower() for planta in data['plantas']]
                self.db.actualiza_plantas(data['plantas'])
                response_dict['status'] = 200
            # Metodo para saber el estado del agua
            elif data['method'] == 'agua':
                response_dict['method'] = data['method']
                self.monitor.agua = Ultrasonico()
                response_dict['capacidad'] = self.monitor.agua
                response_dict['status'] = 200
            # Respuesta en caso de fallo
            else:
                response_dict['status'] = 404

            response_json = json.dumps(response_dict)
            self.send_response(response_dict['status'])
            self.send_header('Content-type', 'application/json')
            self.end_headers()
            self.wfile.write(response_json.encode('utf-8'))

def run(server_class=HTTPServer, handler_class=RequestHandler, port=65432):
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    print(f'Starting httpd on port {port}...')
    httpd.serve_forever()

if __name__ == "__main__":
    luz.on()
    Camara()
    luz.off()
    monitor = StatusMonitor()
    monitor.start()
    RequestHandler.monitor = monitor
    RequestHandler.monitor.plantas = Recognized()
    
    try:
        run()
    except:
        print("Server stopped")
    finally:
        pass
        #monitor.stop()
        #monitor.join()
