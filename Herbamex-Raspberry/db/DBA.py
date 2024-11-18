import sqlite3

db_connection = sqlite3.connect("herbamex.db")

# db_connection.execute("""CREATE TABLE plantas(
#                         id INTEGER PRIMARY KEY AUTOINCREMENT,
#                         planta TEXT NOT NULL UNIQUE)""")

# db_connection.execute("""CREATE TABLE historial(
#                         fecha DATE PRIMARY KEY,
#                         planta1 INTEGER NOT NULL,
#                         planta2 INTEGER NOT NULL,
#                         planta3 INTEGER NOT NULL,
#                         temperatura FLOAT NOT NULL,
#                         humedad FLOAT NOT NULL,
#                         size1 FLOAT NOT NULL,
#                         size2 FLOAT NOT NULL,
#                         size3 FLOAT NOT NULL,
#                         riego INTEGER NOT NULL,
#                         FOREIGN KEY (planta1) REFERENCES plantas(id),
#                         FOREIGN KEY (planta2) REFERENCES plantas(id),
#                         FOREIGN KEY (planta3) REFERENCES plantas(id))""")


db_connection.execute("""DELETE FROM historial""")
db_connection.commit()
