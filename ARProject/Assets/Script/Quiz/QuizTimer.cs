using UnityEngine;
using TMPro;
using System.Collections;

public class QuizTimer : MonoBehaviour
{
    [Header("UI Timer")]
    public TextMeshProUGUI textoTiempo;
    public int duracionSegundos = 40;

    [Header("Paneles")]
    public GameObject panelTiempoAgotado;

    private Coroutine coTiempo;
    private float tiempoRestante;
    private bool corriendo;

    private void OnEnable()
    {
        ReiniciarTimer();
    }

    public void StartTimer()
    {
        tiempoRestante = duracionSegundos;
        ActualizarTiempoUI(tiempoRestante);

        if (coTiempo != null)
            StopCoroutine(coTiempo);

        corriendo = true;
        coTiempo = StartCoroutine(CoCuentaAtras());
    }

    public void StopTimer()
    {
        if (coTiempo != null)
            StopCoroutine(coTiempo);

        corriendo = false;
    }

    public void ReiniciarTimer()
    {
        StopTimer();
        panelTiempoAgotado?.SetActive(false);
        StartTimer();
    }

    IEnumerator CoCuentaAtras()
    {
        while (corriendo && tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarTiempoUI(tiempoRestante);
            yield return null;
        }

        if (corriendo && tiempoRestante <= 0f)
        {
            StopTimer();
            panelTiempoAgotado?.SetActive(true);

            QuestionManager qm = GetComponentInParent<QuestionManager>();
            if (qm != null)
            {
                qm.Invoke("SiguientePreguntaConRetraso", 2f);
            }
        }
    }

    private void ActualizarTiempoUI(float t)
    {
        if (textoTiempo != null)
            textoTiempo.text = Mathf.CeilToInt(t).ToString();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}