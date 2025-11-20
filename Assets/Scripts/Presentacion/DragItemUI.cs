using UnityEngine;
using UnityEngine.EventSystems;

public class DragItemUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public GameObject prefab3D;
    private GameObject currentInstance;
    private bool hasSpawned = false;

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Convierte la posición del mouse a coordenadas del mundo
        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 2f)
        );

        // Solo crear el objeto si no existe aún
        if (!hasSpawned)
        {
            currentInstance = Instantiate(prefab3D, spawnPos, Quaternion.identity);

            // Asegurar que tenga el script de arrastre 3D
            if (currentInstance.GetComponent<DraggableObject>() == null)
                currentInstance.AddComponent<DraggableObject>();

            hasSpawned = true;
        }
    }
}
