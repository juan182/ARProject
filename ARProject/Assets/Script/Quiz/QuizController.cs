using UnityEngine;

public class QuizController : MonoBehaviour
{
    [Header("Paneles del Quiz")]
    public GameObject[] panelesPreguntas;

    private int actual = 0;

    private void Start()
    {
        if (panelesPreguntas == null || panelesPreguntas.Length == 0)
        {
            if (DebugUI.Instance != null)
                DebugUI.Instance.Log("❌ No hay paneles de preguntas asignados en QuizController.");
            return;
        }

        for (int i = 0; i < panelesPreguntas.Length; i++)
            panelesPreguntas[i].SetActive(i == 0);

        if (DebugUI.Instance != null)
            DebugUI.Instance.Log($"🧠 Iniciando QuizController. Pregunta inicial: {actual + 1}/{panelesPreguntas.Length}");
    }

    public void SiguientePregunta()
    {
        if (panelesPreguntas.Length == 0)
        {
            if (DebugUI.Instance != null)
                DebugUI.Instance.Log("⚠️ No hay paneles de preguntas en el array.");
            return;
        }

        if (DebugUI.Instance != null)
            DebugUI.Instance.Log($"➡️ Botón 'Siguiente' presionado. Actual: {actual + 1}/{panelesPreguntas.Length}");

        if (actual < panelesPreguntas.Length)
        {
            panelesPreguntas[actual].SetActive(false);
            if (DebugUI.Instance != null)
                DebugUI.Instance.Log($"🚫 Ocultando pregunta {actual + 1}");

            actual++;

            if (actual < panelesPreguntas.Length)
            {
                panelesPreguntas[actual].SetActive(true);
                if (DebugUI.Instance != null)
                    DebugUI.Instance.Log($"✅ Mostrando pregunta {actual + 1}");
            }
            else
            {
                if (DebugUI.Instance != null)
                    DebugUI.Instance.Log("🏁 Quiz terminado. No hay más preguntas.");
            }
        }
        else
        {
            if (DebugUI.Instance != null)
                DebugUI.Instance.Log("⚠️ Ya estás en la última pregunta. No se puede avanzar.");
        }
    }
}