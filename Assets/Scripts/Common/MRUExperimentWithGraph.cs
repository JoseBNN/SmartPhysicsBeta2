using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class MRUExperimentWithGraph : MonoBehaviour
{
    [Header("Referencias del experimento")]
    public Transform objeto;
    public Transform inicio;
    public Transform fin;
    public float velocidad = 2f;

    [Header("UI")]
    public TMP_InputField velocidadInput;
    public TextMeshProUGUI tiempoText;
    public GameObject botonIniciar;
    public GameObject botonQuiz;
    public GameObject chartPanel;
    public LineRenderer lineRenderer;

    private bool enMovimiento = false;
    private float tiempo = 0f;
    private List<float> tiempoData = new List<float>();
    private List<float> distanciaData = new List<float>();
    private float distanciaTotal;

    void Start()
    {
        ResetExperimento();
        chartPanel.SetActive(false);
    }

    public void IniciarMovimiento()
    {
        if (float.TryParse(velocidadInput.text, out float v))
            velocidad = v;

        objeto.position = inicio.position;
        tiempo = 0f;
        tiempoData.Clear();
        distanciaData.Clear();
        distanciaTotal = Vector3.Distance(inicio.position, fin.position);

        enMovimiento = true;
        botonIniciar.SetActive(false);
        botonQuiz.SetActive(false);
        chartPanel.SetActive(false);
    }

    void Update()
    {
        if (enMovimiento)
        {
            tiempo += Time.deltaTime;
            tiempoText.text = $"Tiempo: {tiempo:F2} s";

            // Movimiento rectilíneo uniforme
            objeto.position = Vector3.MoveTowards(objeto.position, fin.position, velocidad * Time.deltaTime);

            // Guardar datos
            float distanciaRecorrida = Vector3.Distance(inicio.position, objeto.position);
            tiempoData.Add(tiempo);
            distanciaData.Add(distanciaRecorrida);

            // Verificar fin del recorrido
            if (Vector3.Distance(objeto.position, fin.position) < 0.01f)
            {
                enMovimiento = false;
                StartCoroutine(MostrarGrafica());
            }
        }
    }

    IEnumerator MostrarGrafica()
    {
        yield return new WaitForSeconds(1f);
        chartPanel.SetActive(true);
        DibujarGrafica();
        botonQuiz.SetActive(true);
    }

    void DibujarGrafica()
    {
        if (tiempoData.Count < 2) return;

        lineRenderer.positionCount = tiempoData.Count;

        float maxTiempo = tiempoData[tiempoData.Count - 1];
        float maxDistancia = distanciaTotal;

        for (int i = 0; i < tiempoData.Count; i++)
        {
            float x = (tiempoData[i] / maxTiempo) * 6f; // Escala eje X
            float y = (distanciaData[i] / maxDistancia) * 3f; // Escala eje Y
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    public void ResetExperimento()
    {
        objeto.position = inicio.position;
        tiempo = 0f;
        tiempoText.text = "Tiempo: 0.00 s";
        enMovimiento = false;
        botonIniciar.SetActive(true);
        botonQuiz.SetActive(false);
        chartPanel.SetActive(false);
    }
}
