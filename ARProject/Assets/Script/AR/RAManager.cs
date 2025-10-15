using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[System.Serializable]
public class ImageAction
{
    public string imageName;              // Nombre de la referencia de imagen en la librería
    public GameObject[] objectsToActivate; // Objetos 3D que se activan
    public Canvas[] quizCanvases;          // Canvas de preguntas que se activan
}

public class RAManager : MonoBehaviour
{
    [Header("AR Components")]
    public ARTrackedImageManager trackedImageManager;

    [Header("Asistente")]
    public AssistantController assistant;

    [Header("Lista de imágenes y acciones")]
    public List<ImageAction> imagesToTrack;

    private bool assistantActivated = false;

    private void OnEnable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var img in eventArgs.added)
            ProcessImage(img);

        foreach (var img in eventArgs.updated)
            ProcessImage(img);
    }

    private void ProcessImage(ARTrackedImage trackedImage)
    {
        string imgName = trackedImage.referenceImage.name;
        bool isTracking = trackedImage.trackingState == TrackingState.Tracking;

        // 1️⃣ Activar el asistente con la primera imagen
        if (!assistantActivated && imgName == "AsistenteImage" && isTracking)
        {
            assistantActivated = true;
            assistant.gameObject.SetActive(true);

            // Colocar el asistente a 1 metro frente al muro y 5 cm más arriba
            Vector3 offset = trackedImage.transform.forward * 1.0f + Vector3.up * 0.05f;
            assistant.transform.position = trackedImage.transform.position + offset;
            assistant.transform.rotation = trackedImage.transform.rotation;

            Debug.Log("🤖 Asistente activado a 1 m frente al muro, orientado hacia afuera");
        }

        // 2️⃣ Procesar las demás imágenes (incluyendo los quizzes)
        foreach (var action in imagesToTrack)
        {
            if (imgName == action.imageName)
            {
                // --- Objetos 3D ---
                if (action.objectsToActivate != null)
                {
                    foreach (var obj in action.objectsToActivate)
                        if (obj != null) obj.SetActive(isTracking);
                }

                // --- Canvas de preguntas ---
                if (action.quizCanvases != null)
                {
                    foreach (var canvas in action.quizCanvases)
                        if (canvas != null) canvas.enabled = isTracking;
                }

                Debug.Log($"📸 Imagen '{imgName}' | Tracking: {isTracking}");
            }
        }
    }
}