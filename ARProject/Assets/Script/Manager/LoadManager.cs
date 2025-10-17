using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    [Header("Nombre de la escena a cargar")]
    [Tooltip("Escribe aquí el nombre exacto de la escena (debe estar en Build Settings)")]
    public string nombreEscena;

    // Llama a este método desde un botón (OnClick)
    public void CargarEscena()
    {
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogError("⚠️ No se ha asignado el nombre de la escena en el Inspector.");
            return;
        }

        // Verifica si la escena existe en los Build Settings
        if (SceneExiste(nombreEscena))
        {
            Debug.Log($"🔄 Cargando escena: {nombreEscena}");
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogError($"❌ La escena '{nombreEscena}' no está incluida en los Build Settings.");
        }
    }

    private bool SceneExiste(string nombre)
    {
        int total = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < total; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string escena = System.IO.Path.GetFileNameWithoutExtension(path);
            if (escena == nombre)
                return true;
        }
        return false;
    }
}