using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class Reconecta : MonoBehaviour
{
    private const string menuScene = "Cargando";
    private const string testUrl = "https://www.google.com"; // URL para comprobar la conectividad

    // Este método se llamará cuando el botón sea clicado
    public void OnClick()
    {
        StartCoroutine(CheckInternetAndLoadScene());
    }

    private IEnumerator CheckInternetAndLoadScene()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(testUrl))
        {
            // Enviar la solicitud
            yield return www.SendWebRequest();

            // Comprobar si hubo algún error
            if (www.result == UnityWebRequest.Result.Success)
            {
                SceneManager.LoadScene(menuScene);
            }
        }
    }
}