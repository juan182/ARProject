using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// --------------------------------------------------------------
/// GameManager
/// - Toma 1 pregunta aleatoria del JSON cargado por QuestionLoader.
/// - Rellena UI (enunciado + 4 opciones TMP).
/// - Maneja cronómetro (TMP) de cuenta atrás.
/// - Al responder o agotar tiempo: detiene/oculta reloj y
///   dispara UnityEvents (conéctalos a PanelCorrectas/PanelIncorrectas).
/// --------------------------------------------------------------
public class GameManager : MonoBehaviour
{
    // --------- TIEMPO (UI) ----------
    [Header("Tiempo (UI TMP)")]
    [Tooltip("Objeto padre del reloj para ocultarlo/mostrarlo (Canvas/Interfaz/Reloj).")]
    public GameObject relojGO;

    [Tooltip("Texto TMP que muestra los segundos (Canvas/Interfaz/Reloj/TextoTiempo).")]
    public TextMeshProUGUI textoTiempo;

    [Range(5, 120)]
    public int duracionSegundos = 40;

    [Tooltip("Si está activo, al iniciar la escena se carga una pregunta y arranca el reloj.")]
    public bool iniciarAutomatico = true;

    // --------- UI PREGUNTA ----------
    [Header("UI de la Pregunta")]
    [Tooltip("TMP del enunciado (Canvas/PanelPregunta/Enunciado).")]
    public TextMeshProUGUI txtEnunciado;

    [Tooltip("Botones A/B/C/D (en orden).")]
    public Button[] botonesOpciones = new Button[4];

    [Tooltip("TMP de cada opción, hijo de los botones (en el mismo orden A..D).")]
    public TextMeshProUGUI[] textosOpciones = new TextMeshProUGUI[4];

    [Tooltip("Si lo activas, el script conectará OnClick de los botones a OnOpcionPulsada(0..3).")]
    public bool autoConectarBotones = false;

    // --------- EVENTOS RESULTADO ----------
    [Header("Eventos (conéctalos en el Inspector)")]
    public UnityEvent onTimeUp;          // → PanelIncorrectas
    public UnityEvent onAnsweredCorrect; // → PanelCorrectas
    public UnityEvent onAnsweredIncorrect; // → PanelIncorrectas

    // --------- ESTADO INTERNO ----------
    Coroutine coTiempo;
    float tiempoRestante;
    bool corriendo;
    int indiceCorrecto = 0; // 0..3 calculado al cargar la pregunta

    void Awake()
    {
        // Muestra reloj y pone un valor inicial
        SetRelojVisible(true);
        ActualizarTiempoUI(duracionSegundos);
    }

IEnumerator Start()
{
    if (autoConectarBotones) ConectarBotonesAutomaticamente();

    if (iniciarAutomatico)
    {
        // Espera hasta que el JSON esté cargado
        yield return new WaitUntil(() =>
            QuestionLoader.Instance != null &&
            QuestionLoader.Instance.questionList != null &&
            QuestionLoader.Instance.questionList.preguntas != null &&
            QuestionLoader.Instance.questionList.preguntas.Count > 0
        );

        IniciarQuiz(); // ahora sí carga pregunta y arranca reloj
    }
}


    /// <summary>
    /// Punto de entrada del quiz: carga 1 pregunta aleatoria del JSON y enciende el cronómetro.
    /// </summary>
    public void IniciarQuiz()
    {
        if (!CargarPreguntaAleatoriaDesdeJSON())
        {
            Debug.LogError("❌ No se pudo cargar una pregunta desde el JSON. Revisa QuestionLoader y preguntas.json.");
            // Opcional: desactivar botones y reloj si no hay datos
            SetRelojVisible(false);
            for (int i = 0; i < botonesOpciones.Length; i++)
                if (botonesOpciones[i] != null) botonesOpciones[i].interactable = false;
            return;
        }

        StartTimer();
    }

    // =================== TIEMPO ===================
    void StartTimer()
    {
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
        textoTiempo.text = s.ToString(); // TMP
    }

    void StopTimerAndHide()
    {
        if (coTiempo != null) StopCoroutine(coTiempo);
        corriendo = false;
        SetRelojVisible(false);
    }

    void SetRelojVisible(bool v)
    {
        if (relojGO != null) relojGO.SetActive(v);
    }

    // =================== RESPUESTAS ===================
    /// <summary>
    /// Llamado por los botones A/B/C/D con los índices 0..3.
    /// </summary>
    public void OnOpcionPulsada(int indicePulsado)
    {
        if (!corriendo) return;

        bool esCorrecta = (indicePulsado == indiceCorrecto);
        StopTimerAndHide();

        if (esCorrecta) onAnsweredCorrect?.Invoke();
        else            onAnsweredIncorrect?.Invoke();
    }

    // =================== CARGA DESDE JSON ===================
    /// <summary>
    /// Elige una pregunta aleatoria del JSON (QuestionLoader) y coloca textos en UI.
    /// Devuelve true si se cargó correctamente.
    /// </summary>
    bool CargarPreguntaAleatoriaDesdeJSON()
    {
        // Verificamos que QuestionLoader ya haya cargado el archivo
        if (QuestionLoader.Instance == null ||
            QuestionLoader.Instance.questionList == null ||
            QuestionLoader.Instance.questionList.preguntas == null ||
            QuestionLoader.Instance.questionList.preguntas.Count == 0)
        {
            return false;
        }

        List<Pregunta> lista = QuestionLoader.Instance.questionList.preguntas;
        Pregunta p = lista[Random.Range(0, lista.Count)];

        // Enunciado
        if (txtEnunciado != null) txtEnunciado.SetText(p.pregunta);

        // Opciones
        if (textosOpciones != null && textosOpciones.Length >= 4)
        {
            textosOpciones[0].SetText(p.respuesta1);
            textosOpciones[1].SetText(p.respuesta2);
            textosOpciones[2].SetText(p.respuesta3);
            textosOpciones[3].SetText(p.respuesta4);
        }

        // Determinar índice de la opción correcta comparando strings
        if      (p.respuesta1 == p.respuestaCorrecta) indiceCorrecto = 0;
        else if (p.respuesta2 == p.respuestaCorrecta) indiceCorrecto = 1;
        else if (p.respuesta3 == p.respuestaCorrecta) indiceCorrecto = 2;
        else                                          indiceCorrecto = 3;

        // Habilitamos botones por si veníamos de un intento previo
        for (int i = 0; i < botonesOpciones.Length; i++)
            if (botonesOpciones[i] != null) botonesOpciones[i].interactable = true;

        return true;
    }

    // =================== UTILIDAD OPCIONAL ===================
    void ConectarBotonesAutomaticamente()
    {
        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            int idx = i; // capturar índice para la lambda
            if (botonesOpciones[i] == null) continue;

            botonesOpciones[i].onClick.RemoveAllListeners();
            botonesOpciones[i].onClick.AddListener(() => OnOpcionPulsada(idx));
        }
    }
}
