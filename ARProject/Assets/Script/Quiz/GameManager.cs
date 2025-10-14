using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("UI de Pregunta")]
    public TextMeshProUGUI txtEnunciado;
    public Button[] botonesOpciones = new Button[4];
    public TextMeshProUGUI[] textosOpciones = new TextMeshProUGUI[4];
    public bool autoConectarBotones = false;

    [Header("Timer")]
    public QuizTimer quizTimer; // Nuevo

    [Header("Eventos")]
    public UnityEvent onAnsweredCorrect;
    public UnityEvent onAnsweredIncorrect;

    // Internos
    private int indiceCorrecto = 0;
    private Pregunta preguntaActual;
    private List<Pregunta> preguntasPorDecada;
    private int decadaIndex = 0; // 0=1970, 1=1980, etc.
    private string[] decadas = { "1970", "1980", "1990", "2000" };

    void Awake()
    {
        if (autoConectarBotones) ConectarBotonesAutomaticamente();
    }

    IEnumerator Start()
    {
        // Espera hasta que el JSON esté cargado
        yield return new WaitUntil(() =>
            QuestionLoader.Instance != null &&
            QuestionLoader.Instance.questionList != null &&
            QuestionLoader.Instance.questionList.preguntas.Count > 0
        );

        IniciarDecada(decadas[decadaIndex]);
    }

    public void IniciarDecada(string decada)
    {
        preguntasPorDecada = QuestionLoader.Instance.ObtenerPreguntasPorDecada(decada);
        CargarPreguntaAleatoria();
        quizTimer?.StartTimer();
    }

    void CargarPreguntaAleatoria()
    {
        if (preguntasPorDecada == null || preguntasPorDecada.Count == 0) return;

        preguntaActual = preguntasPorDecada[Random.Range(0, preguntasPorDecada.Count)];

        // Enunciado
        txtEnunciado?.SetText(preguntaActual.pregunta);

        // Opciones
        textosOpciones[0]?.SetText(preguntaActual.respuesta1);
        textosOpciones[1]?.SetText(preguntaActual.respuesta2);
        textosOpciones[2]?.SetText(preguntaActual.respuesta3);
        textosOpciones[3]?.SetText(preguntaActual.respuesta4);

        // Determinar índice correcto
        if (preguntaActual.respuesta1 == preguntaActual.respuestaCorrecta) indiceCorrecto = 0;
        else if (preguntaActual.respuesta2 == preguntaActual.respuestaCorrecta) indiceCorrecto = 1;
        else if (preguntaActual.respuesta3 == preguntaActual.respuestaCorrecta) indiceCorrecto = 2;
        else indiceCorrecto = 3;

        // Activar botones
        foreach (var b in botonesOpciones) if (b != null) b.interactable = true;
    }

    public void OnOpcionPulsada(int indicePulsado)
    {
        bool esCorrecta = (indicePulsado == indiceCorrecto);

        quizTimer?.ReiniciarTimer(); // Reinicia el reloj al responder
        foreach (var b in botonesOpciones) if (b != null) b.interactable = false;

        if (esCorrecta) onAnsweredCorrect?.Invoke();
        else onAnsweredIncorrect?.Invoke();

        // Opcional: avanzar a siguiente pregunta/decada
        decadaIndex++;
        if (decadaIndex < decadas.Length)
        {
            IniciarDecada(decadas[decadaIndex]);
        }
        else
        {
            Debug.Log("✅ Se completaron todas las décadas.");
            quizTimer?.StopTimer();
        }
    }

    void ConectarBotonesAutomaticamente()
    {
        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            int idx = i; // capturar índice
            if (botonesOpciones[i] != null)
            {
                botonesOpciones[i].onClick.RemoveAllListeners();
                botonesOpciones[i].onClick.AddListener(() => OnOpcionPulsada(idx));
            }
        }
    }
}