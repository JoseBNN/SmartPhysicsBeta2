using UnityEngine;
using TMPro;

public class ControlParametros : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_InputField gravedadInput;
    public TMP_InputField anguloInput;
    public TMP_InputField pesoInput;
    public TMP_InputField velocidadInput;
    // Valores actuales
    private float gravedad = 9.8f;
    private float angulo = 45f;
    private float peso = 1f;
    private float velocidad = 10f;

    private readonly float gravedadDefault = 9.8f;
    private readonly float anguloDefault = 45f;
    private readonly float pesoDefault = 1f;
    private readonly float velocidadDefault = 10f;
    private void Start()
    {
        ActualizarCampos();
    }

    public void CambiarGravedad(float cambio)
    {
        gravedad = Mathf.Clamp(gravedad + cambio, 0f, 50f);
        ActualizarCampos();
    }

    public void CambiarAngulo(float cambio)
    {
        angulo = Mathf.Clamp(angulo + cambio, 0f, 90f);
        ActualizarCampos();
    }

    public void CambiarPeso(float cambio)
    {
        peso = Mathf.Clamp(peso + cambio, 0.1f, 100f);
        ActualizarCampos();
    }

    public void CambiarVelocidad(float cambio)
    {
        velocidad = Mathf.Clamp(peso + cambio, 0.1f, 100f);
        ActualizarCampos();
    }

    public void AplicarCambios()
    {
        Debug.Log($"Cambios aplicados: Gravedad={gravedad}, Ángulo={angulo}, Peso={peso}, Velocidad={velocidad}");
        // Aquí puedes conectar con tu script de simulación:
        // simulador.SetParametros(gravedad, angulo, peso);
    }

    //  Nueva función: Deshacer cambios
    public void DeshacerCambios()
    {
        gravedad = 0f;
        angulo = 0f;
        peso = 0f;
        velocidad = 0f;
        ActualizarCampos();
        Debug.Log(" Cambios deshechos: todos los valores fueron reiniciados a 0.");
    }

    private void ActualizarCampos()
    {
        gravedadInput.text = gravedad.ToString("F1");
        anguloInput.text = angulo.ToString("F1");
        pesoInput.text = peso.ToString("F1");
        velocidadInput.text = velocidad.ToString("F1");
    }
}
