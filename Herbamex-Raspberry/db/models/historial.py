from .base import Base
from sqlalchemy import Column, Integer, Float, Date, ForeignKey
from sqlalchemy.orm import relationship

class Historial(Base):
    __tablename__ = "historial"

    fecha = Column(Date, primary_key=True)
    planta1 = Column(Integer, ForeignKey('plantas.id'))
    planta2 = Column(Integer, ForeignKey('plantas.id'))
    planta3 = Column(Integer, ForeignKey('plantas.id'))
    temperatura = Column(Float)
    humedad = Column(Float)
    size1 = Column(Float)
    size2 = Column(Float)
    size3 = Column(Float)
    riego = Column(Integer)