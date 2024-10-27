using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBarFill;
    public TMP_Text progreso;
    public string mainScene = "Menu";
    private const string noInternetScene = "Sin_internet";
    public Animator animator;
    private const float timeout = 1f; // Tiempo de espera en segundos

    void Start()
    {
        StartCoroutine(LoadSceneAfterSetup());
    }

    private IEnumerator LoadSceneAfterSetup()
    {
        // Begin loading the main scene
        yield return LoadMainSceneAsync();
    }

    private IEnumerator LoadMainSceneAsync()
    {
        UpdateProgressBar(0f);

        // Obtain weather and store in PlayerPrefs
        yield return StartCoroutine(DataFetcher.GetWeatherCondition());

        UpdateProgressBar(0.2f);

        // Load the main scene
        yield return LoadMainScene();
    }

    private IEnumerator LoadMainScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainScene);
        asyncLoad.allowSceneActivation = false;

        int totalRequests = 6; // Total de métodos a llamar
        int completedRequests = 0;
        bool allDataFetched = true;

        foreach (var method in new[] { "plantas", "medidas", "foto", "reporte", "luz", "agua" })
        {
            yield return StartCoroutine(DataFetcher.SendPostRequest(method));

            if (DataFetcher.IsSuccessful)
            {
                completedRequests++;
                float progress = 0.8f + (completedRequests / (float)totalRequests * 0.2f);
                UpdateProgressBar(progress);
            }
            else
            {
                allDataFetched = false;
                break;
            }
        }

        if (!allDataFetched)
        {
            SceneManager.LoadScene("sin_internet");
            yield break; // Salir de la corutina
        }

        while (!asyncLoad.isDone)
        {
            // Si la escena ha cargado completamente, activa la escena
            if (asyncLoad.progress >= 0.9f)
            {
                UpdateProgressBar(1f); // Actualiza la barra de progreso a 100%
                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // Espera por la animación
                asyncLoad.allowSceneActivation = true; // Activa la escena
            }

            yield return null; // Espera al siguiente frame
        }
    }

    private void UpdateProgressBar(float progress)
    {
        if (progressBarFill != null)
        {
            progressBarFill.value = Mathf.Clamp01(progress);
            progreso.text = $"{progress * 100f:0}%";
        }
    }
}
