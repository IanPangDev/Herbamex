from .models.historial import Historial
from .models.plantas import Plantas
from sqlalchemy.orm import sessionmaker, aliased
from sqlalchemy import create_engine, desc
import pandas as pd

class Model:
    def __init__(self):
        self.engine = create_engine('sqlite:///db/herbamex.db')
        self.Session = sessionmaker(bind=self.engine)
        self.query_planta()

    def query_planta(self):
        session = self.Session()
        Planta1 = aliased(Plantas)
        Planta2 = aliased(Plantas)
        Planta3 = aliased(Plantas)
        self.historial_subquery = (session.query(
                Historial.fecha,
                Planta1.planta.label("planta1"),
                Planta2.planta.label("planta2"),
                Planta3.planta.label("planta3"),
                Historial.temperatura,
                Historial.humedad,
                Historial.size1,
                Historial.size2,
                Historial.size3,
                Historial.riego
                )
                .outerjoin(Planta1, Historial.planta1 == Planta1.id)
                .outerjoin(Planta2, Historial.planta2 == Planta2.id)
                .outerjoin(Planta3, Historial.planta3 == Planta3.id)
                ).subquery()
        session.close()
    
    def get_plantas(self):
        session = self.Session()
        plantas = pd.read_sql_query(
            session.query(
                self.historial_subquery.c.planta1,
                self.historial_subquery.c.planta2,
                self.historial_subquery.c.planta3).statement,
            self.engine).tail(1)
        session.close()
        return [i.lower() for i in plantas.to_numpy()[0]]
        
    def get_planta_index(self, planta):
        session = self.Session()
        id_planta = pd.read_sql_query(
            session.query(Plantas.id).where(Plantas.planta == planta).statement,
            self.engine)
        session.close()
        return id_planta.to_numpy()[0][0]
        
    def get_last_fecha(self):
        session = self.Session()
        last_registro = pd.read_sql_query(
            session.query(Historial.fecha).statement,
            self.engine).tail(1)
        session.close()
        return last_registro.to_numpy()[0][0]
        
    def get_rigos_3days(self, fecha):
        session = self.Session()
        riegos = pd.read_sql_query(
            session.query(self.historial_subquery.c.riego).where(
                self.historial_subquery.c.fecha>fecha).statement,
            self.engine).sum()
        session.close()
        return riegos["riego"].sum()

    def actualiza_plantas(self, plantas):
        session = self.Session()
        try:
            new_plantas = session.query(Historial).order_by(
                        desc(Historial.fecha)).first()
            
            plantas = [session.query(Plantas.id).where(Plantas.planta == i).first()[0] for i in plantas]
            new_plantas.planta1 = plantas[0]
            new_plantas.planta2 = plantas[1]
            new_plantas.planta3 = plantas[2]
            session.commit()
            print(plantas)
            self.query_planta()
        except:
            session.rollback()
        session.close()

    def get_reporte(self, fecha):
        session = self.Session()
        registro = pd.read_sql_query(
            session.query(self.historial_subquery).where(
                self.historial_subquery.c.fecha>fecha).statement,
            self.engine)
        session.close()
        return registro.set_index(registro['fecha']).drop(columns=['fecha'])
    
    def add_registro(self, fecha, planta1, planta2, planta3, temperatura, humedad, size1, size2, size3, riego):
        session = self.Session()
        if session.query(Historial).filter_by(fecha=fecha).first() == None:
            new_registro = Historial(fecha = fecha,
                                planta1=planta1,
                                planta2=planta2,
                                planta3=planta3,
                                temperatura=temperatura,
                                humedad=humedad,
                                size1=size1,
                                size2=size2,
                                size3=size3,
                                riego=riego)
            session.add(new_registro)
            session.commit()
            session.close()
            return True
        else:
            session.close()
            return False
        
    def add_planta(self, planta):
        session = self.Session()
        if session.query(Plantas).filter_by(planta=planta).first() == None:
            session.add(Plantas(planta=planta))
            session.commit()
            session.close()
            return True
        else:
            session.close()
            return False
