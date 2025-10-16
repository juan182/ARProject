using UnityEngine;
using System.Collections;

public class AssistantController : MonoBehaviour
{
    [Header("Referencias")]
    private Animator animator;
    [HideInInspector] public AudioManager audioManager;
    public Transform[] puntos; // [0]=inicio, [1]=A, [2]=B, [3]=C, [4]=D
    public GameObject messagePanel;

    [Header("Control de preguntas")]
    public bool finPregunta1 = false;
    public bool finPregunta2 = false;
    public bool finPregunta3 = false;
    public bool finPregunta4 = false;
    public bool finPregunta5 = false;

    [Header("Velocidad y movimiento")]
    public float speed = 2f;
    public float rotationSpeed = 5f;

    [Header("Desvíos opcionales")]
    public Transform[] desvio1; // entre A→B
    public Transform[] desvio2; // entre B→C
    public Transform[] desvio3; // entre C→D
    public Transform[] desvio4; // entre D→final

    private bool iniciado = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null) Debug.LogError("Animator no encontrado en ningún hijo.");

        audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager == null) Debug.LogError("AudioManager no encontrado en la escena.");

        if (messagePanel != null) messagePanel.SetActive(false);
    }

    // 🔹 Ya no inicia automáticamente aquí
    private void Start() { }

    // 🔹 Nuevo método para iniciar manualmente desde ARManager
    public void IniciarPresentacion()
    {
        if (iniciado) return; // Evita reiniciar dos veces
        iniciado = true;
        StartCoroutine(OnAwakeSequence());
    }

    private IEnumerator OnAwakeSequence()
    {
        // Espera leve para asegurar que todo esté cargado
        yield return new WaitForSeconds(0.3f);

        if (audioManager == null)
        {
            audioManager = FindAnyObjectByType<AudioManager>();
            if (audioManager == null)
            {
                Debug.LogError("❌ AudioManager aún no disponible.");
                yield break;
            }
        }

        audioManager.PlayAssistantAudio("Inicio");
        yield return new WaitUntil(() => !audioManager.IsPlaying());
        StartCoroutine(MoverAsistente(1, "Recorrido1"));
    }

    private IEnumerator MoverAsistente(int indexDestino, string audioName)
    {
        if (indexDestino >= puntos.Length) yield break;
        Transform target = puntos[indexDestino];
        if (target == null) yield break;

        animator.SetBool("isWalking", true);

        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
            StartCoroutine(ParpadeoPanel(messagePanel));
        }

        audioManager.PlayAssistantAudio(audioName);

        // Determinar si hay desvíos para este tramo
        Transform[] desvio = null;
        switch (indexDestino)
        {
            case 2: desvio = desvio1; break;
            case 3: desvio = desvio2; break;
            case 4: desvio = desvio3; break;
            case 5: desvio = desvio4; break;
        }

        // Recorre desvíos si existen
        if (desvio != null && desvio.Length > 0)
        {
            foreach (var p in desvio)
                yield return StartCoroutine(MoverHacia(p));
        }

        yield return StartCoroutine(MoverHacia(target));

        animator.SetBool("isWalking", false);

        if (messagePanel != null) messagePanel.SetActive(false);

        yield return new WaitUntil(() => !audioManager.IsPlaying());

        // Control de preguntas
        switch (indexDestino)
        {
            case 1:
                yield return new WaitUntil(() => finPregunta1);
                StartCoroutine(MoverAsistente(2, "Recorrido2"));
                break;
            case 2:
                yield return new WaitUntil(() => finPregunta2);
                StartCoroutine(MoverAsistente(3, "Recorrido3"));
                break;
            case 3:
                yield return new WaitUntil(() => finPregunta3);
                StartCoroutine(MoverAsistente(4, "Recorrido4"));
                break;
            case 4:
                yield return new WaitUntil(() => finPregunta4);
                StartCoroutine(MoverAsistente(5, "Recorrido5"));
                break;
            case 5:
                yield return new WaitUntil(() => finPregunta5);
                StartCoroutine(FinalSequence());
                break;
        }
    }

    private IEnumerator MoverHacia(Transform destino)
    {
        while (Vector3.Distance(transform.position, destino.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino.position, speed * Time.deltaTime);

            Vector3 dir = (destino.position - transform.position).normalized;
            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotationSpeed);
            }

            yield return null;
        }
    }

    private IEnumerator FinalSequence()
    {
        animator.SetBool("isFinal", true);
        audioManager.PlayAssistantAudio("Fin");

        yield return new WaitUntil(() => !audioManager.IsPlaying());
        Debug.Log("Asistente completó todas las fases.");
    }

    private IEnumerator ParpadeoPanel(GameObject panel)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();

        while (panel.activeSelf)
        {
            cg.alpha = (cg.alpha == 1) ? 0 : 1;
            yield return new WaitForSeconds(0.5f);
        }
    }
}