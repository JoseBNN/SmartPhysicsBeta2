using UnityEngine;

public class soltarObjeto : MonoBehaviour
{
    private Vector3 offset;
    private Camera cam;
    private bool isDragging = false;

    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.WorldToScreenPoint(transform.position).z;
        offset = transform.position - cam.ScreenToWorldPoint(mousePos);

        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = cam.WorldToScreenPoint(transform.position).z;
            Vector3 targetPos = cam.ScreenToWorldPoint(mousePos) + offset;

            transform.position = targetPos;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}
