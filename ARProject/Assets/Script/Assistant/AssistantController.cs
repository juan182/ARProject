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

    [Header("Velocidad de movimiento")]
    public float speed = 2f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioManager = FindAnyObjectByType<AudioManager>();
        messagePanel.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(OnAwakeSequence());
    }

    private IEnumerator OnAwakeSequence()
    {
        // 🎬 La animación "Inicio" comienza sola al activar el asistente.
        // Aquí solo lanzamos el audio asociado.
        audioManager.PlayAssistantAudio("Inicio");

        // Espera hasta que el audio termine.
        yield return new WaitUntil(() => !audioManager.IsPlaying());

        // Empieza el primer recorrido.
        StartCoroutine(MoverAsistente(1, "Recorrido1"));
    }

    private IEnumerator MoverAsistente(int indexDestino, string audioName)
    {
        animator.SetBool("isWalking", true);

        messagePanel.SetActive(true);
        StartCoroutine(ParpadeoPanel(messagePanel));

        audioManager.PlayAssistantAudio(audioName);

        while (Vector3.Distance(transform.position, puntos[indexDestino].position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, puntos[indexDestino].position, speed * Time.deltaTime);
            yield return null;
        }

        animator.SetBool("isWalking", false);
        messagePanel.SetActive(false);

        // Espera fin de audio
        yield return new WaitUntil(() => !audioManager.IsPlaying());

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
