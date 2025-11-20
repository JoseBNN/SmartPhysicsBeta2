using UnityEngine;

public class CollisionNotifier : MonoBehaviour
{
    public ExperimentManager manager;
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Ground") || col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            manager?.OnProjectileLanded(gameObject, transform.position);
        }
    }
}
