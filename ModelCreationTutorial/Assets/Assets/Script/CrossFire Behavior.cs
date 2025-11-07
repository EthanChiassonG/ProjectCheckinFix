using UnityEngine;

public class CrossFire : MonoBehaviour
{
    public float duration = 3f;
    private Vector3 initialScale;
    private float elapsedTime = 0f;

    private void Start()
    {
        initialScale = transform.localScale;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        float scaleY = Mathf.Lerp(initialScale.y, 0f, elapsedTime / duration);
        transform.localScale = new Vector3(initialScale.x, scaleY, initialScale.z);

        if (elapsedTime >= duration)
        {
            Destroy(gameObject);
        }
    }
}
