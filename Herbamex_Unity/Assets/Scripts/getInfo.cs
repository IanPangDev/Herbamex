#nullable enable

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class DataFetcher : MonoBehaviour
{
    private const string weatherApiUrl = "https://es.wttr.in/Mexico%20City?format=%C";
    private const string url = "http://192.168.43.11:65432";
    public static bool IsSuccessful { get; private set; }

    public static IEnumerator GetWeatherCondition()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(weatherApiUrl))
        {
            request.timeout = 2;

            yield return request.SendWebRequest();

            string weather = request.result == UnityWebRequest.Result.Success
                ? request.downloadHandler.text.Trim().ToLower()
                : "soleado";

            PlayerPrefs.SetString("WeatherCondition", weather);
        }
    }

    public static IEnumerator SendPostRequest(string method, float? nivel = null, string[]? plantas = null, string? requestUrl = url)
    {
        PostData postData;

        if (nivel != null)
        {
            postData = new PostData_luz(method, (float)nivel);
        }
        else if (plantas != null)
        {
            postData = new PostData_uplantas(method, plantas);
        }
        else
        {
            postData = new PostData(method);
        }

        string json = JsonUtility.ToJson(postData);

        using (UnityWebRequest request = new UnityWebRequest(requestUrl, "POST"))
        {
            byte[] jsonToSend = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("accept", "application/json");
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 2;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;
                ProcessResponse(responseJson);
                IsSuccessful = true;
            }
            else
            {
                IsSuccessful = false;
            }
        }
    }

    private static string GetDefaultResponse(string method)
    {
        return method switch
        {
            "plantas" => "{\"method\": \"plantas\", \"status\": 200, \"plantas\": [\"epazote\", \"romero\", \"hierbabuena\"]}",
            "medidas" => "{\"method\": \"medidas\", \"status\": 200, \"medidas\": [\"25\", \"30\"]}",
            "foto" => Resources.Load<TextAsset>("plantas").text,
            "reporte" => Resources.Load<TextAsset>("reporte").text,
            "luz" => "{\"method\": \"luz\", \"status\": 200, \"nivel\": 0}",
            "agua" => "{\"method\": \"agua\", \"status\": 200, \"capacidad\": 50}",
            _ => "{}"
        };
    }

    private static void ProcessResponse(string responseJson)
    {
        try
        {
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseJson);

            switch (responseData.method)
            {
                case "plantas":
                    if (responseData.status == 200)
                    {
                        PlayerPrefs.SetString("Plantas", JsonUtility.ToJson(new PlantasWrapper { plantas = responseData.plantas }));
                    }
                    break;

                case "medidas":
                    if (responseData.status == 200)
                    {
                        PlayerPrefs.SetString("Medidas", JsonUtility.ToJson(new MedidasWrapper { medidas = responseData.medidas }));
                    }
                    break;

                case "foto":
                    if (responseData.status == 200)
                    {
                        PlayerPrefs.SetString("Foto", JsonUtility.ToJson(new FotoWrapper { foto = responseData.foto }));
                    }
                    break;

                case "reporte":
                    if (responseData.status == 200)
                    {
                        PlayerPrefs.SetString("Reporte", JsonUtility.ToJson(new ReporteWrapper { reporte = responseData.reporte }));
                    }
                    break;

                case "luz":
                    if (responseData.status == 200)
                    {
                        PlayerPrefs.SetFloat("Luz", responseData.nivel);
                    }
                    break;

                case "agua":
                    if (responseData.status == 200)
                    {
                        PlayerPrefs.SetInt("Agua", responseData.capacidad);
                    }
                    break;

                case "riega":
                    if (responseData.status == 200)
                    {
                        Debug.Log("Riego exitoso");
                    }
                    break;
                
                case "uplantas":
                    if (responseData.status == 200)
                    {
                        Debug.Log("Cambio de plantas exitoso");
                    }
                    break;

                default:
                    Debug.LogWarning("Unexpected response: " + responseJson);
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error processing JSON response: {e.Message}");
        }
    }
}

// Define una clase anidada para representar los datos JSON
class PostData
{
    public string method;

    public PostData(string method)
    {
        this.method = method;
    }
}

class PostData_luz : PostData
{
    public float nivel;

    public PostData_luz(string method, float nivel) : base(method)
    {
        this.method = method;
        this.nivel = nivel;
    }
}

class PostData_uplantas : PostData
{
    public string[] plantas;

    public PostData_uplantas(string method, string[] plantas) : base(method)
    {
        this.method = method;
        this.plantas = plantas;
    }
}