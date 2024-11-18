from .base import Base
from sqlalchemy import Column, String, Integer

class Plantas(Base):
    __tablename__ = "plantas"

    id = Column(Integer, primary_key=True)
    planta = Column(String)