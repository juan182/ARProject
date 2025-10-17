using UnityEngine;
using System.Collections;

public class AssistantController : MonoBehaviour
{
    [Header("Referencias")]
    private Animator animator;
    [HideInInspector] public AudioManager audioManager;
    public Transform[] puntos; // [0]=inicio, [1]=A, [2]=B, [3]=C, [4]=D
    public GameObject messagePanel;

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
        Debug.Log($"🧩 AssistantController activo en objeto: {gameObject.name} (ID {GetInstanceID()})");
    }

    private void Start()
    {
        Debug.Log($"🧩 AssistantController iniciado en objeto: {gameObject.name} (ID {GetInstanceID()})");
    }

    // 🔹 Método para iniciar desde ARManager
    public void IniciarPresentacion()
    {
        if (iniciado) return;
        iniciado = true;
        StartCoroutine(OnAwakeSequence());
    }

    private IEnumerator OnAwakeSequence()
    {
        yield return new WaitForSeconds(0.3f); // breve espera de carga

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

        // Comienza la secuencia completa automáticamente
        yield return StartCoroutine(MoverAsistente(1, "Recorrido1"));
        yield return StartCoroutine(MoverAsistente(2, "Recorrido2"));
        yield return StartCoroutine(MoverAsistente(3, "Recorrido3"));
        yield return StartCoroutine(MoverAsistente(4, "Recorrido4"));
        yield return StartCoroutine(MoverAsistente(5, "Recorrido5"));
        yield return StartCoroutine(FinalSequence());
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

        // Desvíos
        Transform[] desvio = null;
        switch (indexDestino)
        {
            case 2: desvio = desvio1; break;
            case 3: desvio = desvio2; break;
            case 4: desvio = desvio3; break;
            case 5: desvio = desvio4; break;
        }

        if (desvio != null && desvio.Length > 0)
        {
            foreach (var p in desvio)
                yield return StartCoroutine(MoverHacia(p));
        }

        yield return StartCoroutine(MoverHacia(target));

        animator.SetBool("isWalking", false);

        if (messagePanel != null)
            messagePanel.SetActive(false);

        yield return new WaitUntil(() => !audioManager.IsPlaying());
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