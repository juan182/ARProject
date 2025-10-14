using UnityEngine;

public class TestButton : MonoBehaviour
{
    public AssistantController assistant;
    public QuizTimer quizTimer;

    public void OnClick_FinPregunta1()
    {
        ActivarPregunta(1);
    }

    public void OnClick_FinPregunta2()
    {
        ActivarPregunta(2);
    }

    public void OnClick_FinPregunta3()
    {
        ActivarPregunta(3);
    }

    public void OnClick_FinPregunta4()
    {
        ActivarPregunta(4);
    }

    public void OnClick_FinPregunta5()
    {
        ActivarPregunta(5);
    }

    private void ActivarPregunta(int num)
    {
        if (assistant == null)
        {
            Debug.LogWarning("⚠️ No se asignó el asistente en el UIController.");
            return;
        }

        // Detener audio actual si hay AudioManager
        if (assistant.audioManager != null)
        {
            assistant.audioManager.StopAssistantAudio();
        }

        // Activar el bool correspondiente
        switch (num)
        {
            case 1: assistant.finPregunta1 = true; break;
            case 2: assistant.finPregunta2 = true; break;
            case 3: assistant.finPregunta3 = true; break;
            case 4: assistant.finPregunta4 = true; break;
            case 5: assistant.finPregunta5 = true; break;
        }

        // Detiene o reinicia el timer
        if (quizTimer != null)
        {
            quizTimer.StopTimer();
            quizTimer.ReiniciarTimer(); // si quieres que aparezca listo para la siguiente pregunta
        }

        Debug.Log($"✅ finPregunta{num} activado y timer reiniciado");
    }
}