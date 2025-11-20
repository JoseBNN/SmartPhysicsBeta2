using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExperimentManager : MonoBehaviour
{
    private GameObject current;
    private float spawnTime;
    private Vector3 spawnPos;

    public ResultPanelUI resultPanel;
    public GameObject chartPanel;      // Panel de gráfica
    public LineRenderer lineRenderer;  // LineRenderer de la gráfica
    public GameObject quizPanel;       // Panel del quiz
    public LocalChatAI aiManager;   //  referencia a tu IA local

    private List<float> timeData = new List<float>();
    private List<float> velocityData = new List<float>();
    private Rigidbody currentRb;
    private bool recording = false;
    private float timer = 0f;

    // === Llamado por el lanzador ===
    public void OnLaunch(GameObject instance, float v0, float angleDeg, float g, float m)
    {
        current = instance;
        spawnTime = Time.time;
        spawnPos = current.transform.position;

        var detector = current.AddComponent<CollisionNotifier>();
        detector.manager = this;

        currentRb = current.GetComponent<Rigidbody>();
        recording = true;
        timer = 0f;
        timeData.Clear();
        velocityData.Clear();
    }

    void FixedUpdate()
    {
        if (recording && currentRb != null)
        {
            timer += Time.fixedDeltaTime;
            timeData.Add(timer);
            velocityData.Add(currentRb.linearVelocity.magnitude);
        }
    }

    // === Llamado cuando el proyectil toca el suelo ===
    public void OnProjectileLanded(GameObject proj, Vector3 landPos)
    {
        recording = false;

        float flightTime = Time.time - spawnTime;
        float range = landPos.x - spawnPos.x;
        float maxHeight = GetMaxHeight(proj);
        float avgVelocity = 0f;

        if (velocityData.Count > 0)
            avgVelocity = Sum(velocityData) / velocityData.Count;

        // Mostrar resultados numéricos
        resultPanel.ShowResults(flightTime, range, maxHeight, proj.name);

        // 🔹 Llamar a la IA para retroalimentación local
        if (aiManager != null)
            StartCoroutine(aiManager.ObtenerRetroalimentacion(flightTime, avgVelocity));

        // 🔹 Mostrar la gráfica después
        StartCoroutine(ShowChartAfterResults());

        current = null;
    }

    IEnumerator ShowChartAfterResults()
    {
        yield return new WaitForSeconds(1.5f);
        chartPanel.SetActive(true);
        DrawChart();
    }

    public void CloseChartAndStartQuiz()
    {
        chartPanel.SetActive(false);
        quizPanel.SetActive(true);
    }

    float GetMaxHeight(GameObject proj)
    {
        var tracker = proj.GetComponent<MaxHeightTracker>();
        return tracker != null ? tracker.maxHeight : proj.transform.position.y;
    }

    void DrawChart()
    {
        if (timeData.Count < 2) return;

        lineRenderer.positionCount = timeData.Count;
        float maxT = timeData[timeData.Count - 1];
        float maxV = Mathf.Max(velocityData.ToArray());

        for (int i = 0; i < timeData.Count; i++)
        {
            float x = (timeData[i] / maxT) * 8f;
            float y = (velocityData[i] / maxV) * 4f;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    float Sum(List<float> list)
    {
        float total = 0f;
        foreach (float f in list) total += f;
        return total;
    }
}
