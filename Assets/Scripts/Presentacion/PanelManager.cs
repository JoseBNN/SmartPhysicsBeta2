using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject panel; 

    // Activa o desactiva el panel
    public void TogglePanel()
    {
        bool estado = panel.activeSelf;
        panel.SetActive(!estado);
    }

 
    public void MostrarPanel()
    {
        panel.SetActive(true);
    }

    public void OcultarPanel()
    {
        panel.SetActive(false);
    }
}
