using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

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
    public static QuestionLoader Instance { get; private set; }
    public QuestionList questionList;
    public bool PreguntasCargadas { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // lanzar la carga como coroutine (compatible con Android)
            StartCoroutine(CargarPreguntasCoroutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator CargarPreguntasCoroutine()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "preguntas.json");
        string json = null;

        // En Android el path es "jar:file://..." y File.ReadAllText no funciona.
        if (path.Contains("://") || path.Contains(":///"))
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(path))
            {
                yield return uwr.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                if (uwr.result != UnityWebRequest.Result.Success)
#else
                if (uwr.isNetworkError || uwr.isHttpError)
#endif
                {
                    Debug.LogError($"❌ Error leyendo JSON desde StreamingAssets (UnityWebRequest): {uwr.error}  path={path}");
                    questionList = new QuestionList { preguntas = new List<Pregunta>() };
                }
                else
                {
                    json = uwr.downloadHandler.text;
                }
            }
        }
        else
        {
            // Editor / Standalone: se puede leer con File
            try
            {
                json = File.ReadAllText(path);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error leyendo JSON con File.ReadAllText: {e.Message} path={path}");
            }
            yield return null;
        }

        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                questionList = JsonUtility.FromJson<QuestionList>(json);
                if (questionList == null || questionList.preguntas == null)
                    questionList = new QuestionList { preguntas = new List<Pregunta>() };

                PreguntasCargadas = true;
                Debug.Log($"✅ Se cargaron {questionList.preguntas.Count} preguntas desde JSON");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Error parseando JSON: {e.Message}");
                questionList = new QuestionList { preguntas = new List<Pregunta>() };
            }
        }
        else
        {
            if (questionList == null)
                questionList = new QuestionList { preguntas = new List<Pregunta>() };
        }
    }

    public List<Pregunta> ObtenerPreguntasPorDecada(string decada)
    {
        if (questionList == null || questionList.preguntas == null)
            return new List<Pregunta>();

        return questionList.preguntas.FindAll(p => p.decada == decada);
    }
}