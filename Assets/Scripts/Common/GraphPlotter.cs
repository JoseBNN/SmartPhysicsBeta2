using UnityEngine;
using System.Collections.Generic;

public class GraphPlotter : MonoBehaviour
{
    public RectTransform graphArea;
    public LineRenderer lineRenderer;

    public void Plot(List<float> timeData, List<float> velocityData)
    {
        int count = timeData.Count;
        if (count != velocityData.Count) return;

        lineRenderer.positionCount = count;

        float width = graphArea.rect.width;
        float height = graphArea.rect.height;

        float maxTime = Mathf.Max(timeData.ToArray());
        float maxVelocity = Mathf.Max(velocityData.ToArray());

        for (int i = 0; i < count; i++)
        {
            float xNorm = timeData[i] / maxTime;
            float yNorm = velocityData[i] / maxVelocity;

            Vector3 pos = new Vector3(
                xNorm * width - width / 2,
                yNorm * height - height / 2,
                0f
            );

            lineRenderer.SetPosition(i, pos);
        }
    }
}
