using UnityEngine;
using TMPro;

public class ResultPanelUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI tiempoText;
    public TextMeshProUGUI alcanceText;
    public TextMeshProUGUI alturaText;
    public TextMeshProUGUI feedbackText;

    public void ShowResults(float tiempo, float alcance, float altura, string name)
    {
        panel.SetActive(true);
        tiempoText.text = $"Tiempo: {tiempo:F2} s";
        alcanceText.text = $"Alcance: {alcance:F2} m";
        alturaText.text = $"Altura máxima: {altura:F2} m";

        // Ejemplo simple de evaluación
        string category = EvaluatePerformance(tiempo, alcance, altura);
        feedbackText.text = category;
    }

    string EvaluatePerformance(float t, float range, float h)
    {
        // lógica ejemplo: compara con valores esperados (puedes mejorar)
        // aquí solo devolvemos un mensaje genérico
        if (range < 1f) return "Parece que el lanzamiento fue muy bajo. Prueba aumentar la velocidad o el ángulo.";
        if (range > 15f) return "Buen alcance. Intenta ajustar la precisión de los parámetros.";
        return "Resultado aceptable. Revisa las fórmulas y repite el experimento.";
    }
}
