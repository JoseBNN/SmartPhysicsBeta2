using UnityEngine;
using UnityEngine.InputSystem; 

public class FreeObjectDrag : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject selectedObject;
    private Vector3 offset;
    private float zCoord;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                selectedObject = hit.transform.gameObject;
                Debug.Log("Has hecho clic sobre: " + selectedObject.name);

                zCoord = mainCamera.WorldToScreenPoint(selectedObject.transform.position).z;
                offset = selectedObject.transform.position - GetMouseWorldPosition();
            }
        }

        if (Mouse.current.leftButton.isPressed && selectedObject != null)
        {
            selectedObject.transform.position = GetMouseWorldPosition() + offset;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            selectedObject = null;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = zCoord;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}
