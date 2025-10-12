using UnityEngine;

public class TestButton : MonoBehaviour
{
    // Arrastra aquí el objeto del asistente desde la jerarquía
    public AssistantController assistant;

    // Este método se llamará desde el botón
    public void OnClick_FinPregunta1()
    {
        if (assistant != null)
        {
            // Detiene el audio actual
            if (assistant.audioManager != null)
            {
                assistant.audioManager.StopAssistantAudio();
            }

            assistant.finPregunta1 = true;
            Debug.Log("✅ finPregunta1 ahora es TRUE");
        }
        else
        {
            Debug.LogWarning("⚠️ No se asignó el asistente en el UIController.");
        }
    }
}
