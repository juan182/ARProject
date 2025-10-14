using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[System.Serializable]
public class ImageAction
{
    public string imageName;             // Nombre de la referencia de la imagen
    public GameObject[] objectsToActivate; // Objetos que se activan cuando la imagen es detectada
}

public class RAManager : MonoBehaviour
{
    [Header("AR Components")]
    public ARTrackedImageManager trackedImageManager;

    [Header("Asistente")]
    public AssistantController assistant; // Referencia al asistente

    [Header("Lista de imágenes y objetos")]
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
        {
            ProcessImage(img);
        }

        foreach (var img in eventArgs.updated)
        {
            ProcessImage(img);
        }
    }

    private void ProcessImage(ARTrackedImage trackedImage)
    {
        string imgName = trackedImage.referenceImage.name;

        // 1️⃣ Verifica si es la imagen del asistente
        if (!assistantActivated && imgName == "AsistenteImage")
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                assistantActivated = true;
                assistant.gameObject.SetActive(true);

                // Posición y orientación inicial
                assistant.transform.position = trackedImage.transform.position;
                assistant.transform.rotation = trackedImage.transform.rotation;

                Debug.Log("Asistente activado por AR: " + imgName);
            }
        }

        // 2️⃣ Recorre la lista de otras imágenes y objetos
        foreach (var imageAction in imagesToTrack)
        {
            if (imgName == imageAction.imageName)
            {
                bool isTracking = trackedImage.trackingState == TrackingState.Tracking;

                foreach (var obj in imageAction.objectsToActivate)
                {
                    obj.SetActive(isTracking);
                    if (isTracking)
                    {
                        // Opcional: colocar el objeto en la posición de la imagen
                        obj.transform.position = trackedImage.transform.position;
                        obj.transform.rotation = trackedImage.transform.rotation;
                    }
                }
            }
        }
    }
}
