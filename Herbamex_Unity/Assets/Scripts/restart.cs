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
            // Verifica cuánto tiempo ha pasado sin atención
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
            // La aplicación ha perdido el enfoque
            lastFocusTime = DateTime.Now; // Guarda el momento en que perdió el enfoque
            hasBeenInBackground = true;
        }
    }
}
