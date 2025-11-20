using UnityEngine;

public class SpawnFromUI : MonoBehaviour
{
    public GameObject prefab; 
    public Transform spawnPoint; 

    public void SpawnObject()
    {
        if (prefab != null)
        {
            if (spawnPoint != null)
            {
                Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            }
            else
            {
                
                Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
                Instantiate(prefab, position, Quaternion.identity);
            }
        }
    }
}