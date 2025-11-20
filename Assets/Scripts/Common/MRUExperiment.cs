using UnityEngine;
using TMPro;
using System.Collections;

public class MRUExperiment : MonoBehaviour
{
    [Header("Objetos del experimento")]
    public Transform objeto;
    public Transform inicio;
    public Transform fin;
    public float velocidad = 2f;

    [Header("UI")]
    public TMP_InputField velocidadInput;
    public TextMeshProUGUI tiempoText;
    public GameObject botonIniciar;
    public GameObject botonQuiz;

    [Header("IA")]
    public LocalChatAI aiResponseManager;

    [Header("Audio del cronómetro")]
    public AudioSource relojSource;     //  AudioSource que reproduce el reloj
    public AudioClip sonidoReloj;       //  Sonido del reloj tipo "tic tac"

    private bool enMovimiento = false;
    private float tiempo = 0f;

    void Start()
    {
        ResetExperimento();
    }

    public void IniciarMovimiento()
    {
        if (float.TryParse(velocidadInput.text, out float v))
            velocidad = v;

        objeto.position = inicio.position;
        tiempo = 0f;
        enMovimiento = true;

        botonIniciar.SetActive(false);
        botonQuiz.SetActive(false);

        //  Iniciar sonido del reloj
        if (relojSource != null && sonidoReloj != null)
        {
            relojSource.clip = sonidoReloj;
            relojSource.loop = true;
            relojSource.Play();
        }
    }

    void Update()
    {
        if (enMovimiento)
        {
            tiempo += Time.deltaTime;
            tiempoText.text = $"Tiempo: {tiempo:F2} s";

            Vector3 direccion = (fin.position - inicio.position).normalized;
            float distanciaFrame = velocidad * Time.deltaTime;
            float distanciaRestante = Vector3.Distance(objeto.position, fin.position);

            if (distanciaRestante > distanciaFrame)
            {
                Vector3 nuevaPos = objeto.position + direccion * distanciaFrame;

                //  Mantener la altura del plano detectado
                nuevaPos.y = inicio.position.y;

                objeto.position = nuevaPos;

                // --- Rotación ---
                float radio = objeto.localScale.x / 2f;
                float rotacionAngulo = (distanciaFrame / (2 * Mathf.PI * radio)) * 360f;
                Vector3 ejeRotacion = Vector3.Cross(direccion, Vector3.up);
                objeto.Rotate(ejeRotacion, rotacionAngulo, Space.World);
            }
            else
            {
                Vector3 finalPos = fin.position;
                finalPos.y = inicio.position.y;  //  aseguramos nivelación
                objeto.position = finalPos;

                enMovimiento = false;

                if (relojSource != null) relojSource.Stop();
                StartCoroutine(MostrarResultados());
            }
        }
    }

    IEnumerator MostrarResultados()
    {
        yield return new WaitForSeconds(1f);

        botonQuiz.SetActive(true);

        if (aiResponseManager != null)
            StartCoroutine(aiResponseManager.ObtenerRetroalimentacion(tiempo, velocidad));
    }

    public void ResetExperimento()
    {
        objeto.position = inicio.position;
        tiempo = 0f;
        enMovimiento = false;

        tiempoText.text = "Tiempo: 0.00 s";
        botonIniciar.SetActive(true);
        botonQuiz.SetActive(false);

        //  Por si estaba reproduciendo sonido
        if (relojSource != null) relojSource.Stop();
    }
}
