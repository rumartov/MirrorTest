using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main; // Найдем основную камеру в сцене
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Поворачиваем UI объект к камере
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }
    }
}