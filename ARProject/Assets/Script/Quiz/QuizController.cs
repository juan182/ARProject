using UnityEngine;

public class QuizController : MonoBehaviour
{
    [Header("Paneles del Quiz")]
    public GameObject[] panelesPreguntas;

    private int actual = 0;

    private void Start()
    {
        // Asegura que solo el primer panel esté activo al inicio
        for (int i = 0; i < panelesPreguntas.Length; i++)
        {
            panelesPreguntas[i].SetActive(i == 0);
        }
    }

    public void SiguientePregunta()
    {
        if (panelesPreguntas.Length == 0) return;

        // Desactiva el panel actual
        if (actual < panelesPreguntas.Length)
        {
            panelesPreguntas[actual].SetActive(false);
            actual++;

            // Si hay más preguntas, activa la siguiente
            if (actual < panelesPreguntas.Length)
            {
                panelesPreguntas[actual].SetActive(true);
                Debug.Log($"➡️ Mostrando pregunta {actual + 1}");
            }
            else
            {
                Debug.Log("✅ Quiz terminado.");
            }
        }
    }
}