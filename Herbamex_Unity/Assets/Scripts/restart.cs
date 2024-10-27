using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class FocusManager : MonoBehaviour
{
    private const float focusThreshold = 10f * 60f; // 10 minuto en segundos

    private DateTime lastFocusTime;
    private bool hasBeenInBackground;

    void Start()
    {
        lastFocusTime = DateTime.Now;
        hasBeenInBackground = false;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            // Verifica cu�nto tiempo ha pasado sin atenci�n
            if (hasBeenInBackground)
            {
                TimeSpan timeInBackground = DateTime.Now - lastFocusTime;
                if (timeInBackground.TotalSeconds > focusThreshold)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Cargando");
                }
            }
            lastFocusTime = DateTime.Now; // Actualiza el tiempo de enfoque
            hasBeenInBackground = false;
        }
        else
        {
            // La aplicaci�n ha perdido el enfoque
            lastFocusTime = DateTime.Now; // Guarda el momento en que perdi� el enfoque
            hasBeenInBackground = true;
        }
    }
}
