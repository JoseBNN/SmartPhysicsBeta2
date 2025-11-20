using UnityEngine;
using TMPro;

public class ProjectileLauncher : MonoBehaviour
{
    public GameObject prefab;
    public Transform spawnPoint;

    public TMP_InputField speedInput;
    public TMP_InputField angleInput;
    public TMP_InputField gravityInput;
    public TMP_InputField massInput;

    public LineRenderer trajectoryLine;
    public ExperimentManager experimentManager;

    public AudioSource launchSound;

    private GameObject currentInstance;

    // Direcciones F�JAS (no dependientes del ImageTarget)
    private Vector3 fixedRight = Vector3.right;
    private Vector3 fixedUp = Vector3.up;
    private Vector3 fixedForward = Vector3.forward;

    public void Launch()
    {
        if (launchSound != null) launchSound.Play();

        float v0 = float.TryParse(speedInput.text, out v0) ? v0 : 10f;
        float angleDeg = float.TryParse(angleInput.text, out angleDeg) ? angleDeg : 45f;
        float g = float.TryParse(gravityInput.text, out g) ? g : 9.8f;
        float m = float.TryParse(massInput.text, out m) ? m : 1f;

        Physics.gravity = new Vector3(0, -Mathf.Abs(g), 0);

        if (currentInstance != null) Destroy(currentInstance);
        currentInstance = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        var rb = currentInstance.GetComponent<Rigidbody>();
        if (rb != null) rb.mass = m;

        // �ngulo
        float angleRad = angleDeg * Mathf.Deg2Rad;

        // VELOCIDAD ? SIEMPRE igual, independiente de Vuforia
        Vector3 velocity =
            fixedForward * (v0 * Mathf.Cos(angleRad)) +
            fixedUp * (v0 * Mathf.Sin(angleRad));

        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }

        if (trajectoryLine != null)
            DrawTrajectory(v0, angleRad, g);

        experimentManager?.OnLaunch(currentInstance, v0, angleDeg, g, m);
    }

    void DrawTrajectory(float v0, float angleRad, float g)
    {
        int points = 50;
        trajectoryLine.positionCount = points;

        float T = 2f * v0 * Mathf.Sin(angleRad) / g;

        for (int i = 0; i < points; i++)
        {
            float t = (i / (float)(points - 1)) * T;
            float x = v0 * Mathf.Cos(angleRad) * t;
            float y = v0 * Mathf.Sin(angleRad) * t - 0.5f * g * t * t;

            Vector3 pos = spawnPoint.position + new Vector3(0, y, x);
            trajectoryLine.SetPosition(i, pos);
        }
    }
}
