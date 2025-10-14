using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Pregunta
{
    public string pregunta;
    public string respuesta1;
    public string respuesta2;
    public string respuesta3;
    public string respuesta4;
    public string respuestaCorrecta;
    public string decada;
}

[System.Serializable]
public class QuestionList
{
    public List<Pregunta> preguntas;
}

public class QuestionLoader : MonoBehaviour
{
    public static QuestionLoader Instance;
    public QuestionList questionList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CargarPreguntas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CargarPreguntas()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "preguntas.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            try
            {
                questionList = JsonUtility.FromJson<QuestionList>(json);
            }
            catch (System.Exception)
            {
                questionList = new QuestionList { preguntas = new List<Pregunta>() };
            }
        }
        else
        {
            questionList = new QuestionList { preguntas = new List<Pregunta>() };
        }
    }

    public List<Pregunta> ObtenerPreguntasPorDecada(string decada)
    {
        if (questionList == null || questionList.preguntas == null)
            return new List<Pregunta>();

        return questionList.preguntas.FindAll(p => p.decada == decada);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}