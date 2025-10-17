using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    [Header("Referencias del UI")]
    public TextMeshProUGUI textoPregunta;
    public Button[] botonesRespuestas;

    [Header("Paneles de estado")]
    public GameObject panelCorrecto;
    public GameObject panelIncorrecto;
    public GameObject panelFinal;

    [Header("Controladores externos")] 
    public GameObject quizGameObject;       // El objeto "Quiz" (para AR, no se apagará automáticamente)

    private List<Pregunta> todasLasPreguntas;
    private List<string> decadasOrdenadas;
    private int indiceDecadaActual = 0;
    private int indicePreguntaActual = 0;
    private Pregunta preguntaActual;
    private string respuestaCorrectaActual;

    private void Start()
    {
        StartCoroutine(EsperarPreguntasCargadas());
    }

    private System.Collections.IEnumerator EsperarPreguntasCargadas()
    {
        while (!QuestionLoader.Instance.PreguntasCargadas)
            yield return null;

        todasLasPreguntas = QuestionLoader.Instance.questionList.preguntas;

        if (todasLasPreguntas == null || todasLasPreguntas.Count == 0)
        {
            Debug.LogError("❌ No hay preguntas cargadas en el JSON.");
            if (DebugUI.Instance != null)
                DebugUI.Instance.Log("❌ No hay preguntas cargadas en el JSON.");
            yield break;
        }

        decadasOrdenadas = todasLasPreguntas
            .Select(p => p.decada)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        MostrarPreguntaActual();
    }

    private void MostrarPreguntaActual()
    {
        if (indiceDecadaActual >= decadasOrdenadas.Count)
        {
            MostrarPanelFinal();
            return;
        }

        string decada = decadasOrdenadas[indiceDecadaActual];
        List<Pregunta> preguntasDeDecada = QuestionLoader.Instance.ObtenerPreguntasPorDecada(decada);

        if (preguntasDeDecada.Count == 0)
        {
            Debug.LogWarning($"⚠️ No hay preguntas en la década {decada}");
            if (DebugUI.Instance != null)
                DebugUI.Instance.Log($"⚠️ No hay preguntas en la década {decada}");
            indiceDecadaActual++;
            indicePreguntaActual = 0;
            MostrarPreguntaActual();
            return;
        }

        // Selecciona la pregunta según indicePreguntaActual
        preguntaActual = preguntasDeDecada[indicePreguntaActual];
        respuestaCorrectaActual = preguntaActual.respuestaCorrecta;

        // Mostrar en UI
        textoPregunta.text = preguntaActual.pregunta;

        List<string> respuestas = new List<string> {
            preguntaActual.respuesta1,
            preguntaActual.respuesta2,
            preguntaActual.respuesta3,
            preguntaActual.respuesta4
        }.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < botonesRespuestas.Length; i++)
        {
            int index = i;
            botonesRespuestas[i].GetComponentInChildren<TextMeshProUGUI>().text = respuestas[i];
            botonesRespuestas[i].onClick.RemoveAllListeners();
            botonesRespuestas[i].onClick.AddListener(() => Responder(respuestas[index]));
            botonesRespuestas[i].interactable = true;
        }

        if (DebugUI.Instance != null)
            DebugUI.Instance.Log($"🧠 Mostrando pregunta de la década {decada}: {preguntaActual.pregunta}");
    }

    public void Responder(string respuestaSeleccionada)
    {
        bool esCorrecta = respuestaSeleccionada == respuestaCorrectaActual;

        panelCorrecto.SetActive(esCorrecta);
        panelIncorrecto.SetActive(!esCorrecta);

        foreach (var btn in botonesRespuestas)
            btn.interactable = false;

        if (DebugUI.Instance != null)
            DebugUI.Instance.Log(esCorrecta ? "✅ Respuesta correcta" : "❌ Respuesta incorrecta");
    }

    // 🔹 Botón “Siguiente” para mostrar la próxima pregunta
    public void OnClick_Siguiente()
    {
        if (DebugUI.Instance != null)
            DebugUI.Instance.Log("🟢 Botón Siguiente presionado");

        // Apaga paneles de feedback
        panelCorrecto.SetActive(false);
        panelIncorrecto.SetActive(false);

        // Avanzar al siguiente índice de pregunta
        indicePreguntaActual++;

        string decada = decadasOrdenadas[indiceDecadaActual];
        List<Pregunta> preguntasDeDecada = QuestionLoader.Instance.ObtenerPreguntasPorDecada(decada);

        if (indicePreguntaActual < preguntasDeDecada.Count)
        {
            MostrarPreguntaActual();
        }
        else
        {
            // Pasar a la siguiente década
            indiceDecadaActual++;
            indicePreguntaActual = 0;

            if (indiceDecadaActual < decadasOrdenadas.Count)
                MostrarPreguntaActual();
            else
                MostrarPanelFinal();
        }
    }

    private void MostrarPanelFinal()
    {
        panelFinal.SetActive(true);
        if (DebugUI.Instance != null)
            DebugUI.Instance.Log("🎉 Se completaron todas las preguntas del JSON.");
        Debug.Log("🎉 Se completaron todas las preguntas del JSON.");
    }
}