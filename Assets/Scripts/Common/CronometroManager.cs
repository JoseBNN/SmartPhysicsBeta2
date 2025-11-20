using UnityEngine;
using TMPro; 

public class CronometroManager : MonoBehaviour
{
    public TextMeshProUGUI tiempoTexto;
    public GameObject botonIniciar;
    public GameObject botonDetener;

    private float tiempo = 0f;
    private bool enMarcha = false;

    void Update()
    {
        if (enMarcha)
        {
            tiempo += Time.deltaTime;
            ActualizarTexto();
        }
    }

    public void IniciarCronometro()
    {
        tiempo = 0f;
        enMarcha = true;
        botonIniciar.SetActive(false);
        botonDetener.SetActive(true);
    }

    public void DetenerCronometro()
    {
        enMarcha = false;
        botonIniciar.SetActive(true);
        botonDetener.SetActive(false);
    }

    private void ActualizarTexto()
    {
        int minutos = Mathf.FloorToInt(tiempo / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        int milisegundos = Mathf.FloorToInt((tiempo * 100) % 100);

        tiempoTexto.text = $"{minutos:00}:{segundos:00}:{milisegundos:00}";
    }
}
