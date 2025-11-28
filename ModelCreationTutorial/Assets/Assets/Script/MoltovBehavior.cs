using UnityEngine;
using UnityEngine.UIElements;

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

    private void Update()
    {
        this.transform.Rotate(3f, 0f, 0f);
    }
    private void SpawnObject()
    {
        
        if (objectToSpawn != null)
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
    }
}
