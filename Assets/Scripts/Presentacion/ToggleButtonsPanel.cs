using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonsPanel : MonoBehaviour
{
    public GameObject botonOn;   // Botón que activa el panel
    public GameObject botonOff;  // Botón que desactiva el panel
    public GameObject panel;     // Panel que se mostrará / ocultará

    void Start()
    {
        // Estado inicial: solo el botón ON visible y el panel oculto
        botonOn.SetActive(true);
        botonOff.SetActive(false);
        panel.SetActive(false);
    }

    // Llamado al presionar el botón ON
    public void ActivarPanel()
    {
        panel.SetActive(true);   // Mostrar panel
        botonOn.SetActive(false);
        botonOff.SetActive(true);
        Debug.Log("Panel activado");
    }

    // Llamado al presionar el botón OFF
    public void DesactivarPanel()
    {
        panel.SetActive(false);  // Ocultar panel
        botonOn.SetActive(true);
        botonOff.SetActive(false);
        Debug.Log("Panel desactivado");
    }
}
