using UnityEngine;
using System;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    public Material sunnyMaterial;
    public Material rainyMaterial;
    public Material cloudyMaterial;
    public Material nightMaterial;
    public Material defaultMaterial;

    public Material photoMaterial;
    public Material reporteMaterial;
    
    public AudioSource bgm;
    public AudioClip lluvia;
    public AudioClip nublado;
    public AudioClip soleado;
    public AudioClip noche;

    public GameObject romero;
    public GameObject hierbabuena;
    public GameObject epazote;

    public Vector3[] positionsPlantas;

    public TMP_Text medidas;
    public GameObject foto;
    public GameObject reporte;

    public TMP_Dropdown[] plantaDown;
    public TMP_Dropdown planta1;
    public TMP_Dropdown planta2;
    public TMP_Dropdown planta3;

    public Slider capacidad;
    public TMP_Text capacidad_text;

    void Start()
    {
        plantaDown = new TMP_Dropdown[] { planta1, planta2, planta3 };
        capacidad.value = PlayerPrefs.GetInt("Agua");
        capacidad_text.text = capacidad.value.ToString() + "%";
        StartCoroutine(InitializeScene());
    }

    IEnumerator InitializeScene()
    {
        // Obtener el clima
        string weather = PlayerPrefs.GetString("WeatherCondition", "default");

        // Plantas
        string plantasJson = PlayerPrefs.GetString("Plantas", "[]");
        PlantasWrapper plantasWrapper = JsonUtility.FromJson<PlantasWrapper>(plantasJson);

        // Medidas
        string medidasJson = PlayerPrefs.GetString("Medidas", "[]");
        MedidasWrapper medidasWrapper = JsonUtility.FromJson<MedidasWrapper>(medidasJson);
        
        // Foto
        string fotoJson = PlayerPrefs.GetString("Foto", "[]");
        FotoWrapper fotoWrapper = JsonUtility.FromJson<FotoWrapper>(fotoJson);

        // Reporte
        string reporteJson = PlayerPrefs.GetString("Reporte", "[]");
        ReporteWrapper reporteWrapper = JsonUtility.FromJson<ReporteWrapper>(reporteJson);

        // Definicion de noche
        bool isNight = DateTime.Now.Hour >= 19 || DateTime.Now.Hour < 6;

        Material skyboxMaterial = GetSkyboxMaterialBasedOnWeather(weather, isNight);
        RenderSettings.skybox = skyboxMaterial;

        // Initialize plant positions (Ensure it has enough positions as needed)
        positionsPlantas = new Vector3[]
        {
            new Vector3(-0.7f, 2.0f, 0.0f),
            new Vector3(0.0f, 2.0f, 0.0f),
            new Vector3(0.7f, 2.0f, 0.0f)
        };

        // Instantiate plants
        InstantiatePlantas(plantasWrapper);

        // Instantiate las medidas
        InstantiateMedidas(medidasWrapper);

        // Instantiate photo
        if (fotoWrapper.foto.Length > 0)
        {
            byte[] photoBytes = Convert.FromBase64String(fotoWrapper.foto[0]);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(photoBytes);

            // Aplicar la textura al material
            if (photoMaterial != null)
            {
                photoMaterial.mainTexture = texture;
            }

            // Aplicar el material al plano
            if (foto != null)
            {
                Renderer renderer = foto.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = photoMaterial;
                }
            }
        }
        else
        {
            Debug.LogWarning("No se recibió imagen para mostrar.");
        }

        // Instantiate photo reporte
        if (reporteWrapper.reporte.Length > 0)
        {
            byte[] photoBytes = Convert.FromBase64String(reporteWrapper.reporte[0]);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(photoBytes);

            // Aplicar la textura al material
            if (reporteMaterial != null)
            {
                reporteMaterial.mainTexture = texture;
            }

            // Aplicar el material al plano
            if (reporte != null)
            {
                Renderer renderer = reporte.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = reporteMaterial;
                }
            }
        }
        else
        {
            Debug.LogWarning("No se recibió imagen para mostrar.");
        }

        yield return null;
    }

    Material GetSkyboxMaterialBasedOnWeather(string weather, bool isNight)
    {
        Material selectedMaterial;
        if (weather.ToLower().Contains("soleado"))
        {
            bgm.clip = isNight ? noche : soleado;
            selectedMaterial = isNight ? nightMaterial : defaultMaterial;
        }
        else if (weather.ToLower().Contains("lluvia"))
        {
            bgm.clip = lluvia;
            selectedMaterial = rainyMaterial;
        }
        else if (weather.ToLower().Contains("nublado"))
        {
            bgm.clip = nublado;
            selectedMaterial = isNight ? nightMaterial : cloudyMaterial;
        }
        else if (weather.ToLower().Contains("despejado"))
        {
            bgm.clip = isNight ? noche : soleado;
            selectedMaterial = isNight ? nightMaterial : defaultMaterial;
        }
        else
        {
            // Caso por defecto si no se encuentra ninguna palabra clave específica.
            bgm.clip = isNight ? noche : soleado;
            selectedMaterial = isNight ? nightMaterial : defaultMaterial;
        }
        bgm.Play();
        return selectedMaterial;
    }

    void InstantiatePlantas(PlantasWrapper plantasWrapper)
    {
        for (int i = 0; i < plantasWrapper.plantas.Length; i++)
        {
            GameObject temp;

            // Instanciar el prefab adecuado según el tipo de planta
            switch (plantasWrapper.plantas[i])
            {
                case "romero":
                    temp = Instantiate(romero, positionsPlantas[i], Quaternion.Euler(0, 90, 0));
                    plantaDown[i].value = 2;
                    break;
                case "hierbabuena":
                    temp = Instantiate(hierbabuena, positionsPlantas[i], Quaternion.identity);
                    plantaDown[i].value = 1;
                    break;
                default:
                    temp = Instantiate(epazote, positionsPlantas[i], Quaternion.Euler(0, 90, 0));
                    plantaDown[i].value = 0;
                    break;
            }

            // Configurar la instancia
            temp.transform.localScale = new Vector3(6, 6, 6);
            temp.transform.name = "planta" + i;
            temp.tag = "Planta";  // Asignar el tag después de instanciar el objeto
        }
    }

    void InstantiateMedidas(MedidasWrapper medidasWrapper)
    {
        medidas.text = medidasWrapper.medidas[0] + " °C\n" + medidasWrapper.medidas[1] + " %";
    }
}