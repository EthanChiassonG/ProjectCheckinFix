using UnityEngine;

public class ForwardProjectileBehavior : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lifetime = 3f;

    private float elapsedTime = 0f;

    private void Update()
    {
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
