using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RAManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager arTIM;

    // Si usas prefabs, ponlos aquí. Si usas objetos de la escena, también puedes arrastrarlos aquí.
    [SerializeField] private GameObject[] arModels2Place;

    private Dictionary<string, GameObject> arModels = new Dictionary<string, GameObject>();
    private Dictionary<string, bool> modelState = new Dictionary<string, bool>();

    private void Start()
    {
        foreach (var arModel in arModels2Place)
        {
            // ✅ No instanciamos: usamos los objetos ya existentes
            arModels.Add(arModel.name, arModel);
            arModel.SetActive(false);
            modelState.Add(arModel.name, false);
        }
    }

    private void OnEnable()
    {
        arTIM.trackedImagesChanged += ImageFound;
    }

    private void OnDisable()
    {
        arTIM.trackedImagesChanged -= ImageFound;
    }

    private void ImageFound(ARTrackedImagesChangedEventArgs eventData)
    {
        foreach (var trackedImage in eventData.added)
        {
            ShowARModel(trackedImage);
        }

        foreach (var trackedImage in eventData.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                ShowARModel(trackedImage);
            }
            // Si se pierde el tracking, no ocultamos el modelo
        }
    }

    private void ShowARModel(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        bool isModelActive = modelState[imageName];

        if (!isModelActive)
        {
            GameObject arModel = arModels[imageName];
            arModel.transform.position = trackedImage.transform.position;
            arModel.transform.rotation = trackedImage.transform.rotation;

            arModel.SetActive(true);
            modelState[imageName] = true;
        }
        // Si ya está activo, no se mueve.
    }
}
