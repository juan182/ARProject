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

        Debug.Log($"Intentando leer archivo JSON en: {path}");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            try
            {
                questionList = JsonUtility.FromJson<QuestionList>(json);
                if (questionList != null && questionList.preguntas != null)
                {
                    Debug.Log($"✅ Archivo JSON leído correctamente. Total de preguntas: {questionList.preguntas.Count}");
                    foreach (var p in questionList.preguntas)
                    {
                        Debug.Log($"→ {p.pregunta} (Década: {p.decada})");
                    }
                }
                else
                {
                    Debug.LogError("⚠ El archivo JSON fue leído, pero está vacío o mal estructurado.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error al convertir el JSON: {e.Message}");
            }
        }
        else
        {
            Debug.LogError($"❌ No se encontró el archivo preguntas.json en: {path}");
        }
    }

    public List<Pregunta> ObtenerPreguntasPorDecada(string decada)
    {
        if (questionList == null || questionList.preguntas == null)
        {
            Debug.LogError("❌ No hay preguntas cargadas. Asegúrate de que el JSON se leyó correctamente.");
            return new List<Pregunta>();
        }

        List<Pregunta> filtradas = questionList.preguntas.FindAll(p => p.decada == decada);

        Debug.Log($"📘 Preguntas encontradas para la década {decada}: {filtradas.Count}");
        return filtradas;
    }
}