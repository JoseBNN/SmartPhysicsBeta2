using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [Header("Nombres de las escenas")]
    public string sceneWithoutVuforia = "Experimento3D";
    public string sceneWithVuforia = "ExperimentoAR";

    private bool isVuforiaActive = false;

    // Si deseas guardar el estado aunque cambies de escena:
    private static bool vuforiaState = false;

    void Start()
    {
        // Sincroniza estado global
        isVuforiaActive = vuforiaState;
    }

    public void ToggleScene()
    {
        // Cambiar entre escenas según el estado actual
        if (isVuforiaActive)
        {
            Debug.Log(" Cambiando a escena sin Vuforia...");
            vuforiaState = false;
            SceneManager.LoadScene(sceneWithoutVuforia);
        }
        else
        {
            Debug.Log(" Cambiando a escena con Vuforia...");
            vuforiaState = true;
            SceneManager.LoadScene(sceneWithVuforia);
        }
    }
}
