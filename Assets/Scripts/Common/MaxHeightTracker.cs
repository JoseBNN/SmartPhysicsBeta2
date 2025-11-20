using UnityEngine;

public class MaxHeightTracker : MonoBehaviour
{
    public float maxHeight = -Mathf.Infinity;
    void Update()
    {
        if (transform.position.y > maxHeight) maxHeight = transform.position.y;
    }
}
