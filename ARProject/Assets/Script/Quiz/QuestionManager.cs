using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    [Header("Configuración de preguntas")]
    public string decadaSeleccionada; // Permite cambiar la década desde el Inspector
    private List<Pregunta> preguntasFiltradas;
    private Pregunta preguntaActual;

    [Header("Referencias")]
    public QuizController quizController; // Controlador principal del quiz
    public QuizTimer timer; // Referencia al timer

    [Header("UI")]
    public TextMeshProUGUI textoPregunta;
    public Button[] botonesRespuestas; // Deben ser 4 botones
    public GameObject panelCorrecto;
    public GameObject panelIncorrecto;

    private void Start()
    {
        CargarPreguntasPorDecada();
        MostrarPreguntaAleatoria();
    }

    public void CargarPreguntasPorDecada()
    {
        preguntasFiltradas = QuestionLoader.Instance.ObtenerPreguntasPorDecada(decadaSeleccionada);
        if (preguntasFiltradas == null || preguntasFiltradas.Count == 0)
        {
            Debug.LogWarning($"⚠️ No hay preguntas para la década {decadaSeleccionada}");
        }
    }

    public void MostrarPreguntaAleatoria()
    {
        if (preguntasFiltradas == null || preguntasFiltradas.Count == 0) return;

        // Selecciona una pregunta aleatoria
        preguntaActual = preguntasFiltradas[Random.Range(0, preguntasFiltradas.Count)];
        textoPregunta.text = preguntaActual.pregunta;

        // Mezcla las respuestas
        List<string> respuestas = new List<string>
        {
            preguntaActual.respuesta1,
            preguntaActual.respuesta2,
            preguntaActual.respuesta3,
            preguntaActual.respuesta4
        };
        Shuffle(respuestas);

        // Asigna el texto a los botones y los listeners
        for (int i = 0; i < botonesRespuestas.Length; i++)
        {
            botonesRespuestas[i].GetComponentInChildren<TextMeshProUGUI>().text = respuestas[i];
            botonesRespuestas[i].onClick.RemoveAllListeners();

            string respuestaBoton = respuestas[i]; // necesario para cierre
            botonesRespuestas[i].onClick.AddListener(() => VerificarRespuesta(respuestaBoton));
        }

        // Reiniciar el timer
        if (timer != null)
        {
            timer.ReiniciarTimer();
        }

        // Ocultar paneles de feedback
        panelCorrecto.SetActive(false);
        panelIncorrecto.SetActive(false);
    }

    private void VerificarRespuesta(string respuestaSeleccionada)
    {
        // Detener el timer al responder
        if (timer != null)
            timer.StopTimer();

        bool correcta = respuestaSeleccionada == preguntaActual.respuestaCorrecta;

        if (correcta)
        {
            panelCorrecto.SetActive(true);
        }
        else
        {
            panelIncorrecto.SetActive(true);
        }

        // Pasar a la siguiente pregunta después de 2 segundos
        Invoke(nameof(AvanzarPregunta), 2f);
    }

    private void AvanzarPregunta()
    {
        if (quizController != null)
            quizController.SiguientePregunta();
        else
            Debug.LogWarning("⚠️ No se asignó el QuizController en el QuestionManager.");
    }

    // Mezclar lista
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            T temp = list[rnd];
            list[rnd] = list[i];
            list[i] = temp;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (botonesRespuestas != null)
        {
            foreach (var boton in botonesRespuestas)
            {
                if (boton != null)
                    boton.onClick.RemoveAllListeners();
            }
        }
    }
}