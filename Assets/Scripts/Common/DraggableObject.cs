using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private float distanceToCamera;

    void Start()
    {
        mainCamera = Camera.main;
        Debug.Log($"[DraggableObject] Script inicializado en: {gameObject.name}");
    }

    void OnMouseDown()
    {
        Debug.Log($"[DraggableObject] Clic detectado sobre: {gameObject.name}");
        distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = distanceToCamera;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

            transform.position = worldPos;

            Debug.Log($"[DraggableObject] Arrastrando {gameObject.name} a posición: {worldPos}");
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            Debug.Log($"[DraggableObject] Soltaste el objeto: {gameObject.name}");
            isDragging = false;
        }
    }
}
