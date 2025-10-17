using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AssistantAudio
{
    public string name;      // Nombre del audio ("Inicio", "Recorrido1", etc.)
    public AudioClip clip;   // Archivo de audio
}

public class AudioManager : MonoBehaviour
{
    public List<AssistantAudio> assistantAudios = new List<AssistantAudio>();
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    public void PlayAssistantAudio(string name)
    {
        AssistantAudio a = assistantAudios.Find(x => x.name == name);
        if (a != null && a.clip != null)
        {
            audioSource.clip = a.clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Audio '{name}' no encontrado en AudioManager.");
        }
    }

    public void StopAssistantAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("🔇 Audio del asistente detenido manualmente.");
        }
    }


    public bool IsPlaying() => audioSource.isPlaying;

    // Devuelve la duración del clip del audio por nombre
    public float GetClipLength(string name)
    {
        AssistantAudio a = assistantAudios.Find(x => x.name == name);
        if (a != null && a.clip != null)
            return a.clip.length;
        else
            return 0f; // fallback
    }

}