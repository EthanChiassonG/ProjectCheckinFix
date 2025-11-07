using UnityEngine;

public class Mltov : MonoBehaviour
{
    public GameObject objectToSpawn;
    public string groundLayerName = "Ground Layer";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayerName))
        {
            SpawnObject();
            Destroy(gameObject);
        }
    }

    private void SpawnObject()
    {
        if (objectToSpawn != null)
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
    }
}
