using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public static DebugUI Instance;          // Instancia única (singleton)
    [Header("Texto donde se mostrará el debug")]
    public TextMeshProUGUI debugText;        // Asigna aquí el TMP del Canvas

    private void Awake()
    {
        // Garantiza que haya solo una instancia
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Log("🧩 DebugUI inicializado correctamente");
    }

    /// <summary>
    /// Muestra un mensaje en pantalla (el más nuevo arriba)
    /// </summary>
    public void Log(string mensaje)
    {
        if (debugText == null)
        {
            Debug.LogWarning("⚠️ No hay TextMeshPro asignado al DebugUI");
            return;
        }

        // Muestra lo más reciente arriba y limita a 20 líneas
        debugText.text = mensaje + "\n" + debugText.text;

        string[] lineas = debugText.text.Split('\n');
        if (lineas.Length > 20)
        {
            debugText.text = string.Join("\n", lineas, 0, 20);
        }
    }

    /// <summary>
    /// Limpia los mensajes viejos (puedes llamarlo al cambiar de escena o pregunta)
    /// </summary>
    public void Clear()
    {
        if (debugText != null)
            debugText.text = "";
    }
}
