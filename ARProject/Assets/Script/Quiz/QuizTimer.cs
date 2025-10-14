using UnityEngine;
using TMPro;
using System.Collections;

public class QuizTimer : MonoBehaviour
{
    [Header("UI Timer")]
    public TextMeshProUGUI textoTiempo;
    public int duracionSegundos = 40;

    [Header("Paneles")]
    public GameObject panelTiempoAgotado; // Panel opcional que se activa cuando se acaba el tiempo

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
            if (tiempoRestante < 0f) tiempoRestante = 0f;
            ActualizarTiempoUI(tiempoRestante);
            yield return null;
        }

        if (corriendo && tiempoRestante <= 0f)
        {
            StopTimer();

            if (panelTiempoAgotado != null)
                panelTiempoAgotado.SetActive(true);

            // Buscar QuestionManager en el mismo panel y avanzar
            QuestionManager qm = GetComponentInParent<QuestionManager>();
            if (qm != null)
            {
                qm.Invoke("AvanzarPregunta", 2f);
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