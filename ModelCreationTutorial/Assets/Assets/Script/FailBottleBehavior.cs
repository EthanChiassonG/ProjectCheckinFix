using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;

public class FailBottleBehavior : MonoBehaviour
{
    private bool thrownback = false;
    public float travelspeed;
    public Rigidbody body;
    public GameObject enemy;
    private Vector3 dir;
    void Start()
    {
        body = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        enemy = GameObject.Find("MainEnemy");
        enemy.GetComponent<Transform>();
        this.transform.Rotate(0f, 0.1f, 0f);
        dir = this.transform.position - enemy.transform.position;

        if (thrownback == true)
        {
            this.transform.position -= dir * travelspeed * Time.deltaTime;
            body.useGravity = false;
            this.transform.Rotate(1f, 1f, 1f);
           
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            thrownback = true;
        }
        
    }
}
