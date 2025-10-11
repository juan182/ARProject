using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Tiempo (UI TMP)")]
    public GameObject relojGO;                 // Canvas/Interfaz/Reloj   (objeto raíz del reloj)
    public TextMeshProUGUI textoTiempo;        // Canvas/Interfaz/Reloj/TextoTiempo
    [Range(5,120)] public int duracionSegundos = 40;
    public bool iniciarAutomatico = true;

    [Header("Respuesta correcta (0..3) para la única pregunta actual")]
    [Range(0,3)] public int indiceCorrecto = 0;

    [Header("Eventos (conéctalos desde el Inspector)")]
    public UnityEvent onTimeUp;                // se acabó el tiempo -> mostrar PanelIncorrectas
    public UnityEvent onAnsweredCorrect;       // respondió correcto -> mostrar PanelCorrectas
    public UnityEvent onAnsweredIncorrect;     // respondió mal -> mostrar PanelIncorrectas

    // ---- internos ----
    Coroutine coTiempo;
    float tiempoRestante;
    bool corriendo;

    void Awake()
    {
        // Reloj visible pero parado
        SetRelojVisible(true);
        ActualizarTiempoUI(duracionSegundos);
    }

    void Start()
    {
        if (iniciarAutomatico) StartTimer();
    }

    // === CONTROL DEL TIEMPO ===
    public void StartTimer()
    {
        // prepara y arranca
        tiempoRestante = duracionSegundos;
        ActualizarTiempoUI(tiempoRestante);

        if (coTiempo != null) StopCoroutine(coTiempo);
        corriendo = true;
        SetRelojVisible(true);
        coTiempo = StartCoroutine(CoCuentaAtras());
    }

    IEnumerator CoCuentaAtras()
    {
        while (corriendo && tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime;
            if (tiempoRestante < 0f) tiempoRestante = 0f;
            ActualizarTiempoUI(tiempoRestante);
            yield return null;
        }

        // tiempo agotado
        if (corriendo && tiempoRestante <= 0f)
        {
            StopTimerAndHide();
            onTimeUp?.Invoke();
        }
    }

    void ActualizarTiempoUI(float t)
    {
    if (textoTiempo == null) return;
    int s = Mathf.CeilToInt(t);
    // cualquiera de las dos es válida:
    // textoTiempo.SetText("{0}", s);
    textoTiempo.text = s.ToString();
    }   

    public void StopTimerAndHide()
    {
        if (coTiempo != null) StopCoroutine(coTiempo);
        corriendo = false;
        SetRelojVisible(false); // <- desaparece el reloj de inmediato
    }

    void SetRelojVisible(bool v)
    {
        if (relojGO != null) relojGO.SetActive(v);
    }

    // === LLAMADA DESDE LOS BOTONES DE OPCIÓN ===
    // Conecta cada botón A/B/C/D a este método con el índice que le corresponda (0..3)
    public void OnOpcionPulsada(int indicePulsado)
    {
        if (!corriendo) return;

        bool esCorrecta = (indicePulsado == indiceCorrecto);
        StopTimerAndHide(); // detén y oculta el cronómetro

        if (esCorrecta) onAnsweredCorrect?.Invoke();
        else            onAnsweredIncorrect?.Invoke();
    }

    // (opcional) si se quiere setear la correcta por botón/inspector en caliente
    public void SetIndiceCorrecto(int idx)
    {
        indiceCorrecto = Mathf.Clamp(idx, 0, 3);
    }
}
