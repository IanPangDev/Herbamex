using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    public Animator cameraAnimator;
    private touchController touchControllerScript;
    public GameObject basse;
    private Quaternion initialRotation;
    public TMP_Text textToBlink; // Referencia al componente Text

    private Coroutine blinkCoroutine; // Para controlar el parpadeo

    public Button vistaButton; // Referencia al botón Vista
    public Button medidaButton; // Referencia al botón Medida
    public Button reporteButton; // Referencia al botón Reporte
    public Button riegoButton; // Referencia al botón Riego
    public Button luzButton; // Referencia al botón Luz
    public Button uPlantasButton; // Referencia al botón uPlantas

    public GameObject photoPlane; // Objeto plano donde se aplicará la foto
    public GameObject camara;

    // Nuevo arreglo para almacenar las plantas
    public GameObject[] plantaObjects;

    private Button[] allButtons; // Arreglo con todos los botones

    public ParticleSystem agua1;
    public ParticleSystem agua2;
    public ParticleSystem agua3;

    public GameObject reporte;

    public GameObject uPlantas;
    public TMP_Dropdown[] plantaDown;
    public TMP_Dropdown planta1;
    public TMP_Dropdown planta2;
    public TMP_Dropdown planta3;

    public GameObject capacidad;

    public Light led;

    private void Start()
    {
        touchControllerScript = GetComponent<touchController>();
        initialRotation = basse.transform.rotation;
        plantaObjects = GameObject.FindGameObjectsWithTag("Planta");

        // Inicializar el arreglo con todos los botones
        allButtons = new Button[] { vistaButton, medidaButton, reporteButton, riegoButton, luzButton, uPlantasButton };

        agua1.Stop();
        agua2.Stop();
        agua3.Stop();

        led.intensity = PlayerPrefs.GetFloat("Luz");

        plantaDown = new TMP_Dropdown[] { planta1, planta2, planta3 };
    }

    public void ToVista()
    {
        touchControllerScript.enabled = !touchControllerScript.enabled;
        if (!touchControllerScript.enabled)
        {
            StartCoroutine(RotateToInitialRotation());
            SetButtonsInteractableExcept(vistaButton, false);
            StartCoroutine(CheckCameraPositionAndRotation(true));
        }
        else
        {
            StartCoroutine(CheckCameraPositionAndRotation(false));
            SetButtonsInteractableExcept(vistaButton, true);
        }
        cameraAnimator.SetTrigger("Vista");
    }

    public void ToMedida()
    {
        touchControllerScript.enabled = !touchControllerScript.enabled;
        if (touchControllerScript.enabled)
        {
            StartCoroutine(RotateToInitialRotation());
            // Detén el parpadeo y asegúrate de que el texto esté visible
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }
            textToBlink.enabled = true;
            SetButtonsInteractableExcept(medidaButton, true);
        }
        else
        {
            StartCoroutine(RotateToMedidas());
            blinkCoroutine = StartCoroutine(BlinkText());
            SetButtonsInteractableExcept(medidaButton, false);
        }
        cameraAnimator.SetTrigger("Medida");
    }

    public void ToRiega()
    {
        touchControllerScript.enabled = !touchControllerScript.enabled;
        if (!touchControllerScript.enabled)
        {
            StartCoroutine(RotateToInitialRotation());
            SetButtonsInteractableExcept(riegoButton, false);
            Riega(true);
        }
        else
        {
            Riega(false);
            SetButtonsInteractableExcept(riegoButton, true);
        }
        cameraAnimator.SetTrigger("Riego");
    }

    public void ToReporte()
    {
        touchControllerScript.enabled = !touchControllerScript.enabled;
        if (!touchControllerScript.enabled)
        {
           reporte.SetActive(true);
           capacidad.SetActive(false);
           SetButtonsInteractableExcept(reporteButton, false);
        }
        else
        {
            reporte.SetActive(false);
            capacidad.SetActive(true);
            SetButtonsInteractableExcept(reporteButton, true);
        }
    }

    private void SetButtonsInteractableExcept(Button activeButton, bool interactable)
    {
        foreach (Button button in allButtons)
        {
            if (button != activeButton)
            {
                button.interactable = interactable;
            }
        }
    }

    private IEnumerator RotateToInitialRotation()
    {
        Quaternion startRotation = basse.transform.rotation;
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            basse.transform.rotation = Quaternion.RotateTowards(startRotation, initialRotation, 360 * t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        basse.transform.rotation = initialRotation;
    }

    private IEnumerator RotateToMedidas()
    {
        Quaternion startRotation = basse.transform.rotation;
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            basse.transform.rotation = Quaternion.RotateTowards(startRotation, Quaternion.Euler(0.0f, 0.0f, 0.0f), 360 * t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        basse.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    }

    private IEnumerator CheckCameraPositionAndRotation(bool activate)
    {
        Vector3 targetPosition = new Vector3(0, 3.75f, 0);
        Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        float positionTolerance = 8.0f;
        float rotationTolerance = 50.0f;
        float checkInterval = 0.3f;

        while (true)
        {
            float distance = Vector3.Distance(camara.transform.position, targetPosition);
            float angle = Quaternion.Angle(camara.transform.rotation, targetRotation);

            if (distance < positionTolerance && angle < rotationTolerance)
            {
                if (activate)
                {
                    // Usa el arreglo para activar/desactivar las plantas
                    foreach (GameObject planta in plantaObjects)
                    {
                        planta.SetActive(false);
                    }
                    // Activa el plano
                    if (photoPlane != null)
                    {
                        photoPlane.SetActive(true);
                    }
                }
                else
                {
                    // Usa el arreglo para activar/desactivar las plantas
                    foreach (GameObject planta in plantaObjects)
                    {
                        planta.SetActive(true);
                    }
                    // Desactiva el plano
                    if (photoPlane != null)
                    {
                        photoPlane.SetActive(false);
                    }
                }
                break;
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void Riega(bool modo)
    {
        if (modo) 
        {
            StartCoroutine(riega_post());
            agua1.Play();
            agua2.Play();
            agua3.Play();
        }
        else
        {
            agua1.Stop();
            agua2.Stop();
            agua3.Stop();
        }
    }

    private IEnumerator riega_post()
    {
        // Envía la solicitud POST y espera a que se complete
        yield return StartCoroutine(DataFetcher.SendPostRequest("riega"));
    }

    private IEnumerator BlinkText()
    {
        float blinkRate = 0.5f; // Frecuencia del parpadeo

        while (true)
        {
            textToBlink.enabled = !textToBlink.enabled;
            yield return new WaitForSeconds(blinkRate);
        }
    }

    public void cambia_luz()
    {
        // Llama a la corutina para actualizar la intensidad
        StartCoroutine(UpdateLightIntensity());
    }

    private IEnumerator UpdateLightIntensity()
    {
        // Envía la solicitud POST y espera a que se complete
        yield return StartCoroutine(DataFetcher.SendPostRequest("luz", led.intensity));

        // Actualiza la intensidad desde PlayerPrefs
        led.intensity = PlayerPrefs.GetFloat("Luz");
    }

    public void ToUPlantas()
    {
        bool diferente = false;
        touchControllerScript.enabled = !touchControllerScript.enabled;
        if (!touchControllerScript.enabled)
        {
            uPlantas.SetActive(true);
            capacidad.SetActive(false);
            SetButtonsInteractableExcept(uPlantasButton, false);
        }
        else
        {
            // Plantas
            string plantasJson = PlayerPrefs.GetString("Plantas", "[]");
            string[] plantasText = new string[3];

            PlantasWrapper plantasWrapper = JsonUtility.FromJson<PlantasWrapper>(plantasJson);
            capacidad.SetActive(true);
            uPlantas.SetActive(false);
            for (int i = 0; i < plantasWrapper.plantas.Length; i++)
            {
                plantasText[i] = plantaDown[i].options[plantaDown[i].value].text.ToUpper();
                if (plantaDown[i].options[plantaDown[i].value].text.ToLower() != plantasWrapper.plantas[i].ToLower())
                    diferente = true;
            }
            SetButtonsInteractableExcept(uPlantasButton, true);
            if (diferente)
            {
                StartCoroutine(UpdatePlantas(plantasText));
                UnityEngine.SceneManagement.SceneManager.LoadScene("Cargando");
            }
        }
    }

    private IEnumerator UpdatePlantas(string[] plantas)
    {
        // Envía la solicitud POST y espera a que se complete
        yield return StartCoroutine(DataFetcher.SendPostRequest("uplantas", null, plantas));
    }
}
