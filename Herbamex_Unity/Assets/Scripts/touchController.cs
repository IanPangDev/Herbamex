using UnityEngine;

public class touchController : MonoBehaviour
{
    public GameObject basse;  // Referencia a la base
    public float rotationSpeed = 0.2f;  // Velocidad de rotaci�n
    public RectTransform panel;  // Referencia al Panel (asignar en el Inspector)

    private Vector2 lastTouchPosition;  // Para almacenar la posici�n anterior del toque

    void Update()
    {
        // Buscar y establecer los objetos como hijos de "basse"
        GameObject[] plantas = GameObject.FindGameObjectsWithTag("Planta");
        foreach (GameObject planta in plantas)
        {
            planta.transform.SetParent(basse.transform);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;

            // Verificar si el toque est� dentro del �rea del Panel
            if (panel == null)
            {
                Debug.LogError("Panel no asignado. Por favor, asigna el Panel en el Inspector.");
                return;
            }

            if (IsTouchInsidePanel(panel, touchPosition))
            {
                // Si el toque est� dentro del �rea del Panel, no hacer nada
                return;
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Al iniciar el toque, almacenar la posici�n inicial
                    lastTouchPosition = touchPosition;
                    break;

                case TouchPhase.Moved:
                    // Calcular la diferencia entre la posici�n actual y la anterior
                    Vector2 delta = touchPosition - lastTouchPosition;

                    // Rotar la base solo en el eje Y
                    basse.transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);

                    // Actualizar la posici�n del �ltimo toque
                    lastTouchPosition = touchPosition;
                    break;

                case TouchPhase.Ended:
                    // Al finalizar el toque, puedes implementar alguna l�gica si es necesario
                    break;
            }
        }
    }

    // M�todo para verificar si el toque est� dentro del �rea del Panel
    private bool IsTouchInsidePanel(RectTransform panel, Vector2 touchPosition)
    {
        Vector2 localPoint;

        // Convertir la posici�n del toque a coordenadas locales del Panel
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(panel, touchPosition, null, out localPoint))
        {
            // Verificar si el punto est� dentro del rect�ngulo del Panel
            return panel.rect.Contains(localPoint);
        }

        return false;
    }
}
