// Modelo para deserializar el JSON
[System.Serializable]
public class ResponseData
{
    public string method;
    public string[] plantas;
    public string[] medidas;
    public string[] foto;
    public string[] reporte;
    public float nivel;
    public int capacidad;
    public int status;
}

// Wrapper para serializar el arreglo de plantas
[System.Serializable]
public class PlantasWrapper
{
    public string[] plantas;
}

// Wrapper para serializar el arreglo de medidas
[System.Serializable]
public class MedidasWrapper
{
    public string[] medidas;
}

// Wrapper para serializar la foto
[System.Serializable]
public class FotoWrapper
{
    public string[] foto;
}

// Wrapper para serializar el reporte
[System.Serializable]
public class ReporteWrapper
{
    public string[] reporte;
}