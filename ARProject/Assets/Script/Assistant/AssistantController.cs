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
    public float speed = 2f;           // velocidad de desplazamiento
    public float rotationSpeed = 5f;   // velocidad de rotación

    private void Awake()
    {
        // Busca el Animator dentro del FBX hijo
        animator = GetComponentInChildren<Animator>();
        if (animator == null) Debug.LogError("Animator no encontrado en ningún hijo.");

        // Busca AudioManager en la escena
        audioManager = FindAnyObjectByType<AudioManager>();
        if (audioManager == null) Debug.LogError("AudioManager no encontrado en la escena.");

        // Inicializa panel de mensaje
        if (messagePanel != null) messagePanel.SetActive(false);
    }

    private void Start()
    {
        Debug.Log("OnAwakeSequence iniciado");
        StartCoroutine(OnAwakeSequence());
    }

    private IEnumerator OnAwakeSequence()
    {
        Debug.Log("Reproduciendo audio Inicio");
        audioManager.PlayAssistantAudio("Inicio");

        yield return new WaitUntil(() => !audioManager.IsPlaying());

        Debug.Log("Comenzando recorrido 1");
        StartCoroutine(MoverAsistente(1, "Recorrido1"));
    }

    private IEnumerator MoverAsistente(int indexDestino, string audioName)
    {
        if (indexDestino >= puntos.Length)
        {
            Debug.LogError("IndexDestino fuera de rango");
            yield break;
        }

        Transform target = puntos[indexDestino];
        if (target == null)
        {
            Debug.LogError("Punto destino no asignado: " + indexDestino);
            yield break;
        }

        animator.SetBool("isWalking", true);

        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
            StartCoroutine(ParpadeoPanel(messagePanel));
        }

        Debug.Log("Reproduciendo audio: " + audioName);
        audioManager.PlayAssistantAudio(audioName);

        while (Vector3.Distance(transform.position, target.position) > 0.05f)
        {
            // Movimiento hacia el destino
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            // Rotación suave hacia destino
            Vector3 direction = (target.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            yield return null;
        }

        animator.SetBool("isWalking", false);

        if (messagePanel != null) messagePanel.SetActive(false);

        yield return new WaitUntil(() => !audioManager.IsPlaying());

        Debug.Log("Llegó al punto " + indexDestino);

        // Manejo de preguntas y siguientes recorridos
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
